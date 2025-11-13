using IronBridge.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Booking.Service.Data;

namespace Booking.Service.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _context;
    private readonly IProductHttpClient _productClient;

    public BookingService(BookingDbContext context, IProductHttpClient productClient)
    {
        _context = context;
        _productClient = productClient;
    }

    public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto)
    {
        // Get product details from Product Service
        var product = await _productClient.GetProductByIdAsync(dto.ProductId);

        if (product == null || !product.IsActive || product.Stock < dto.Quantity)
            return null;

        // Calculate total amount
        var totalAmount = product.Price * dto.Quantity;

        var booking = new Models.Booking
        {
            UserId = dto.UserId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            TotalAmount = totalAmount,
            PaymentMethod = dto.PaymentMethod
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Update product stock
        await _productClient.UpdateStockAsync(dto.ProductId, dto.Quantity);

        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            ProductId = booking.ProductId,
            Quantity = booking.Quantity,
            TotalAmount = booking.TotalAmount,
            BookingStatus = booking.BookingStatus,
            PaymentStatus = booking.PaymentStatus,
            TransactionId = booking.TransactionId,
            PaymentMethod = booking.PaymentMethod,
            BookingDate = booking.BookingDate,
            UpdatedAt = booking.UpdatedAt
        };
    }

    public async Task<BookingDto?> GetBookingByIdAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return null;

        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            ProductId = booking.ProductId,
            Quantity = booking.Quantity,
            TotalAmount = booking.TotalAmount,
            BookingStatus = booking.BookingStatus,
            PaymentStatus = booking.PaymentStatus,
            TransactionId = booking.TransactionId,
            PaymentMethod = booking.PaymentMethod,
            BookingDate = booking.BookingDate,
            UpdatedAt = booking.UpdatedAt
        };
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(Guid userId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

        return bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            UserId = b.UserId,
            ProductId = b.ProductId,
            Quantity = b.Quantity,
            TotalAmount = b.TotalAmount,
            BookingStatus = b.BookingStatus,
            PaymentStatus = b.PaymentStatus,
            TransactionId = b.TransactionId,
            PaymentMethod = b.PaymentMethod,
            BookingDate = b.BookingDate,
            UpdatedAt = b.UpdatedAt
        });
    }

    public async Task<BookingDto?> UpdateBookingStatusAsync(int id, UpdateBookingStatusDto dto)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return null;

        booking.BookingStatus = dto.BookingStatus;
        booking.PaymentStatus = dto.PaymentStatus;
        booking.TransactionId = dto.TransactionId;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            ProductId = booking.ProductId,
            Quantity = booking.Quantity,
            TotalAmount = booking.TotalAmount,
            BookingStatus = booking.BookingStatus,
            PaymentStatus = booking.PaymentStatus,
            TransactionId = booking.TransactionId,
            PaymentMethod = booking.PaymentMethod,
            BookingDate = booking.BookingDate,
            UpdatedAt = booking.UpdatedAt
        };
    }
}
