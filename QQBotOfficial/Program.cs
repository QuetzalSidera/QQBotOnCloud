using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using QQBotOfficial;

public class Program
{
    public static async Task Main(string[] args)
    {
        _ = Task.Run(async () =>
        {
            while (await TokenManager.Timer.WaitForNextTickAsync())
            {
                await TokenManager.GetAccessToken();
            }
        });

        await TokenManager.GetAccessToken();
        Console.WriteLine(TokenManager.AccessToken);
        var builder = WebApplication.CreateBuilder(args);


        // 配置 Kestrel 监听 8443 端口，启用 HTTPS
        builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(8443, listenOptions => { }); });

        var app = builder.Build();

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "img")),
            RequestPath = "/img"
        });
        // WebHook 接收端点 - 使用正确的ASP.NT Core方式
        app.MapPost("/qqbotofficial/api", async (HttpContext context) => await PostQqBotOfficial.PostHandler(context));
        app.MapGet("/", async (HttpContext context) => await GetQqBotOfficial.GetHandler(context));
        app.Run();
    }
}