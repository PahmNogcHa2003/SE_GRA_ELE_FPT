using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Payments;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.User.Service
{
    public interface IPaymentService
    {
        Task<PaymentUrlResponseDTO> CreateVnPayPaymentUrlAsync(CreatePaymentRequestDTO request, HttpContext httpContext, CancellationToken cancellationToken);
        Task<PaymentResultDTO> ProcessVnPayCallbackAsync(IQueryCollection collections, CancellationToken cancellationToken);
    }
}
