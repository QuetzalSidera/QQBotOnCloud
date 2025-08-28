using System.Text.Json;
using Microsoft.AspNetCore.Http;
using QQBotOfficial.Dto;

namespace QQBotOfficial;

public static class GroupMessage
{
    public static async Task Handler(string body, HttpContext httpContext)
    {
        try
        {
            var response = JsonSerializer.Deserialize<EventPayload<GroupReceiveMessage>>(body);
            if (response == null)
                return;
            //生成唯一ID
            var id = body.GetContextId();
            if (id == null)
                return;
            //获取用户标识
            var openId = response.Data.GroupOpenId;
            var name = response.Data.Author.MemberOpenId;
            var message = response.Data.Content;
            var msgId = response.Data.Id;
            var eventId= response.EventType;
            //如果普通命令没有处理，则交由AI
            if (!(await Commands.Handler(body, ChatType.Group,eventId,msgId)))
            {
                var result = await Models.DeepSeek.SendRequest(id, name, message);
                await Tools.SendGroupMessage(result, openId,eventId,msgId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("群聊消息处理错误");
            Console.WriteLine(body);
            return;
        }
    }
}