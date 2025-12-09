using Booking.Repository.Models;

namespace Booking.Repository.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment?> GetByIdAsync(int id);
    Task<Payment?> GetByTransactionIdAsync(string transactionId);
    Task<Payment?> GetByBookingIdAsync(int bookingId);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment> UpdateAsync(Payment payment);
    Task<bool> DeleteAsync(int id);
}
