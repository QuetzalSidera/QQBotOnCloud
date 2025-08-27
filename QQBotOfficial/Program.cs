using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using QQBotOfficial;

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
                // listenOptions.UseHttps(GetCertificate());
                // 使用开发证书（ASP.NET Core自动生成）

                // 使用开发证书（需要安装.NET Core开发证书）
                listenOptions.UseHttps(httpsOptions =>
                {
                    // 使用开发证书
                });
            });
        });

        var app = builder.Build();

        // WebHook 接收端点 - 使用正确的ASP.NET Core方式
        app.MapPost("/qqbotofficial/api", async (HttpContext context) => await PostQqBotOfficial.PostHandler(context));
        app.MapGet("/qqbotofficial", async (HttpContext context) => await GetQqBotOfficial.GetHandler(context));

        app.Run();
    }

    private static X509Certificate2 GetDevelopmentCertificate()
    {
        // 尝试获取开发证书
        var cert = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "localhost.pfx"), "password");

        // 或者使用证书存储中的开发证书
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);
        var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, "localhost", true);

        return certificates.Count > 0 ? certificates[0] : null;
    }
}