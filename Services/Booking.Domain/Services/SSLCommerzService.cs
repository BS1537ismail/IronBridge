using Booking.Domain.DTOs;
using Booking.Domain.Interfaces;
using Booking.Domain.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Booking.Domain.Services;

public class SSLCommerzService : ISSLCommerzService
{
    private readonly HttpClient _httpClient;
    private readonly SSLCommerzSettings _settings;

    public SSLCommerzService(HttpClient httpClient, IOptions<SSLCommerzSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<SSLCommerzInitResponse> InitiatePaymentAsync(CreatePaymentDto paymentDto, string orderId)
    {
        var requestData = new Dictionary<string, string>
        {
            { "store_id", _settings.StoreId },
            { "store_passwd", _settings.StorePassword },
            { "total_amount", paymentDto.Amount.ToString("F2") },
            { "currency", paymentDto.Currency },
            { "tran_id", orderId },
            { "success_url", _settings.SuccessUrl },
            { "fail_url", _settings.FailUrl },
            { "cancel_url", _settings.CancelUrl },
            { "ipn_url", _settings.IpnUrl },
            { "cus_name", paymentDto.CustomerName },
            { "cus_email", paymentDto.CustomerEmail },
            { "cus_phone", paymentDto.CustomerPhone },
            { "cus_add1", paymentDto.CustomerAddress },
            { "cus_city", paymentDto.CustomerCity },
            { "cus_country", paymentDto.CustomerCountry },
            { "shipping_method", "NO" },
            { "product_name", "Product Purchase" },
            { "product_category", "General" },
            { "product_profile", "general" }
        };

        var content = new FormUrlEncodedContent(requestData);
        var response = await _httpClient.PostAsync(_settings.ApiUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"SSLCommerz API Error: {responseString}");
        }

        try
        {
            var result = JsonSerializer.Deserialize<SSLCommerzInitResponse>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? new SSLCommerzInitResponse { status = "FAILED", failedreason = "Invalid response from gateway" };
        }
        catch (JsonException ex)
        {
            throw new Exception($"Failed to parse SSLCommerz response. Error: {ex.Message}. Response: {responseString}");
        }
    }

    public async Task<SSLCommerzValidationResponse> ValidatePaymentAsync(string transactionId)
    {
        var validationUrl = $"{_settings.ValidationUrl}?val_id={transactionId}&store_id={_settings.StoreId}&store_passwd={_settings.StorePassword}&format=json";

        var response = await _httpClient.GetAsync(validationUrl);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"SSLCommerz Validation Error: {responseString}");
        }

        try
        {
            var result = JsonSerializer.Deserialize<SSLCommerzValidationResponse>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? new SSLCommerzValidationResponse { status = "FAILED", error = "Invalid validation response" };
        }
        catch (JsonException ex)
        {
            throw new Exception($"Failed to parse SSLCommerz validation response. Error: {ex.Message}. Response: {responseString}");
        }
    }
}
