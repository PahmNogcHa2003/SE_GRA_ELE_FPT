using Application.DTOs.Payments;
using Application.Interfaces.User.Service;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using static Application.Interfaces.User.Service.IPaymentGatewayService;
using Domain.Enums;
using Application.Interfaces.User.Repository;
using Application.Interfaces.Staff.Repository;
using Application.Services.Identity;
using Domain.Rules;

namespace Application.Services.User
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletService _walletService;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPromotionCampaignRepository _promotionCampaignRepository;
        public PaymentService(
            IUnitOfWork unitOfWork,
        IWalletService walletService,
        IPaymentGatewayService paymentGatewayService,
        IPaymentRepository paymentRepository,
        IPromotionCampaignRepository promotionCampaignRepository,
        IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork;
            _walletService = walletService;
            _paymentGatewayService = paymentGatewayService;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _promotionCampaignRepository = promotionCampaignRepository;
        }

        public async Task<PaymentUrlResponseDTO> CreateVnPayPaymentUrlAsync(CreatePaymentRequestDTO request, HttpContext httpContext, CancellationToken cancellationToken)
        {
            var userId = httpContext.User.GetUserIdAsLong();
            if (userId is null)
                throw new UnauthorizedAccessException("Invalid or missing user id in token.");
            var order = new Order
            {
                UserId = userId.Value,
                OrderNo = GenerateOrderNo(userId.Value),
                OrderType = "WalletTopUp",
                Status = OrderStatus.Pending,
                Subtotal = request.Amount,
                Total = request.Amount,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            var payment = new Payment
            {
                Order = order, 
                Provider = "VNPay",
                Amount = request.Amount,
                Status = PaymentStatus.Pending,
                ProviderTxnRef = Guid.NewGuid().ToString(), 
                CreatedAt = DateTimeOffset.UtcNow,
            };

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            payment.OrderId = order.Id; 
            var paymentInfo = new PaymentInfo
            {
                OrderId = order.Id, 
                Amount = payment.Amount,
                OrderInfo = $"Nap tien {payment.Amount:N0} VND vao vi",
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"

            };
            var paymentUrl = _paymentGatewayService.CreatePaymentUrl(paymentInfo);

            payment.CheckoutUrl = paymentUrl;

            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PaymentUrlResponseDTO { PaymentUrl = paymentUrl };
        }

        public async Task<PaymentResultDTO> ProcessVnPayCallbackAsync(IQueryCollection collections, CancellationToken cancellationToken)
        {
            if (!_paymentGatewayService.ValidateSignature(collections))
                return new PaymentResultDTO { IsSuccess = false, Message = "Invalid signature.", RspCode = "97" };

            var vnp_TxnRef = collections["vnp_TxnRef"].FirstOrDefault();
            var vnp_ResponseCode = collections["vnp_ResponseCode"].FirstOrDefault();
            var vnp_TransactionNo = collections["vnp_TransactionNo"].FirstOrDefault();

            if (string.IsNullOrEmpty(vnp_TxnRef))
                return new PaymentResultDTO { IsSuccess = false, Message = "Transaction reference not found.", RspCode = "01" };

            var parts = vnp_TxnRef.Split('_');
            if (parts.Length < 2 || !long.TryParse(parts[1], out long orderId))
                return new PaymentResultDTO { IsSuccess = false, Message = "Invalid transaction reference format.", RspCode = "01" };

            var payment = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);
            if (payment == null)
                return new PaymentResultDTO { IsSuccess = false, Message = "Order not found.", RspCode = "01" };
            if (payment.Status == PaymentStatus.Success)
            {
                return new PaymentResultDTO
                {
                    IsSuccess = false,
                    Message = "Giao dịch này đã được xác nhận trước đó. Không thể xử lý lại.",
                    RspCode = "02"
                };
            }
            var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                payment.GatewayTxnId = vnp_TransactionNo;
                payment.PaidAt = DateTimeOffset.UtcNow;

                if (vnp_ResponseCode == "00")
                {
                    payment.Status = PaymentStatus.Success;
                    payment.Order.Status = OrderStatus.Success;
                    payment.Order.PaidAt = DateTimeOffset.UtcNow;

                    var now = DateTimeOffset.UtcNow;
                    // Ghi nhận nạp ví
                    var walletTxn = await _walletService.CreditAsync(
                        payment.Order.UserId, 
                        payment.Amount, "VNPay", 
                        payment.OrderId, 
                        cancellationToken);
                    // Tính toán khuyến mãi
                    decimal promoBonus = 0m;
                    var activePromo = await _promotionCampaignRepository
                        .GetActivePromotionAsync(payment.Amount, now, cancellationToken);

                    if (activePromo != null)
                    {
                        promoBonus = payment.Amount * (activePromo.BonusPercent / 100m);
                    }
                    else
                    {
                        promoBonus = PromoRules.CalculateBonus(payment.Amount);
                    }

                    WalletTransaction? promoTxn = null;
                    if (promoBonus > 0)
                    {
                        promoTxn = await _walletService.CreditPromoAsync(
                            payment.Order.UserId,
                            promoBonus,
                            "WalletTopupPromo",
                            cancellationToken);
                    }
                    _paymentRepository.Update(payment);
                    _orderRepository.Update(payment.Order);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitTransactionAsync(transaction, cancellationToken);

                    // ✅ Chuẩn bị dữ liệu gửi FE
                    return new PaymentResultDTO
                    {
                        IsSuccess = true,
                        Message = "Thanh toán thành công.",
                        RspCode = "00",
                        Order = new
                        {
                            payment.Order.Id,
                            payment.Order.OrderNo,
                            payment.Order.Total,
                            payment.Order.Status,
                            payment.Order.PaidAt,
                            payment.Order.CreatedAt
                        },
                        Transaction = new
                        {
                            Wallet = new
                            {
                                walletTxn.Id,
                                walletTxn.Amount,
                                walletTxn.Direction,
                                walletTxn.Source,
                                walletTxn.BalanceAfter,
                                walletTxn.CreatedAt
                            },
                            Promo = promoTxn == null ? null : new
                            {
                                promoTxn.Id,
                                promoTxn.Amount,
                                promoTxn.Direction,
                                promoTxn.Source,
                                promoTxn.PromoAfter,
                                promoTxn.CreatedAt
                            }
                        }
                    };
                }
                else
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.Order.Status = OrderStatus.Failed;
                    payment.FailureReason = $"VNPay response code: {vnp_ResponseCode}";

                    _paymentRepository.Update(payment);
                    _orderRepository.Update(payment.Order);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitTransactionAsync(transaction, cancellationToken);

                    return new PaymentResultDTO
                    {
                        IsSuccess = false,
                        Message = "Thanh toán thất bại hoặc bị hủy.",
                        RspCode = vnp_ResponseCode,
                        Order = new
                        {
                            payment.Order.Id,
                            payment.Order.OrderNo,
                            payment.Order.Total,
                            payment.Order.Status,
                            payment.Order.CreatedAt
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(transaction, cancellationToken);
                return new PaymentResultDTO { IsSuccess = false, Message = $"Error: {ex.Message}", RspCode = "99" };
            }
        }

        private static string GenerateOrderNo(long userId)
      => $"ORD-{userId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
    }
}