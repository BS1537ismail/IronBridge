using IronBridge.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Booking.Service.Services;

namespace Booking.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var booking = await _bookingService.CreateBookingAsync(dto);

        if (booking == null)
            return BadRequest(new { message = "Product not available or insufficient stock" });

        return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetBookingById(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);

        if (booking == null)
            return NotFound(new { message = "Booking not found" });

        return Ok(booking);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByUserId(Guid userId)
    {
        var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
        return Ok(bookings);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<BookingDto>> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusDto dto)
    {
        var booking = await _bookingService.UpdateBookingStatusAsync(id, dto);

        if (booking == null)
            return NotFound(new { message = "Booking not found" });

        return Ok(booking);
    }
}
