using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;


namespace QQBotOfficial;

public static class GetQqBotOfficial
{
    public static async Task<IResult> GetHandler(HttpContext httpContext)
    {
        var request = httpContext.Request;
        // 读取请求体
        using var reader = new StreamReader(request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        Console.WriteLine("收到 WebHook 请求：");
        Console.WriteLine(body);

        try
        {
            // var json = JsonSerializer.Deserialize<JsonElement>(body);
            // Console.WriteLine("解析后的 JSON： " + json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("JSON 解析失败: " + ex.Message);
            return Results.BadRequest(new { error = "JSON解析失败", message = ex.Message });
        }

        // 直接返回IResult，不要手动操作Response
        return Results.Ok(new
        {
            status = "success",
            message = "访问成功"
        });
    }
}