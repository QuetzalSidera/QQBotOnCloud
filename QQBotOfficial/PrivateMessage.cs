using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QQBotOfficial.Dto;

namespace QQBotOfficial;

public static class PrivateMessage
{
    private static string _timestamp = string.Empty;

    public static async Task Handler(string body, HttpContext httpContext)
    {
        try
        {
            Console.WriteLine("in PrivateMessage Handler 1");
            var response = JsonSerializer.Deserialize<EventPayload<PrivateReceiveMessage>>(body);
            Console.WriteLine("in PrivateMessage Handler 2");
            if (response == null)
                return;
            Console.WriteLine("in PrivateMessage Handler 3");
            //生成唯一ID
            var id = body.GetContextId();
            if (id == null)
                return;
            Console.WriteLine("in PrivateMessage Handler 4");
            //获取用户标识
            var openId = response.Data.Author.OpenId;
            var name = response.Data.Author.OpenId;
            var message = response.Data.Content;
            var msgId = response.Data.Id;
            var eventId = response.EventType;
            var oldTimestamp = _timestamp;
            _timestamp = response.Data.Timestamp;
            if (oldTimestamp == _timestamp)
                return;
            // var eventId = "C2C_MSG_RECEIVE";
            Console.WriteLine(eventId);
            Console.WriteLine(msgId);
            Console.WriteLine("in PrivateMessage Handler 5");
            //如果普通命令没有处理，则交由AI
            bool isDebugCommand = await Commands.DebugHandler(body, ChatType.Private, msgId);
            bool isMessageCommand = await Commands.MessageHandler(openId, message, ChatType.Private, msgId);
            if(message.Trim().StartsWith("@"+Config.Name))
                message=message[(Config.Name.Length+1)..];
            bool isChatCommand = message.Trim().StartsWith(Config.ChatCommand);
            
            if (isChatCommand)
                message = message.Remove(0, Config.ChatCommand.Length);
            if (isChatCommand || !(isMessageCommand || isDebugCommand))
            {
                Console.WriteLine("in PrivateMessage Handler 6");
                var result = await Models.DeepSeek.SendRequest(id, name, message);
                await Tools.SendPrivateMessage(result, openId, msgId);
            }

            Console.WriteLine("in PrivateMessage Handler 7");
        }
        catch (Exception ex)
        {
            Console.WriteLine("私聊消息处理错误");
            Console.WriteLine(body);

            return;
        }
    }
}