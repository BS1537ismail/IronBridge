using Mapster;
using Microsoft.EntityFrameworkCore;
using Booking.Repository.Interfaces;
using Booking.Repository.Models;

namespace Booking.Repository.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _context;

    public BookingRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Models.Booking>> GetAllAsync()
    {
        var bookings = await _context.Bookings.ToListAsync();
        return bookings;
    }

    public async Task<IList<Models.Booking>> GetActiveBookingsAsync()
    {
        var bookings = await _context.Bookings
            .Where(b => b.IsActive)
            .ToListAsync();
        return bookings;
    }

    public async Task<Models.Booking?> GetByIdAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        return booking;
    }

    public async Task<IList<Models.Booking>> GetBookingsByUserIdAsync(string userId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId && b.IsActive)
            .ToListAsync();
        return bookings;
    }

    public async Task<IList<Models.Booking>> GetBookingsByStatusAsync(string status)
    {
        var bookings = await _context.Bookings
            .Where(b => b.Status == status && b.IsActive)
            .ToListAsync();
        return bookings;
    }

    public async Task<Models.Booking> AddAsync(Models.Booking booking)
    {
        var entity = booking.Adapt<Models.Booking>();
        _context.Bookings.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Models.Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
}
