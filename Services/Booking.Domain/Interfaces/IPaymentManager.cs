using Booking.Domain.DTOs;
using Booking.Domain.Models;

namespace Booking.Domain.Interfaces;

public interface IPaymentManager
{
    Task<PaymentResponseDto> InitiatePaymentAsync(CreatePaymentDto paymentDto, string userId);
    Task<Payment?> ValidateAndUpdatePaymentAsync(string transactionId);
    Task<Payment?> GetPaymentByIdAsync(int id);
    Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId);
    Task<Payment?> GetPaymentByBookingIdAsync(int bookingId);
}
