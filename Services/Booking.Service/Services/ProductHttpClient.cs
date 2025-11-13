using IronBridge.Shared.DTOs;
using System.Text.Json;

namespace Booking.Service.Services;

public class ProductHttpClient : IProductHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ProductHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["Services:ProductService"]!);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Product/{productId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        try
        {
            var response = await _httpClient.PatchAsync($"api/Product/{productId}/stock?quantity={quantity}", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
