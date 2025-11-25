namespace Booking.Repository.Interfaces;

public interface IBookingRepository
{
    Task<IList<Models.Booking>> GetAllAsync();
    Task<IList<Models.Booking>> GetActiveBookingsAsync();
    Task<Models.Booking?> GetByIdAsync(int id);
    Task<IList<Models.Booking>> GetBookingsByUserIdAsync(string userId);
    Task<IList<Models.Booking>> GetBookingsByStatusAsync(string status);
    Task<Models.Booking> AddAsync(Models.Booking booking);
    Task UpdateAsync(Models.Booking booking);
}
