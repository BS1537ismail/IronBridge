using Booking.Domain.DTOs;
using Booking.Domain.Interfaces;
using Booking.Domain.Models;
using Booking.Repository.Interfaces;
using IronBridge.Shared.Interfaces;
using System.Text.Json;

namespace Booking.Domain.Managers;

public class PaymentManager : IPaymentManager
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ISSLCommerzService _sslCommerzService;
    private readonly ICacheService _cacheService;

    public PaymentManager(
        IPaymentRepository paymentRepository,
        IBookingRepository bookingRepository,
        ISSLCommerzService sslCommerzService,
        ICacheService cacheService)
    {
        _paymentRepository = paymentRepository;
        _bookingRepository = bookingRepository;
        _sslCommerzService = sslCommerzService;
        _cacheService = cacheService;
    }

    public async Task<PaymentResponseDto> InitiatePaymentAsync(CreatePaymentDto paymentDto, string userId)
    {
        // Verify booking exists and belongs to user
        var booking = await _bookingRepository.GetByIdAsync(paymentDto.BookingId);
        if (booking == null)
        {
            throw new Exception("Booking not found");
        }

        if (booking.UserId != userId)
        {
            throw new Exception("Unauthorized access to booking");
        }

        if (booking.PaymentStatus == "Paid")
        {
            throw new Exception("Booking is already paid");
        }

        // Generate unique transaction ID
        var transactionId = $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{booking.Id}";

        // Create payment record in database
        var payment = new Repository.Models.Payment
        {
            BookingId = booking.Id,
            TransactionId = transactionId,
            Amount = paymentDto.Amount,
            Currency = paymentDto.Currency,
            PaymentStatus = "Pending",
            PaymentGateway = "SSLCommerz",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdPayment = await _paymentRepository.CreateAsync(payment);

        // Initiate SSLCommerz payment
        var sslResponse = await _sslCommerzService.InitiatePaymentAsync(paymentDto, transactionId);

        if (sslResponse.status != "SUCCESS")
        {
            // Update payment status to failed
            payment.PaymentStatus = "Failed";
            payment.FailureReason = sslResponse.failedreason;
            await _paymentRepository.UpdateAsync(payment);

            return new PaymentResponseDto
            {
                PaymentId = createdPayment.Id,
                TransactionId = transactionId,
                Status = "Failed",
                Message = sslResponse.failedreason
            };
        }

        // Clear cache for this booking
        await _cacheService.RemoveAsync($"booking_{booking.Id}");

        return new PaymentResponseDto
        {
            PaymentId = createdPayment.Id,
            TransactionId = transactionId,
            GatewayPageURL = sslResponse.GatewayPageURL,
            Status = "Success",
            Message = "Payment session created successfully"
        };
    }

    public async Task<Payment?> ValidateAndUpdatePaymentAsync(string transactionId)
    {
        // Get payment from database
        var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
        if (payment == null)
        {
            throw new Exception("Payment not found");
        }

        // Validate with SSLCommerz
        var validationResponse = await _sslCommerzService.ValidatePaymentAsync(transactionId);

        if (validationResponse.status == "VALID" || validationResponse.status == "VALIDATED")
        {
            // Update payment record
            payment.PaymentStatus = "Paid";
            payment.BankTransactionId = validationResponse.bank_tran_id;
            payment.CardType = validationResponse.card_type;
            payment.CardNo = validationResponse.card_no;
            payment.PaymentMethod = validationResponse.card_type ?? "Unknown";
            payment.PaymentDate = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);

            // Update booking payment status
            var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
            if (booking != null)
            {
                booking.PaymentStatus = "Paid";
                booking.TransactionId = transactionId;
                booking.PaymentMethod = payment.PaymentMethod;
                booking.PaymentDate = DateTime.UtcNow;
                booking.Status = "Confirmed";
                booking.UpdatedAt = DateTime.UtcNow;

                await _bookingRepository.UpdateAsync(booking);

                // Clear cache
                await _cacheService.RemoveAsync($"booking_{booking.Id}");
                await _cacheService.RemoveAsync($"user_bookings_{booking.UserId}");
            }
        }
        else
        {
            // Payment failed or invalid
            payment.PaymentStatus = "Failed";
            payment.FailureReason = validationResponse.error ?? "Payment validation failed";
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);

            // Update booking
            var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
            if (booking != null)
            {
                booking.PaymentStatus = "Failed";
                booking.UpdatedAt = DateTime.UtcNow;
                await _bookingRepository.UpdateAsync(booking);

                await _cacheService.RemoveAsync($"booking_{booking.Id}");
            }
        }

        // Convert to domain model
        return new Payment
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            TransactionId = payment.TransactionId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethod = payment.PaymentMethod,
            PaymentGateway = payment.PaymentGateway,
            BankTransactionId = payment.BankTransactionId,
            CardType = payment.CardType,
            CardNo = payment.CardNo,
            PaymentDate = payment.PaymentDate,
            FailureReason = payment.FailureReason,
            CreatedBy = payment.CreatedBy,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt,
            IsActive = payment.IsActive
        };
    }

    public async Task<Payment?> GetPaymentByIdAsync(int id)
    {
        var cacheKey = $"payment_{id}";
        var cachedPayment = await _cacheService.GetAsync<Payment>(cacheKey);

        if (cachedPayment != null)
        {
            return cachedPayment;
        }

        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null) return null;

        var domainPayment = new Payment
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            TransactionId = payment.TransactionId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethod = payment.PaymentMethod,
            PaymentGateway = payment.PaymentGateway,
            BankTransactionId = payment.BankTransactionId,
            CardType = payment.CardType,
            CardNo = payment.CardNo,
            PaymentDate = payment.PaymentDate,
            FailureReason = payment.FailureReason,
            CreatedBy = payment.CreatedBy,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt,
            IsActive = payment.IsActive
        };

        await _cacheService.SetAsync(cacheKey, domainPayment, TimeSpan.FromMinutes(10));
        return domainPayment;
    }

    public async Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId)
    {
        var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
        if (payment == null) return null;

        return new Payment
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            TransactionId = payment.TransactionId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethod = payment.PaymentMethod,
            PaymentGateway = payment.PaymentGateway,
            BankTransactionId = payment.BankTransactionId,
            CardType = payment.CardType,
            CardNo = payment.CardNo,
            PaymentDate = payment.PaymentDate,
            FailureReason = payment.FailureReason,
            CreatedBy = payment.CreatedBy,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt,
            IsActive = payment.IsActive
        };
    }

    public async Task<Payment?> GetPaymentByBookingIdAsync(int bookingId)
    {
        var payment = await _paymentRepository.GetByBookingIdAsync(bookingId);
        if (payment == null) return null;

        return new Payment
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            TransactionId = payment.TransactionId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethod = payment.PaymentMethod,
            PaymentGateway = payment.PaymentGateway,
            BankTransactionId = payment.BankTransactionId,
            CardType = payment.CardType,
            CardNo = payment.CardNo,
            PaymentDate = payment.PaymentDate,
            FailureReason = payment.FailureReason,
            CreatedBy = payment.CreatedBy,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt,
            IsActive = payment.IsActive
        };
    }
}
