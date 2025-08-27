using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QQBotOfficial.Dto;

namespace QQBotOfficial;

public static class PrivateMessage
{
    public static async Task Handler(string body, HttpContext httpContext)
    {
        try
        {
            var response = JsonSerializer.Deserialize<PrivateReceiveMessage>(body);
            if (response == null)
                return;

            //生成唯一ID
            var id = body.GetContextId();
            if (id == null)
                return;
            //获取用户标识
            var openId = response.Author.OpenId;
            var name = response.Author.OpenId;
            var message = response.Content;
            //如果普通命令没有处理，则交由AI
            if (!(await Commands.Handler(body, ChatType.Private)))
            {
                var result = await Models.DeepSeek.SendRequest(id, name, message);
                await Tools.SendPrivateMessage(result, openId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("私聊消息处理错误");
            Console.WriteLine(body);
            return;
        }
    }
}