namespace Booking.Domain.Models;

public class Booking
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";

    // Payment related fields
    public string? TransactionId { get; set; }
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed
    public string? PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }

    // Shipping details
    public string? ShippingAddress { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingPhone { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual ICollection<Payment>? Payments { get; set; }
}
