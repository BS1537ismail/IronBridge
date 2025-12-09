using Booking.Domain.DTOs;
using Booking.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentManager _paymentManager;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentManager paymentManager, ILogger<PaymentController> logger)
    {
        _paymentManager = paymentManager;
        _logger = logger;
    }

    /// <summary>
    /// Initiate payment for a booking
    /// </summary>
    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment([FromBody] CreatePaymentDto paymentDto)
    {
        try
        {
            // Get user ID from claims (you might need to add authentication middleware)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "guest";

            var result = await _paymentManager.InitiatePaymentAsync(paymentDto, userId);

            if (result.Status == "Failed")
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message,
                    data = result
                });
            }

            return Ok(new
            {
                success = true,
                message = "Payment session created successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating payment");
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        try
        {
            var payment = await _paymentManager.GetPaymentByIdAsync(id);

            if (payment == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Payment not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = payment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment");
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get payment by transaction ID
    /// </summary>
    [HttpGet("transaction/{transactionId}")]
    public async Task<IActionResult> GetPaymentByTransactionId(string transactionId)
    {
        try
        {
            var payment = await _paymentManager.GetPaymentByTransactionIdAsync(transactionId);

            if (payment == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Payment not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = payment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment by transaction ID");
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get payment by booking ID
    /// </summary>
    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetPaymentByBookingId(int bookingId)
    {
        try
        {
            var payment = await _paymentManager.GetPaymentByBookingIdAsync(bookingId);

            if (payment == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Payment not found for this booking"
                });
            }

            return Ok(new
            {
                success = true,
                data = payment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment by booking ID");
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Success callback from SSLCommerz
    /// </summary>
    [HttpPost("success")]
    public async Task<IActionResult> PaymentSuccess([FromForm] string tran_id)
    {
        try
        {
            _logger.LogInformation($"Payment success callback received for transaction: {tran_id}");

            var payment = await _paymentManager.ValidateAndUpdatePaymentAsync(tran_id);

            if (payment == null)
            {
                return Redirect($"/payment/failed?message=Payment+not+found");
            }

            if (payment.PaymentStatus == "Paid")
            {
                return Redirect($"/payment/success?transactionId={tran_id}");
            }

            return Redirect($"/payment/failed?transactionId={tran_id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment success callback");
            return Redirect($"/payment/failed?message={ex.Message}");
        }
    }

    /// <summary>
    /// Fail callback from SSLCommerz
    /// </summary>
    [HttpPost("fail")]
    public async Task<IActionResult> PaymentFail([FromForm] string tran_id)
    {
        try
        {
            _logger.LogWarning($"Payment failed callback received for transaction: {tran_id}");

            var payment = await _paymentManager.GetPaymentByTransactionIdAsync(tran_id);

            return Redirect($"/payment/failed?transactionId={tran_id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment fail callback");
            return Redirect($"/payment/failed?message={ex.Message}");
        }
    }

    /// <summary>
    /// Cancel callback from SSLCommerz
    /// </summary>
    [HttpPost("cancel")]
    public async Task<IActionResult> PaymentCancel([FromForm] string tran_id)
    {
        try
        {
            _logger.LogInformation($"Payment cancelled by user for transaction: {tran_id}");

            var payment = await _paymentManager.GetPaymentByTransactionIdAsync(tran_id);

            return Redirect($"/payment/cancelled?transactionId={tran_id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment cancel callback");
            return Redirect($"/payment/failed?message={ex.Message}");
        }
    }

    /// <summary>
    /// IPN (Instant Payment Notification) from SSLCommerz
    /// This is the most reliable callback
    /// </summary>
    [HttpPost("ipn")]
    public async Task<IActionResult> PaymentIPN([FromForm] string tran_id, [FromForm] string status)
    {
        try
        {
            _logger.LogInformation($"IPN received for transaction: {tran_id}, status: {status}");

            if (status == "VALID" || status == "VALIDATED")
            {
                var payment = await _paymentManager.ValidateAndUpdatePaymentAsync(tran_id);

                if (payment != null && payment.PaymentStatus == "Paid")
                {
                    return Ok(new { success = true, message = "Payment validated successfully" });
                }
            }

            return Ok(new { success = false, message = "Payment validation failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing IPN");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
