using MauiHybridApp.Models;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text.Json;

namespace MauiHybridApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> GetAsync<T>(string endPoint)
    {
        string requestApiUrl = string.Concat(_httpClient.BaseAddress,endPoint);
        var response = await _httpClient.GetAsync(requestApiUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var result = JsonSerializer.Deserialize<T>(content, _jsonSerializerOptions);
            return result;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing content: {ex.Message}");
            throw;
        }
    }

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
