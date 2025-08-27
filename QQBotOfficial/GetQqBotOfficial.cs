using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace QQBotOfficial;

public static class GetQqBotOfficial
{
    public static async Task<IResult> GetHandler(HttpListenerContext httpContext)
    {
        var request = httpContext.Request;
        var response = httpContext.Response;

        using var reader = new StreamReader(httpContext.Request.InputStream);
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
        }

        await httpContext.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("访问成功"));
        httpContext.Response.Close();
        return Results.Ok(new { status = "success" });
    }
}