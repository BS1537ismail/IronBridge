using Booking.Domain.DTOs;
using Booking.Domain.Models;

namespace Booking.Domain.Interfaces;

public interface ISSLCommerzService
{
    Task<SSLCommerzInitResponse> InitiatePaymentAsync(CreatePaymentDto paymentDto, string orderId);
    Task<SSLCommerzValidationResponse> ValidatePaymentAsync(string transactionId);
}
