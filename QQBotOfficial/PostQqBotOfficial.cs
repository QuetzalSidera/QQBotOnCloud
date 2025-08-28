using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math.EC.Rfc8032;
using Org.BouncyCastle.Security;
using QQBotOfficial.Dto;

namespace QQBotOfficial;

public class PostQqBotOfficial
{
    public static async Task<IResult> PostHandler(HttpContext httpContext)
    {
        var request = httpContext.Request;

        // 读取请求体
        using var reader = new StreamReader(request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        Console.WriteLine("收到 WebHook 请求：");
        Console.WriteLine(body);

        try
        {
            await PostHandler(body, httpContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine("POST消息解析失败: " + ex.Message);
        }

        // 直接返回IResult，不要手动操作Response
        return Results.Ok(new
        {
            status = "success",
            message = "访问成功"
        });
    }

    public static async Task PostHandler(string body, HttpContext httpContext)
    {
        Console.WriteLine("in PostHandler 1");
        var response = JsonSerializer.Deserialize<EventPayload>(body);
        if (response == null)
            return;
        Console.WriteLine("in PostHandler 2");
        switch (OperationCodeEnumHelper.ToOperationCodeEnum(response.OpCode))
        {
            case OperationCodeEnum.Dispatch:
                await DispatchHandler(body, httpContext);
                break;
            case OperationCodeEnum.CallbackAak:
                await CallbackAakHandler(body, httpContext);
                break;
            case OperationCodeEnum.CallbackValidation:
                await CallbackValidationHandler(body, httpContext);
                break;
            default:
                Console.WriteLine($"未知的事件类型: {response.OpCode}");
                break;
        }
    }


    public static async Task DispatchHandler(string body, HttpContext httpContext)
    {
        var response = JsonSerializer.Deserialize<EventPayload>(body);
        if (response == null)
            return;
        switch (EventTypeEnumHelper.ToEventTypeEnum(response.EventType))
        {
            case EventTypeEnum.GatewayEventName:
                await GatewayEventName.Handler(body, httpContext);
                break;
            case EventTypeEnum.GroupAtMessageReceive:
                await GroupMessage.Handler(body, httpContext);
                break;
            case EventTypeEnum.C2CMessageReceive:
                await PrivateMessage.Handler(body, httpContext);
                break;
            default:
                Console.WriteLine($"不支持的事件类型: {response.EventType}");
                break;
        }
    }

    public static async Task CallbackAakHandler(string body, HttpContext httpContext)
    {
    }

    /// <summary>
    /// 回调Url验证
    /// </summary>
    /// <param name="body"></param>
    /// <param name="httpContext"></param>
    public static async Task CallbackValidationHandler(string body, HttpContext httpContext)
    {
        Console.WriteLine("in CallbackValidationHandler 1");
        var response = JsonSerializer.Deserialize<EventPayload<CallbackValidationEvent>>(body);
        if (response == null)
            return;
        Console.WriteLine("in CallbackValidationHandler 2");
        string eventTs = response.Data.EventTs;
        string plainToken = response.Data.PlainToken;


        string signature = GoEd25519Compatible.GenerateSignature(
            plainToken: plainToken,
            botSecret: TokenManager.BotSecret,
            eventTs: eventTs
        );

        var result = new CallbackValidationEventRet
        {
            PlainToken = plainToken,
            Signature = signature
        };
        Console.WriteLine("in CallbackValidationHandler 3");
        Console.WriteLine(
            $"BotSecret:{TokenManager.BotSecret}, EventTs:{eventTs}, PlainToken:{plainToken}, Signature:{signature}");
        var json = JsonSerializer.Serialize(result);
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = 200;
        Console.WriteLine("in CallbackValidationHandler 4");
        await httpContext.Response.WriteAsync(json);
    }
}