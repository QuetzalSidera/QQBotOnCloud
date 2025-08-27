using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using QQBotOfficial;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 配置 Kestrel 监听 8443 端口，启用 HTTPS
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(8443, listenOptions =>
            {
            });
        });
        
        var app = builder.Build();
        
        // WebHook 接收端点 - 使用正确的ASP.NET Core方式
        app.MapPost("/qqbotofficial/api", async (HttpContext context) => await PostQqBotOfficial.PostHandler(context));
        app.MapGet("/qqbotofficial", async (HttpContext context) => await GetQqBotOfficial.GetHandler(context));
        
        app.Run();
    }
}