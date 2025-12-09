namespace Booking.Domain.DTOs;

public class PaymentResponseDto
{
    public int PaymentId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string GatewayPageURL { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
