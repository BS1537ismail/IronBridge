namespace Booking.Domain.Models;

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty; // Card, MobileBanking, NetBanking, etc.
    public string PaymentGateway { get; set; } = "SSLCommerz";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BDT";
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed, Cancelled
    public string? BankTransactionId { get; set; }
    public string? CardType { get; set; }
    public string? CardNo { get; set; } // Last 4 digits only
    public DateTime? PaymentDate { get; set; }
    public string? FailureReason { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual Booking? Booking { get; set; }
}
