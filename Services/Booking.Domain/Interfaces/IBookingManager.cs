using IronBridge.Shared.DTOs;

namespace Booking.Domain.Interfaces;

public interface IBookingManager
{
    Task<IList<Models.Booking>> GetAllBookingsAsync();
    Task<IList<Models.Booking>> GetActiveBookingsAsync();
    Task<Models.Booking?> GetBookingByIdAsync(int id);
    Task<IList<Models.Booking>> GetBookingsByUserIdAsync(string userId);
    Task<IList<Models.Booking>> GetBookingsByStatusAsync(string status);
    Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto);
    Task<BookingDto?> UpdateBookingAsync(int id, UpdateBookingDto dto);
    Task<bool> DeleteBookingAsync(int id);
    Task<bool> UpdateBookingStatusAsync(int id, string status);
}
