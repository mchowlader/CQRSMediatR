﻿using MauiHybridApp.Components.Pages;
using System.Text;
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
        string requestApiUrl = string.Concat(_httpClient.BaseAddress, endPoint);
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

    public async Task<string> CreateAsync<T>(string endPoint, T data)
    {
        string requestApiUrl = string.Concat(_httpClient.BaseAddress, endPoint);
        var jsonContent = JsonSerializer.Serialize(data);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(requestApiUrl, content);
        response.EnsureSuccessStatusCode();
        var response2 = await response.Content.ReadAsStringAsync();
        return await response.Content.ReadAsStringAsync();
    }

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
