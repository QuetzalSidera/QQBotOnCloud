using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;
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
                // // 方法1：使用配置方式而不是直接UseHttps()
                // listenOptions.UseHttps(httpsOptions =>
                // {
                //     // httpsOptions.ServerCertificate = LoadCertificate();
                //     httpsOptions.ClientCertificateMode = ClientCertificateMode.NoCertificate;
                //     httpsOptions.CheckCertificateRevocation = false;
                // });
            });
        });

        var app = builder.Build();

        // WebHook 接收端点 - 使用正确的ASP.NET Core方式
        app.MapPost("/qqbotofficial/api", async (HttpContext context) => await PostQqBotOfficial.PostHandler(context));
        app.MapGet("/qqbotofficial", async (HttpContext context) => await GetQqBotOfficial.GetHandler(context));

        app.Run();
    }

    private static X509Certificate2 LoadCertificate()
    {
        try
        {
            // 尝试从证书存储获取开发证书
            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
        
            var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, "localhost", true);
            if (certificates.Count > 0)
            {
                return certificates[0];
            }

            // 如果证书存储中没有，尝试从文件加载
            var certPath = Path.Combine(Directory.GetCurrentDirectory(), "quetzalsidera.pfx");
            if (File.Exists(certPath))
            {
                return new X509Certificate2(certPath, "Qs@81920", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
            }

            Console.WriteLine("未找到证书，将尝试使用默认证书");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"证书加载失败: {ex.Message}");
            return null;
        }
    }
}