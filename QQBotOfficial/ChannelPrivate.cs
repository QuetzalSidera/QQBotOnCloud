using System.Text.Json;
using Microsoft.AspNetCore.Http;
using QQBotOfficial.Dto;

namespace QQBotOfficial;

public class ChannelPrivate
{
    private static string _timestamp = string.Empty;

    public static async Task Handler(string body, HttpContext httpContext)
    {
        try
        {
            Console.WriteLine("in ChannelPrivate Handler 1");
            var response = JsonSerializer.Deserialize<EventPayload<ChannelPrivateReceive>>(body);
            if (response == null)
                return;
            Console.WriteLine("in ChannelPrivate Handler 2");
            //生成唯一ID
            var id = body.GetContextId();
            if (id == null)
                return;
            Console.WriteLine("in ChannelPrivate Handler 3");
            var guildId = response.Data.GuildId;
            var message = response.Data.Content;
            var name = response.Data.Author.Username;
            var msgId = response.EventId;
            var oldTimestamp = _timestamp;
            _timestamp = response.Data.Timestamp;
            if (oldTimestamp == _timestamp)
                return;
            // var eventId = "GROUP_MSG_RECEIVE";
            //如果普通命令没有处理，则交由AI
            Console.WriteLine("in ChannelPrivate Handler 4");
            bool isDebugCommand = await Commands.DebugHandler(body, ChatType.ChannelPrivate, msgId);
            bool isMessageCommand = await Commands.MessageHandler(guildId, message, ChatType.ChannelPrivate, msgId);
            Console.WriteLine("in ChannelPrivate Handler 5");
            if (message.Trim().StartsWith("@" + Config.Name))
                message = message[(Config.Name.Length + 1)..];
            bool isChatCommand = message.Trim().StartsWith(Config.ChatCommand);
            if (isChatCommand)
                message = message.Remove(0, Config.ChatCommand.Length);
            if (isChatCommand || !(isMessageCommand || isDebugCommand))
            {
                Console.WriteLine("in ChannelPrivate Handler 6");
                var result = await Models.DeepSeek.SendRequest(id, name, message);
                await Tools.SendChannelPrivateMessage(result, guildId, msgId);
            }

            Console.WriteLine("in ChannelPrivate Handler 7");
        }
        catch (Exception ex)
        {
            Console.WriteLine("频道私聊消息处理错误");
            Console.WriteLine(body);
            return;
        }
    }
}