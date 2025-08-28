using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using QQBotOfficial.Dto;

namespace QQBotOfficial;

public static class GroupMessage
{
    private static string _timestampCreate = string.Empty;

    public static async Task Handler(string body, HttpContext httpContext)
    {
        try
        {
            Console.WriteLine("in GroupAtMessageHandler A1");
            var response = JsonSerializer.Deserialize<EventPayload<GroupAtMessageCreate>>(body);
            if (response == null)
                return;
            Console.WriteLine("in GroupAtMessageHandler A2");
            //生成唯一ID
            var id = body.GetContextId();
            if (id == null)
                return;
            Console.WriteLine("in GroupAtMessageHandler A3");
            //获取用户标识
            var openId = response.Data.GroupOpenId;
            var name = response.Data.Author.MemberOpenId;
            var message = response.Data.Content;
            var msgId = response.EventId;

            var oldTimestamp = _timestampCreate;
            _timestampCreate = response.Data.Timestamp;
            if (oldTimestamp == _timestampCreate)
                return;
            //处理缓存
            Console.WriteLine("in GroupAtMessageHandler A4");
            // var eventId = "GROUP_MSG_CREATE";
            //如果普通命令没有处理，则交由AI
            bool isDebugCommand = await Commands.DebugHandler(body, ChatType.Group, msgId);
            bool isMessageCommand = await Commands.MessageHandler(openId, message, ChatType.Group, msgId);
            bool isChatCommand = message.Trim().StartsWith(Config.ChatCommand);
            if (isChatCommand)
                message = message.Remove(0, Config.ChatCommand.Length);
            if (isChatCommand || !(isMessageCommand || isDebugCommand))
            {
                Console.WriteLine("in GroupAtMessageHandler A5");
                var result = await Models.DeepSeek.SendRequest(id, name, message);
                await Tools.SendGroupMessage(result, openId, msgId);
            }

            Console.WriteLine("in GroupAtMessageHandler A6");
        }

        catch (Exception ex)
        {
            Console.WriteLine("群聊消息处理错误A");
            Console.WriteLine(body);
            return;
        }
    }

    private static long _timestampReceive = 0;

    public static async Task MessageReceiveHandler(string body, HttpContext httpContext)
    {
        try
        {
            Console.WriteLine("in GroupAtMessageHandler B1");
            var response = JsonSerializer.Deserialize<EventPayload<GroupMessageReceive>>(body);
            if (response == null)
                return;
            Console.WriteLine("in GroupAtMessageHandler B2");
            //生成唯一ID
            var id = body.GetContextId();
            if (id == null)
                return;
            Console.WriteLine("in GroupAtMessageHandler B3");
            //获取用户标识
            var openId = response.Data.GroupOpenId;
            var name = response.Data.OpMemberOpenId;
            string message = "你好";
            var msgId = response.EventId;

            var oldTimestamp = _timestampReceive;
            _timestampReceive = response.Data.Timestamp;
            if (oldTimestamp == _timestampReceive)
                return;
            //处理缓存
            Console.WriteLine("in GroupAtMessageHandler B4");
            // var eventId = "GROUP_MSG_RECEIVE";
            //如果普通命令没有处理，则交由AI
            bool isDebugCommand = await Commands.DebugHandler(body, ChatType.Group, msgId);
            bool isMessageCommand = await Commands.MessageHandler(openId, message, ChatType.Group, msgId);
            if(message.Trim().StartsWith("@"+Config.Name))
                message=message[(Config.Name.Length+1)..];
            bool isChatCommand = message.Trim().StartsWith(Config.ChatCommand);
            if (isChatCommand)
                message = message.Remove(0, Config.ChatCommand.Length);
            if (isChatCommand || !(isMessageCommand || isDebugCommand))
            {
                Console.WriteLine("in GroupAtMessageHandler B5");
                var result = await Models.DeepSeek.SendRequest(id, name, message);
                await Tools.SendGroupMessage(result, openId, msgId);
            }

            Console.WriteLine("in GroupAtMessageHandler B6");
        }

        catch (Exception ex)
        {
            Console.WriteLine("群聊消息处理错误B");
            Console.WriteLine(body);
            return;
        }
    }
}