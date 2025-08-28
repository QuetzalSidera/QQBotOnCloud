using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;


namespace QQBotOfficial;

public static class GetQqBotOfficial
{
    public static async Task<IResult> GetHandler(HttpContext httpContext)
    {
        var request = httpContext.Request;
        var response = httpContext.Response;

        // 1. 检查User-Agent，阻止无User-Agent或明显是爬虫的请求
        var userAgent = request.Headers.UserAgent.ToString();

        if (string.IsNullOrEmpty(userAgent) ||
            userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("crawler", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("spider", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("python", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("java", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("curl", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("wget", StringComparison.OrdinalIgnoreCase))
        {
            // 对于爬虫，返回简单响应而不是HTML
            return Results.Ok(new
            {
                status = "success",
                message = "API is running",
                timestamp = DateTime.UtcNow
            });
        }

        // 2. 检查是否是人类用户的浏览器
        bool isHumanBrowser = userAgent.Contains("mozilla", StringComparison.OrdinalIgnoreCase) ||
                              userAgent.Contains("chrome", StringComparison.OrdinalIgnoreCase) ||
                              userAgent.Contains("safari", StringComparison.OrdinalIgnoreCase) ||
                              userAgent.Contains("firefox", StringComparison.OrdinalIgnoreCase) ||
                              userAgent.Contains("edge", StringComparison.OrdinalIgnoreCase);

        // 3. 如果是浏览器请求，返回HTML内容
        if (isHumanBrowser)
        {
            try
            {
                // 异步读取HTML文件
                string htmlContent = await File.ReadAllTextAsync("readme1.html");
                // 设置响应头
                response.ContentType = "text/html; charset=utf-8";

                // 添加安全相关的响应头
                response.Headers["X-Content-Type-Options"] = "nosniff";
                response.Headers["X-Frame-Options"] = "DENY";
                response.Headers["X-XSS-Protection"] = "1; mode=block";

                // 直接写入响应
                await response.WriteAsync(htmlContent);

                // 返回空结果，因为我们已经手动写了响应
                return Results.Empty;
            }
            catch (FileNotFoundException)
            {
                return Results.NotFound("HTML file not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading HTML file: {ex.Message}");
                return Results.Problem("Internal server error");
            }
        }

        // 4. 对于其他类型的请求，返回简单的API响应
        return Results.Ok(new
        {
            status = "success",
            message = "QQ Bot API is running",
            version = "1.0.0",
            timestamp = DateTime.UtcNow
        });
    }

    public static async Task<IResult> GetImgHandler(HttpContext httpContext)
    {
        var request = httpContext.Request;
        var response = httpContext.Response;

        // 1. 检查User-Agent，阻止无User-Agent或明显是爬虫的请求
        var userAgent = request.Headers.UserAgent.ToString();

        if (string.IsNullOrEmpty(userAgent) ||
            userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("crawler", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("spider", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("python", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("java", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("curl", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("wget", StringComparison.OrdinalIgnoreCase))
        {
            // 对于爬虫，返回简单响应而不是HTML
            return Results.Ok(new
            {
                status = "success",
                message = "API is running",
                timestamp = DateTime.UtcNow
            });
        }

        // 2. 检查是否是人类用户的浏览器
        bool isHumanBrowser = userAgent.Contains("mozilla", StringComparison.OrdinalIgnoreCase) ||
                              userAgent.Contains("chrome", StringComparison.OrdinalIgnoreCase) ||
                              userAgent.Contains("safari", StringComparison.OrdinalIgnoreCase) ||
                              userAgent.Contains("firefox", StringComparison.OrdinalIgnoreCase) ||
                              userAgent.Contains("edge", StringComparison.OrdinalIgnoreCase);

        if (isHumanBrowser)
        {
            try
            {
                // 直接写入响应
                await response.SendFileAsync("和栗薰子.jepg");

                // 返回空结果，因为我们已经手动写了响应
                return Results.Empty;
            }
            catch (FileNotFoundException)
            {
                return Results.NotFound("HTML file not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading HTML file: {ex.Message}");
                return Results.Problem("Internal server error");
            }
        }

        // 4. 对于其他类型的请求，返回简单的API响应
        return Results.Ok(new
        {
            status = "success",
            message = "QQ Bot API is running",
            version = "1.0.0",
            timestamp = DateTime.UtcNow
        });
    }
}