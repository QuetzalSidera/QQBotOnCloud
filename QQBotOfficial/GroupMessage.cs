using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using QQBotOfficial.Dto;

namespace QQBotOfficial;

public static class GroupMessage
{
    private static string _timestampCreate = string.Empty;
    private static long _timestampReceive = 0;
    private static ConcurrentDictionary<GroupMemberId, GroupMessageCache> _cache = new();

    private class GroupMemberId(string groupOpenId, string memberOpenId)
    {
        public readonly string GroupOpenId = groupOpenId;
        public readonly string MemberOpenId = memberOpenId;

        public override int GetHashCode()
        {
            return HashCode.Combine(GroupOpenId, MemberOpenId);
        }

        public override bool Equals(object? obj)
        {
            return obj is GroupMemberId other &&
                   GroupOpenId == other.GroupOpenId &&
                   MemberOpenId == other.MemberOpenId;
        }
    }

    private class GroupMessageCache(string? message, string? msgId)
    {
        /// <summary>
        /// 属于GroupMessageCreate的部分
        /// </summary>
        public string? Message = message;

        /// <summary>
        /// 属于GroupMessageReceive的部分
        /// </summary>
        public string? MsgId = msgId;
    }

    public static async Task Handler(string body, HttpContext httpContext, EventTypeEnum type)
    {
        try
        {
            if (type == EventTypeEnum.GroupAtMessageCreate)
            {
                var response = JsonSerializer.Deserialize<EventPayload<GroupAtMessageCreate>>(body);
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

                var oldTimestamp = _timestampCreate;
                _timestampCreate = response.Data.Timestamp;
                if (oldTimestamp == _timestampCreate)
                    return;

                GroupMemberId memberId = new GroupMemberId(openId, name);
                //处理缓存
                if (_cache.TryGetValue(memberId, out var cache) && cache.MsgId != null)
                {
                    // var eventId = "GROUP_MSG_RECEIVE";
                    //如果普通命令没有处理，则交由AI
                    if (!(await Commands.Handler(body, ChatType.Group, cache.MsgId)))
                    {
                        var result = await Models.DeepSeek.SendRequest(id, name, message);
                        await Tools.SendGroupMessage(result, openId, cache.MsgId);
                    }

                    _cache.TryRemove(memberId, out _);
                }
                else
                {
                    _cache.TryAdd(memberId, new GroupMessageCache(message, null));
                }
            }
            else if (type == EventTypeEnum.GroupAtMessageReceive)
            {
                var response = JsonSerializer.Deserialize<EventPayload<GroupAtMessageReceive>>(body);
                if (response == null)
                    return;
                
                var oldTimestamp = _timestampReceive;
                _timestampReceive = response.Data.Timestamp;
                if (oldTimestamp == _timestampReceive)
                    return;
                
                var msgId = response.EventId;
                var openId = response.Data.GroupOpenId;
                var name = response.Data.OpMemberOpenId;
                ContextId id = new ContextId(ChatType.Group, openId);
                GroupMemberId memberId = new GroupMemberId(openId, name);
                if (_cache.TryGetValue(memberId, out var cache) && cache.Message != null)
                {
                    // var eventId = "GROUP_MSG_RECEIVE";
                    //如果普通命令没有处理，则交由AI
                    if (!(await Commands.Handler(body, ChatType.Group, msgId)))
                    {
                        var result = await Models.DeepSeek.SendRequest(id, name, cache.Message);
                        await Tools.SendGroupMessage(result, openId, msgId);
                    }

                    _cache.TryRemove(memberId, out _);
                }
                else
                {
                    _cache.TryAdd(memberId, new GroupMessageCache(null, msgId));
                }
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