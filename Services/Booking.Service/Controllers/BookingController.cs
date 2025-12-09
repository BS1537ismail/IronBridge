using IronBridge.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Booking.Domain.Interfaces;
using Booking.Domain.DTOs;

namespace Booking.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingManager _bookingManager;
    private readonly IPaymentManager _paymentManager;
    private readonly ILogger<BookingController> _logger;

    public BookingController(
        IBookingManager bookingManager,
        IPaymentManager paymentManager,
        ILogger<BookingController> logger)
    {
        _bookingManager = bookingManager;
        _paymentManager = paymentManager;
        _logger = logger;
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
        try
        {
            // Step 1: Create the booking
            var booking = await _bookingManager.CreateBookingAsync(dto);

            if (booking == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to create booking"
                });
            }

            // Step 2: Automatically initiate payment
            var paymentDto = new CreatePaymentDto
            {
                BookingId = booking.Id,
                Amount = dto.TotalAmount,
                Currency = "BDT",
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                CustomerPhone = dto.CustomerPhone,
                CustomerAddress = dto.CustomerAddress,
                CustomerCity = dto.CustomerCity,
                CustomerCountry = dto.CustomerCountry
            };

            var paymentResult = await _paymentManager.InitiatePaymentAsync(paymentDto, dto.UserId);

            if (paymentResult.Status == "Failed")
            {
                _logger.LogWarning($"Payment initiation failed for booking {booking.Id}: {paymentResult.Message}");

                return Ok(new
                {
                    success = true,
                    message = "Booking created but payment initiation failed",
                    data = new
                    {
                        booking = booking,
                        payment = new
                        {
                            status = "Failed",
                            message = paymentResult.Message
                        }
                    }
                });
            }

            // Step 3: Return booking with payment URL
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, new
            {
                success = true,
                message = "Booking created successfully. Please proceed to payment.",
                data = new
                {
                    booking = booking,
                    payment = new
                    {
                        transactionId = paymentResult.TransactionId,
                        paymentUrl = paymentResult.GatewayPageURL,
                        status = paymentResult.Status,
                        message = "Redirect user to paymentUrl to complete payment"
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking and initiating payment");
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
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
