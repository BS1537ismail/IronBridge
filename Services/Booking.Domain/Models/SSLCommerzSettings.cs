namespace Booking.Domain.Models;

public class SSLCommerzSettings
{
    public string StoreId { get; set; } = string.Empty;
    public string StorePassword { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string ValidationUrl { get; set; } = string.Empty;
    public bool IsSandbox { get; set; } = true;
    public string SuccessUrl { get; set; } = string.Empty;
    public string FailUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public string IpnUrl { get; set; } = string.Empty;
}
