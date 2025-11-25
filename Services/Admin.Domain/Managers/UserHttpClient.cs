using Admin.Domain.Interfaces;
using IronBridge.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Admin.Service.HttpClients;

public class UserHttpClient : IUserHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public UserHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["Services:UserAuthService"]!);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Auth/user/{userId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return null;
        }
    }
}
