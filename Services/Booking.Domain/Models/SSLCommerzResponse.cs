namespace Booking.Domain.Models;

public class SSLCommerzInitResponse
{
    public string status { get; set; } = string.Empty;
    public string failedreason { get; set; } = string.Empty;
    public string sessionkey { get; set; } = string.Empty;
    public string GatewayPageURL { get; set; } = string.Empty;
    public string storeBanner { get; set; } = string.Empty;
    public string storeLogo { get; set; } = string.Empty;
    public string desc { get; set; } = string.Empty;
    public string is_direct_pay_enable { get; set; } = string.Empty;
}

public class SSLCommerzValidationResponse
{
    public string status { get; set; } = string.Empty;
    public string tran_date { get; set; } = string.Empty;
    public string tran_id { get; set; } = string.Empty;
    public string val_id { get; set; } = string.Empty;
    public string amount { get; set; } = string.Empty;
    public string store_amount { get; set; } = string.Empty;
    public string currency { get; set; } = string.Empty;
    public string bank_tran_id { get; set; } = string.Empty;
    public string card_type { get; set; } = string.Empty;
    public string card_no { get; set; } = string.Empty;
    public string card_issuer { get; set; } = string.Empty;
    public string card_brand { get; set; } = string.Empty;
    public string card_issuer_country { get; set; } = string.Empty;
    public string card_issuer_country_code { get; set; } = string.Empty;
    public string currency_type { get; set; } = string.Empty;
    public string currency_amount { get; set; } = string.Empty;
    public string currency_rate { get; set; } = string.Empty;
    public string verify_sign { get; set; } = string.Empty;
    public string verify_key { get; set; } = string.Empty;
    public string risk_title { get; set; } = string.Empty;
    public string risk_level { get; set; } = string.Empty;
    public string error { get; set; } = string.Empty;
}
