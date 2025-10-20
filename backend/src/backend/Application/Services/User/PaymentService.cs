using Application.DTOs.Payments;
using Application.Interfaces.User.Service;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using static Application.Interfaces.User.Service.IPaymentGatewayService;
using Domain.Enums;

namespace Application.Services.User
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletService _walletService;
        private readonly IPaymentGatewayService _paymentGatewayService;

        public PaymentService(
            IUnitOfWork unitOfWork,
        IWalletService walletService,
        IPaymentGatewayService paymentGatewayService)
        {
            _unitOfWork = unitOfWork;
            _walletService = walletService;
            _paymentGatewayService = paymentGatewayService;
        }

        public async Task<PaymentUrlResponseDTO> CreateVnPayPaymentUrlAsync(CreatePaymentRequestDTO request, HttpContext httpContext, CancellationToken cancellationToken)
        {
            // Bước 1: Khởi tạo các đối tượng trong bộ nhớ (chưa lưu vào DB)
            var order = new Order
            {
                UserId = request.UserId,
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

            // Bước 2: Lưu tạm vào DB để lấy OrderId
            // Đây là cách tiếp cận đơn giản và hiệu quả nhất
            await _unitOfWork.Orders.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken); // Lấy Order.Id

            payment.OrderId = order.Id; // Gán OrderId cho payment

            // Bước 3: Chuẩn bị thông tin và gọi Gateway Service
            var paymentInfo = new PaymentInfo
            {
                OrderId = order.Id, // Dùng Id vừa được tạo
                Amount = payment.Amount,
                OrderInfo = $"Nap tien {payment.Amount:N0} VND vao vi",
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
            };
            var paymentUrl = _paymentGatewayService.CreatePaymentUrl(paymentInfo);

            payment.CheckoutUrl = paymentUrl;

            // Bước 4: Thêm payment và lưu tất cả thay đổi còn lại
            await _unitOfWork.Payments.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PaymentUrlResponseDTO { PaymentUrl = paymentUrl };
        }

        public async Task<PaymentResultDTO> ProcessVnPayCallbackAsync(IQueryCollection collections, CancellationToken cancellationToken)
        {
            // Bước 1: Ra lệnh cho "thợ kỹ thuật" (VnPayService) xác thực chữ ký
            if (!_paymentGatewayService.ValidateSignature(collections))
            {
                return new PaymentResultDTO { IsSuccess = false, Message = "Invalid signature.", RspCode = "97" };
            }

            var vnp_TxnRef = collections["vnp_TxnRef"].FirstOrDefault();
            var vnp_ResponseCode = collections["vnp_ResponseCode"].FirstOrDefault();
            var vnp_TransactionNo = collections["vnp_TransactionNo"].FirstOrDefault();

            if (string.IsNullOrEmpty(vnp_TxnRef))
            {
                return new PaymentResultDTO { IsSuccess = false, Message = "Transaction reference not found.", RspCode = "01" };
            }

            var parts = vnp_TxnRef.Split('_');
            if (parts.Length < 2 || !long.TryParse(parts[1], out long orderId))
            {
                return new PaymentResultDTO { IsSuccess = false, Message = "Invalid transaction reference format.", RspCode = "01" };
            }
            // Bước 2: Xử lý nghiệp vụ chính
            var payment = await _unitOfWork.Payments.GetByOrderIdAsync(orderId, cancellationToken);
            if (payment == null)
            {
                return new PaymentResultDTO { IsSuccess = false, Message = "Order not found.", RspCode = "01" };
            }
            if (payment.Status == PaymentStatus.Success)
            {
                return new PaymentResultDTO { IsSuccess = true, Message = "Order already confirmed.", RspCode = "02" };
            }

            // Bước 3: Dùng transaction để đảm bảo toàn vẹn dữ liệu
            var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                payment.GatewayTxnId = vnp_TransactionNo;
                payment.PaidAt = DateTimeOffset.UtcNow;

                if (vnp_ResponseCode == "00")
                {
                    payment.Status = PaymentStatus.Success;
                    payment.Order.Status = PaymentStatus.Success;
                    payment.Order.PaidAt = DateTimeOffset.UtcNow;

                    // Gọi WalletService để xử lý logic nạp tiền và trừ nợ
                    await _walletService.CreditAsync(payment.Order.UserId, payment.Amount, "VNPay", payment.OrderId, cancellationToken);
                }
                else
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.Order.Status = OrderStatus.Failed;
                    payment.FailureReason = $"VNPay response code: {vnp_ResponseCode}";
                }
                _unitOfWork.Payments.Update(payment);
                _unitOfWork.Orders.Update(payment.Order);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(transaction, cancellationToken);

                return new PaymentResultDTO { IsSuccess = true, Message = "Confirm Success.", RspCode = "00" };
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(transaction, cancellationToken);
                return new PaymentResultDTO { IsSuccess = false, Message = "An error occurred during processing.", RspCode = "99" };
            }
        }
    }
}