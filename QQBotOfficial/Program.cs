using System.Net;
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
        app.MapPost("/qqbotofficial/api", async (HttpListenerContext context) => await PostQqBotOfficial.PostHandler(context));
        app.MapGet("/qqbotofficial", async (HttpListenerContext context) => await GetQqBotOfficial.GetHandler(context));

        app.Run();
    }
}