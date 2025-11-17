namespace IronBridge.Shared.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public DateTime BookingDate { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateBookingDto
{
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public DateTime BookingDate { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public string CreatedBy { get; set; } = string.Empty;
}

public class UpdateBookingDto
{
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public DateTime BookingDate { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
