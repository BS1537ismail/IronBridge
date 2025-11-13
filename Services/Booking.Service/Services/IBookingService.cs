using IronBridge.Shared.DTOs;

namespace Booking.Service.Services;

public interface IBookingService
{
    Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto);
    Task<BookingDto?> GetBookingByIdAsync(int id);
    Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(Guid userId);
    Task<BookingDto?> UpdateBookingStatusAsync(int id, UpdateBookingStatusDto dto);
}
