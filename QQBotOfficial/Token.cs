using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace QQBotOfficial;

/// <summary>
/// Token相关管理类
/// </summary>
public class TokenManager
{
    public static string AccessToken = string.Empty;
    public const string BotQqId = "3889831055";
    public const string AppId = "102805649";
    public const string BotToken = "YMyBoWQHB5qxoSUkjD8DndZBrHoQq3JV";
    public const string BotSecret = "GoMvU3cBkJtT3dDnNyZAlMxYAmO0cEqS";

    public static readonly PeriodicTimer Timer = new PeriodicTimer(TimeSpan.FromSeconds(6000));

    public static void AddAuthHeader(HttpRequestMessage request)
    {
        request.Headers.Add("Authorization", "QQBot " + AccessToken);
    }

    public static void AddAuthHeader(IHeaderDictionary header)
    {
        header.Append("Authorization", "QQBot " + AccessToken);
    }


    private JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    public static async Task GetAccessToken()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Config.AccessTokenBaseUrl);

            string json = JsonSerializer.Serialize(new AccessTokenParam(AppId, BotSecret));

            Console.WriteLine(json);
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = jsonContent; // 将带有正确头部的Content赋值给request
            var rawResponse = await HttpClientService.Client.SendAsync(request);
            Console.WriteLine(rawResponse.Content.ReadAsStringAsync().Result);
            var response = JsonSerializer.Deserialize<AccessTokenDto>(await rawResponse.Content.ReadAsStringAsync());
            AccessToken = response?.AccessToken ?? string.Empty;
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}

public class AccessTokenDto(string accessToken, string expiresIn)
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; } = accessToken;
    [JsonPropertyName("expires_in")] public string ExpiresIn { get; set; } = expiresIn;
}

public class AccessTokenParam(string appId, string clientSecret)
{
    [JsonPropertyName("appId")] public string AppId { get; set; } = appId;
    [JsonPropertyName("clientSecret")] public string ClientSecret { get; set; } = clientSecret;
}