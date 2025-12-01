using IronBridge.Shared.DTOs;
using IronBridge.Shared.Interfaces;
using Mapster;
using Booking.Domain.Interfaces;
using Booking.Repository.Interfaces;

namespace Booking.Domain.Managers;

public class BookingManager : IBookingManager
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ICacheService _cacheService;

    public BookingManager(IBookingRepository bookingRepository, ICacheService cacheService)
    {
        _bookingRepository = bookingRepository;
        _cacheService = cacheService;
    }

    public async Task<IList<Models.Booking>> GetAllBookingsAsync()
    {
        var cacheKey = "bookings_all";
        var cachedBookings = await _cacheService.GetAsync<IList<Models.Booking>>(cacheKey);

        if (cachedBookings != null)
            return cachedBookings;

        var bookings = await _bookingRepository.GetAllAsync();
        var bookingList = bookings.Adapt<IList<Models.Booking>>();

        await _cacheService.SetAsync(cacheKey, bookingList, TimeSpan.FromMinutes(3));
        return bookingList;
    }

    public async Task<IList<Models.Booking>> GetActiveBookingsAsync()
    {
        var cacheKey = "bookings_active";
        var cachedBookings = await _cacheService.GetAsync<IList<Models.Booking>>(cacheKey);

        if (cachedBookings != null)
            return cachedBookings;

        var bookings = await _bookingRepository.GetActiveBookingsAsync();
        var bookingList = bookings.Adapt<IList<Models.Booking>>();

        await _cacheService.SetAsync(cacheKey, bookingList, TimeSpan.FromMinutes(3));
        return bookingList;
    }

    public async Task<Models.Booking?> GetBookingByIdAsync(int id)
    {
        var cacheKey = $"booking_{id}";
        var cachedBooking = await _cacheService.GetAsync<Models.Booking>(cacheKey);

        if (cachedBooking != null)
            return cachedBooking;

        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return null;

        var bookingModel = booking.Adapt<Models.Booking>();
        await _cacheService.SetAsync(cacheKey, bookingModel, TimeSpan.FromMinutes(3));

        return bookingModel;
    }

    public async Task<IList<Models.Booking>> GetBookingsByUserIdAsync(string userId)
    {
        var cacheKey = $"bookings_user_{userId}";
        var cachedBookings = await _cacheService.GetAsync<IList<Models.Booking>>(cacheKey);

        if (cachedBookings != null)
            return cachedBookings;

        var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
        var bookingList = bookings.Adapt<IList<Models.Booking>>();

        await _cacheService.SetAsync(cacheKey, bookingList, TimeSpan.FromMinutes(3));
        return bookingList;
    }

    public async Task<IList<Models.Booking>> GetBookingsByStatusAsync(string status)
    {
        var cacheKey = $"bookings_status_{status}";
        var cachedBookings = await _cacheService.GetAsync<IList<Models.Booking>>(cacheKey);

        if (cachedBookings != null)
            return cachedBookings;

        var bookings = await _bookingRepository.GetBookingsByStatusAsync(status);
        var bookingList = bookings.Adapt<IList<Models.Booking>>();

        await _cacheService.SetAsync(cacheKey, bookingList, TimeSpan.FromMinutes(3));
        return bookingList;
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

        var bookingDto = new BookingDto
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

        await _cacheService.RemoveAsync("bookings_all");
        await _cacheService.RemoveAsync("bookings_active");
        await _cacheService.RemoveAsync($"bookings_user_{savedBooking.UserId}");

        return bookingDto;
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

        var bookingDto = new BookingDto
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

        await _cacheService.RemoveAsync($"booking_{id}");
        await _cacheService.RemoveAsync("bookings_all");
        await _cacheService.RemoveAsync("bookings_active");
        await _cacheService.RemoveAsync($"bookings_user_{booking.UserId}");
        await _cacheService.RemoveAsync($"bookings_status_{booking.Status}");

        return bookingDto;
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        booking.IsActive = false;
        booking.UpdatedAt = DateTime.UtcNow;
        await _bookingRepository.UpdateAsync(booking);

        await _cacheService.RemoveAsync($"booking_{id}");
        await _cacheService.RemoveAsync("bookings_all");
        await _cacheService.RemoveAsync("bookings_active");
        await _cacheService.RemoveAsync($"bookings_user_{booking.UserId}");

        return true;
    }

    public async Task<bool> UpdateBookingStatusAsync(int id, string status)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return false;

        var oldStatus = booking.Status;
        booking.Status = status;
        booking.UpdatedAt = DateTime.UtcNow;
        await _bookingRepository.UpdateAsync(booking);

        await _cacheService.RemoveAsync($"booking_{id}");
        await _cacheService.RemoveAsync("bookings_all");
        await _cacheService.RemoveAsync("bookings_active");
        await _cacheService.RemoveAsync($"bookings_status_{oldStatus}");
        await _cacheService.RemoveAsync($"bookings_status_{status}");
        await _cacheService.RemoveAsync($"bookings_user_{booking.UserId}");

        return true;
    }
}
