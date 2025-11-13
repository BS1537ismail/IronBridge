using IronBridge.Shared.DTOs;
using System.Text;
using System.Text.Json;

namespace Admin.Service.Services;

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

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Product");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<ProductDto>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<ProductDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? Enumerable.Empty<ProductDto>();
        }
        catch
        {
            return Enumerable.Empty<ProductDto>();
        }
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

    public async Task<ProductDto?> CreateProductAsync(CreateProductDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Product", httpContent);

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

    public async Task<ProductDto?> UpdateProductAsync(int productId, UpdateProductDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/Product/{productId}", httpContent);

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

    public async Task<bool> DeleteProductAsync(int productId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Product/{productId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
