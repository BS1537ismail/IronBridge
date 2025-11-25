using IronBridge.Shared.DTOs;
using Mapster;
using Booking.Domain.Interfaces;
using Booking.Repository.Interfaces;

namespace Booking.Domain.Managers;

public class BookingManager : IBookingManager
{
    private readonly IBookingRepository _bookingRepository;

    public BookingManager(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IList<Models.Booking>> GetAllBookingsAsync()
    {
        var bookings = await _bookingRepository.GetAllAsync();
        return bookings.Adapt<IList<Models.Booking>>();
    }

    public async Task<IList<Models.Booking>> GetActiveBookingsAsync()
    {
        var bookings = await _bookingRepository.GetActiveBookingsAsync();
        return bookings.Adapt<IList<Models.Booking>>();
    }

    public async Task<Models.Booking?> GetBookingByIdAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return null;
        return booking.Adapt<Models.Booking>();
    }

    public async Task<IList<Models.Booking>> GetBookingsByUserIdAsync(string userId)
    {
        var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
        return bookings.Adapt<IList<Models.Booking>>();
    }

    public async Task<IList<Models.Booking>> GetBookingsByStatusAsync(string status)
    {
        var bookings = await _bookingRepository.GetBookingsByStatusAsync(status);
        return bookings.Adapt<IList<Models.Booking>>();
    }

    public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto)
    {
        var booking = new Models.Booking
        {
            UserId = dto.UserId,
            ProductId = dto.ProductId,
            BookingDate = dto.BookingDate,
            Quantity = dto.Quantity,
            TotalAmount = dto.TotalAmount,
            Status = dto.Status,
            CreatedBy = dto.CreatedBy,
            IsActive = true
        };

        var savedBooking = await _bookingRepository.AddAsync(booking.Adapt<Repository.Models.Booking>());

        return new BookingDto
        {
            Id = savedBooking.Id,
            UserId = savedBooking.UserId,
            ProductId = savedBooking.ProductId,
            BookingDate = savedBooking.BookingDate,
            Quantity = savedBooking.Quantity,
            TotalAmount = savedBooking.TotalAmount,
            Status = savedBooking.Status,
            CreatedBy = savedBooking.CreatedBy,
            CreatedAt = savedBooking.CreatedAt,
            UpdatedAt = savedBooking.UpdatedAt,
            IsActive = savedBooking.IsActive
        };
    }

    public async Task<BookingDto?> UpdateBookingAsync(int id, UpdateBookingDto dto)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return null;

        booking.UserId = dto.UserId != "string" ? dto.UserId : booking.UserId;
        booking.ProductId = dto.ProductId != 0 ? dto.ProductId : booking.ProductId;
        booking.BookingDate = dto.BookingDate != default ? dto.BookingDate : booking.BookingDate;
        booking.Quantity = dto.Quantity != 0 ? dto.Quantity : booking.Quantity;
        booking.TotalAmount = dto.TotalAmount != 0 ? dto.TotalAmount : booking.TotalAmount;
        booking.Status = dto.Status != "string" ? dto.Status : booking.Status;
        booking.IsActive = dto.IsActive;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking);

        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            ProductId = booking.ProductId,
            BookingDate = booking.BookingDate,
            Quantity = booking.Quantity,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,
            CreatedBy = booking.CreatedBy,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt,
            IsActive = booking.IsActive
        };
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        booking.IsActive = false;
        booking.UpdatedAt = DateTime.UtcNow;
        await _bookingRepository.UpdateAsync(booking);
        return true;
    }

    public async Task<bool> UpdateBookingStatusAsync(int id, string status)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        booking.Status = status;
        booking.UpdatedAt = DateTime.UtcNow;
        await _bookingRepository.UpdateAsync(booking);
        return true;
    }
}
