using IronBridge.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Booking.Domain.Interfaces;

namespace Booking.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingManager _bookingManager;

    public BookingController(IBookingManager bookingManager)
    {
        _bookingManager = bookingManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _bookingManager.GetAllBookingsAsync();
        return Ok(bookings);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveBookings()
    {
        var bookings = await _bookingManager.GetActiveBookingsAsync();
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var booking = await _bookingManager.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound(new { message = "Booking not found" });
        return Ok(booking);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBookingsByUserId(string userId)
    {
        var bookings = await _bookingManager.GetBookingsByUserIdAsync(userId);
        return Ok(bookings);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetBookingsByStatus(string status)
    {
        var bookings = await _bookingManager.GetBookingsByStatusAsync(status);
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var booking = await _bookingManager.CreateBookingAsync(dto);
        return CreatedAtAction(nameof(GetBookingById), new { id = booking!.Id }, booking);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto dto)
    {
        var booking = await _bookingManager.UpdateBookingAsync(id, dto);
        if (booking == null)
            return NotFound(new { message = "Booking not found" });
        return Ok(booking);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var result = await _bookingManager.DeleteBookingAsync(id);
        if (!result)
            return NotFound(new { message = "Booking not found" });
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateBookingStatus(int id, [FromQuery] string status)
    {
        var result = await _bookingManager.UpdateBookingStatusAsync(id, status);
        if (!result)
            return BadRequest(new { message = "Booking not found" });
        return NoContent();
    }
}
