using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace QQBotOfficial;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// 配置 Kestrel 监听 8443 端口，启用 HTTPS
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(8443, listenOptions =>
            {
                listenOptions.UseHttps("development.pfx", "Qs@81920"); // 替换成你的证书文件和密码
            });
        });

        var app = builder.Build();

// WebHook 接收端点
        app.MapPost("/qqbotofficial/api", async (HttpContext context) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            Console.WriteLine("收到 WebHook 请求：");
            Console.WriteLine(body);

            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(body);
                Console.WriteLine("解析后的 JSON： " + json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("JSON 解析失败: " + ex.Message);
            }

            await context.Response.WriteAsync("访问成功");
            return Results.Ok(new { status = "success" });
        });

        app.Run();
    }
}