namespace Booking.Domain.DTOs;

public class PaymentValidationDto
{
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? BankTransactionId { get; set; }
    public string? CardType { get; set; }
    public string? CardNo { get; set; }
    public string? FailureReason { get; set; }
}
