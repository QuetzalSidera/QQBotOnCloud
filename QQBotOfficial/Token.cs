using System.Text.Json;
using System.Text.Json.Serialization;

namespace QQBotOfficial;

/// <summary>
/// Token相关管理类
/// </summary>
public class Token
{
    public string AccessToken = string.Empty;
    public const string BotQqId = "3889831055";
    public const string AppId = "102805649";
    public const string BotToken = "YMyBoWQHB5qxoSUkjD8DndZBrHoQq3JV";
    public const string BotSecret = "GoMvU3cBkJtT3dDnNyZAlMxYAmO0cEqS";


    public void AddAuthHeader(HttpRequestMessage request)
    {
        request.Headers.Add("Authorization", "QQBot " + AccessToken);
    }

    private JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    public async Task GetAccessToken()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://bots.qq.com/app/getAppAccessToken");

            string json = JsonSerializer.Serialize(new AccessTokenParam(AppId, BotSecret));

            Console.WriteLine(json);
            request.Headers.Add("Content-Type", "application/json");
            request.Content = new StringContent(json);
            var rawResponse = await HttpClientService.Client.SendAsync(request);
            Console.WriteLine(rawResponse.Content.ReadAsStringAsync().Result);
            var response = JsonSerializer.Deserialize<AccessTokenDto>(await rawResponse.Content.ReadAsStringAsync());
            AccessToken = response?.AccessToken ?? string.Empty;
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