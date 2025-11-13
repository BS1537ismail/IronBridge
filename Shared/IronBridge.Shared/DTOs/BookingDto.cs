using IronBridge.Shared.Enums;

namespace IronBridge.Shared.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus BookingStatus { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateBookingDto
{
    public Guid UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public class UpdateBookingStatusDto
{
    public BookingStatus BookingStatus { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
}
