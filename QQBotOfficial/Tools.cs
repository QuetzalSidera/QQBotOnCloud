using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using DeepSeek.Core.Models;
using QQBotOfficial;
using QQBotOfficial.Dto;

namespace QQBotOfficial;

public static class Tools
{
    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="openId">QQ 用户的 openid，可在各类事件中获得。</param>
    /// <param name="msgId">前置收到的用户发送过来的消息 ID，用于发送被动消息（回复）</param>
    public static async Task SendPrivateMessage(string message, string openId, string msgId)
    {
        Console.WriteLine("in SendPrivateMessage 0");
        try
        {
            await TokenManager.GetAccessToken();
        }
        catch
        {
            Console.WriteLine("get access token failed");
        }

        Console.WriteLine("in SendPrivateMessage 1");
        string path = $"/v2/users/{openId}/messages";
        var request = new HttpRequestMessage(HttpMethod.Post, Config.BaseUrl + path);
        var bodyStr = JsonSerializer.Serialize(new SendPrivateMessageParams
        {
            Content = message,
            MessageType = 0,
            MsgId = msgId ?? "",
        });
        request.Content = new StringContent(bodyStr);
        if (TokenManager.AccessToken != string.Empty)
        {
            Console.WriteLine("in SendPrivateMessage 2");
            TokenManager.AddAuthHeader(request);
        }

        Console.WriteLine("in SendPrivateMessage 3");
        var res = await HttpClientService.Client.SendAsync(request);
        try
        {
            Console.WriteLine("in SendPrivateMessage 4");
            var jsonResponse = JsonNode.Parse(res.Content.ReadAsStringAsync().Result);
            if (jsonResponse?["code"] is JsonValue jsonValue && jsonValue.TryGetValue<long>(out var code))
            {
                if (code == 11244)
                {
                    await TokenManager.GetAccessToken();
                    Console.WriteLine("in SendPrivateMessage 5");
                }
            }
        }
        catch
        {
            Console.WriteLine("in SendPrivateMessage 6");
        }

        Console.WriteLine("in SendPrivateMessage 7");
    }

    public class SendPrivateMessageParams
    {
        [JsonPropertyName("content")] public string Content { get; set; }

        /// <summary>
        /// 消息类型：0 是文本，2 是 markdown， 3 ark，4 embed，7 media 富媒体
        /// </summary>
        [JsonPropertyName("msg_type")]
        public int MessageType { get; set; }

        [JsonPropertyName("event_id")] public string EventId { get; set; }
        [JsonPropertyName("msg_id")] public string MsgId { get; set; }
    }

    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="groupOpenId">群聊的 openid</param>
    /// <param name="msgId">前置收到的用户发送过来的消息 ID，用于发送被动消息（回复）</param>
    public static async Task SendGroupMessage(string message, string groupOpenId, string msgId)
    {
        Console.WriteLine("in SendGroupMessage 0");
        try
        {
            await TokenManager.GetAccessToken();
        }
        catch
        {
            Console.WriteLine("get access token failed");
        }

        Console.WriteLine("in SendGroupMessage 1");
        string path = $"/v2/groups/{groupOpenId}/messages";
        var request = new HttpRequestMessage(HttpMethod.Post, Config.BaseUrl + path);
        var bodyStr = JsonSerializer.Serialize(new SendGroupMessageParams
        {
            Content = message,
            MessageType = 0,
            MsgId = msgId,
        });
        request.Content = new StringContent(bodyStr);
        if (TokenManager.AccessToken != string.Empty)
        {
            TokenManager.AddAuthHeader(request);
            Console.WriteLine("in SendGroupMessage 2");
        }

        Console.WriteLine("in SendGroupMessage 3");
        var res = await HttpClientService.Client.SendAsync(request);
        try
        {
            Console.WriteLine("in SendGroupMessage 4");
            var jsonResponse = JsonNode.Parse(res.Content.ReadAsStringAsync().Result);
            if (jsonResponse?["code"] is JsonValue jsonValue && jsonValue.TryGetValue<long>(out var code))
            {
                if (code == 11244)
                {
                    await TokenManager.GetAccessToken();
                    Console.WriteLine("in SendGroupMessage 5");
                }
            }
        }
        catch
        {
            Console.WriteLine("in SendGroupMessage 6");
        }

        Console.WriteLine("in SendGroupMessage 7");
    }

    public class SendGroupMessageParams
    {
        [JsonPropertyName("content")] public string Content { get; set; }

        /// <summary>
        /// 消息类型：0 是文本，2 是 markdown， 3 ark，4 embed，7 media 富媒体
        /// </summary>
        [JsonPropertyName("msg_type")]
        public int MessageType { get; set; }

        [JsonPropertyName("msg_id")] public string MsgId { get; set; }
    }


    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="guildId">私信的 guild_id</param>
    /// <param name="msgId">前置收到的用户发送过来的消息 ID，用于发送被动消息（回复）</param>
    public static async Task SendChannelPrivateMessage(string message, string guildId, string msgId)
    {
        Console.WriteLine("in SendChannelPrivateMessage 0");
        try
        {
            await TokenManager.GetAccessToken();
        }
        catch
        {
            Console.WriteLine("get access token failed");
        }

        Console.WriteLine("in SendChannelPrivateMessage 1");
        string path = $"/dms/{guildId}/messages";
        var request = new HttpRequestMessage(HttpMethod.Post, Config.BaseUrl + path);
        var bodyStr = JsonSerializer.Serialize(new SendChannelMessageParams
        {
            Content = message,
            MsgId = msgId,
        });
        request.Content = new StringContent(bodyStr, Encoding.UTF8, "application/json");
        if (TokenManager.AccessToken != string.Empty)
        {
            TokenManager.AddAuthHeader(request);
            Console.WriteLine("in SendChannelPrivateMessage 2");
        }

        // Console.WriteLine("in SendChannelPrivateMessage 3");
        // var res = await HttpClientService.Client.SendAsync(request);
        //
        // try
        // {
        //     Console.WriteLine("in SendChannelPrivateMessage 4");
        //     var jsonResponse = JsonNode.Parse(res.Content.ReadAsStringAsync().Result);
        //     if (jsonResponse?["code"] is JsonValue jsonValue && jsonValue.TryGetValue<long>(out var code))
        //     {
        //         if (code == 11244)
        //         {
        //             await TokenManager.GetAccessToken();
        //             await HttpClientService.Client.SendAsync(request);
        //             Console.WriteLine("in SendChannelPrivateMessage 5");
        //         }
        //     }
        // }
        // catch
        // {
        //     Console.WriteLine("in SendChannelPrivateMessage 6");
        // }

        Console.WriteLine("in SendChannelPrivateMessage 7");
    }


    public class SendChannelMessageParams
    {
        [JsonPropertyName("content")] public string Content { get; set; }

        [JsonPropertyName("msg_id")] public string MsgId { get; set; }
    }
}

public static class Commands
{
    /// <summary>
    /// 聊天命令实现
    /// </summary>
    public static async Task<bool> MessageHandler(string id, string message, ChatType type, string msgId)
    {
        if (message.Trim().StartsWith("@" + Config.Name))
            message = message[(Config.Name.Length + 1)..];
        message = message.Trim();
        string[] cmds = message.Split(' ');
        if (cmds.Length == 0)
            return false;
        string retMessage = string.Empty;
        switch (cmds[0])
        {
            case Config.GoodMorning:
                retMessage = $"早上好,现在是: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ";
                break;
            case Config.FortuneLookUp:
                Random random = new Random();
                int minMaxNumber = random.Next(10, 100);
                retMessage = $"今天你的运势是{minMaxNumber}分";
                break;
            default:
                return false;
        }

        switch (type)
        {
            case ChatType.Group:
                await Tools.SendGroupMessage(retMessage, id, msgId);
                return true;
            case ChatType.Private:
                await Tools.SendPrivateMessage(retMessage, id, msgId);
                return true;
            case ChatType.ChannelPrivate:
                await Tools.SendChannelPrivateMessage(retMessage, id, msgId);
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// 调试命令实现
    /// </summary>
    /// Debug 
    public async static Task<bool> DebugHandler(string body, ChatType type, string msgId)
    {
        const string prefix = "Debug";
        const string modeOn = "-ModeOn";
        const string modeOff = "-ModeOff";
        const string changeSystemMessage = "-ChangeSystemMessage";
        const string removeHistory = "-RemoveHistory";
        const string changeModel = "-ChangeModel";
        const string listModels = "-ListModels";
        const string help = "-Help";

        ContextId? contextId = body.GetContextId();
        if (contextId == null)
            return false;
        const bool isDebugUser = true;
        string? text = body.GetContent();
        text = text?.TrimStart();
        if (text?.StartsWith("@" + Config.Name) == true)
        {
            text = text[(Config.Name.Length + 1)..];
        }

        text = text?.TrimStart();
        if (text == null)
            return false;

        var cmds = text.Split(" ");

        foreach (var cmd in cmds)
            Console.WriteLine(cmd);

        string? guildId = body.GetGuildId();
        if (guildId == null && type == ChatType.ChannelPrivate)
            return false;
        try
        {
            if (cmds[0] != prefix)
                return false;
            if (isDebugUser)
            {
                switch (cmds[1])
                {
                    //打开调试模式
                    case modeOn:
                        switch (type)
                        {
                            case ChatType.Group:
                                DeepSeekAssistant.HistoryManager.ChangeDebugMode(contextId, true);
                                await Tools.SendGroupMessage("[调试模式] 打开", contextId.Id, msgId);
                                return true;
                            case ChatType.Private:
                                DeepSeekAssistant.HistoryManager.ChangeDebugMode(contextId, true);
                                await Tools.SendPrivateMessage("[调试模式] 打开", contextId.Id, msgId);
                                return true;
                            case ChatType.ChannelPrivate:
                                DeepSeekAssistant.HistoryManager.ChangeDebugMode(contextId, true);
                                await Tools.SendChannelPrivateMessage("[调试模式] 打开", guildId ?? string.Empty, msgId);
                                return true;
                            default:
                                return false;
                        }
                    //关闭调试模式
                    case modeOff:
                        const string debugOffMessage = "[调试模式] 关闭";
                        switch (type)
                        {
                            case ChatType.Group:
                                DeepSeekAssistant.HistoryManager.ChangeDebugMode(contextId, false);
                                await Tools.SendGroupMessage(debugOffMessage, contextId.Id, msgId);
                                return true;
                            case ChatType.Private:
                                DeepSeekAssistant.HistoryManager.ChangeDebugMode(contextId, false);
                                await Tools.SendPrivateMessage(debugOffMessage, contextId.Id, msgId);
                                return true;
                            case ChatType.ChannelPrivate:
                                DeepSeekAssistant.HistoryManager.ChangeDebugMode(contextId, true);
                                await Tools.SendChannelPrivateMessage(debugOffMessage, guildId ?? string.Empty, msgId);
                                return true;
                            default:
                                return false;
                        }
                }

                if (!DeepSeekAssistant.HistoryManager.TryGetDebugMode(contextId, out var debugMode) ||
                    debugMode != true)
                {
                    const string debugOffErrorMessage = "[调试输出] 调试模式未打开";
                    switch (type)
                    {
                        case ChatType.Group:
                            await Tools.SendGroupMessage(debugOffErrorMessage, contextId.Id, msgId);
                            return true;
                        case ChatType.Private:
                            await Tools.SendPrivateMessage(debugOffErrorMessage, contextId.Id, msgId);
                            return true;
                        case ChatType.ChannelPrivate:
                            await Tools.SendChannelPrivateMessage(debugOffErrorMessage, guildId ?? string.Empty, msgId);
                            return true;
                        default:
                            return false;
                    }
                }
                else
                {
                    switch (cmds[1])
                    {
                        case changeSystemMessage:
                            string message = cmds.Length > 2 ? cmds[2] : string.Empty;
                            string changeSystemMessageMessage = $"[调试输出] 清空上下文历史，将SystemMessage重置为: {message}";
                            DeepSeekAssistant.HistoryManager.ChangeSystemMessage(contextId,
                                Message.NewSystemMessage(message));
                            DeepSeekAssistant.HistoryManager.RemoveHistory(contextId);
                            switch (type)
                            {
                                case ChatType.Group:
                                    await Tools.SendGroupMessage(changeSystemMessageMessage, contextId.Id, msgId);
                                    return true;
                                case ChatType.Private:
                                    await Tools.SendPrivateMessage(changeSystemMessageMessage, contextId.Id, msgId);
                                    return true;
                                case ChatType.ChannelPrivate:
                                    await Tools.SendChannelPrivateMessage(changeSystemMessageMessage,
                                        guildId ?? string.Empty,
                                        msgId);
                                    return true;
                                default:
                                    return false;
                            }
                        case removeHistory:
                            string removeHistoryMessage = "[调试输出] 清空上下文历史";
                            DeepSeekAssistant.HistoryManager.RemoveHistory(contextId);
                            DeepSeekAssistant.HistoryManager.RemoveSystemMessage(contextId);
                            switch (type)
                            {
                                case ChatType.Group:
                                    await Tools.SendGroupMessage(removeHistoryMessage, contextId.Id, msgId);
                                    return true;
                                case ChatType.Private:
                                    await Tools.SendPrivateMessage(removeHistoryMessage, contextId.Id, msgId);
                                    return true;
                                case ChatType.ChannelPrivate:
                                    await Tools.SendChannelPrivateMessage(removeHistoryMessage, guildId ?? string.Empty,
                                        msgId);
                                    return true;
                                default:
                                    return false;
                            }

                        case changeModel:
                            string model = cmds.Length > 2 ? cmds[2] : string.Empty;
                            string changeModelMessage = $"[调试输出] 模型设置为: {model}";
                            string changeModelErrorMessage = $"[调试输出] 非法输入，不存在的模型: {model}";
                            if (Models.DeepSeek.ChangeModel(model))
                            {
                                switch (type)
                                {
                                    case ChatType.Group:
                                        await Tools.SendGroupMessage(changeModelMessage, contextId.Id, msgId);
                                        return true;
                                    case ChatType.Private:
                                        await Tools.SendPrivateMessage(changeModelMessage, contextId.Id, msgId);
                                        return true;
                                    case ChatType.ChannelPrivate:
                                        await Tools.SendChannelPrivateMessage(changeModelMessage,
                                            guildId ?? string.Empty, msgId);
                                        return true;
                                    default:
                                        return false;
                                }
                            }
                            else
                            {
                                switch (type)
                                {
                                    case ChatType.Group:
                                        await Tools.SendGroupMessage(changeModelErrorMessage, contextId.Id, msgId);
                                        return true;
                                    case ChatType.Private:
                                        await Tools.SendPrivateMessage(changeModelErrorMessage, contextId.Id, msgId);
                                        return true;
                                    case ChatType.ChannelPrivate:
                                        await Tools.SendChannelPrivateMessage(changeModelErrorMessage,
                                            guildId ?? string.Empty,
                                            msgId);
                                        return true;
                                    default:
                                        return false;
                                }
                            }
                        case listModels:
                            string availableModels = string.Join(",", DeepSeekAssistant.PossibleModel);
                            string listModelsMessage = $"[调试输出] 可用模型: {availableModels}";
                            switch (type)
                            {
                                case ChatType.Group:
                                    await Tools.SendGroupMessage(listModelsMessage, contextId.Id, msgId);
                                    return true;
                                case ChatType.Private:
                                    await Tools.SendPrivateMessage(listModelsMessage, contextId.Id, msgId);
                                    return true;
                                case ChatType.ChannelPrivate:
                                    await Tools.SendChannelPrivateMessage(listModelsMessage, guildId ?? string.Empty,
                                        msgId);
                                    return true;
                                default:
                                    return false;
                            }
                        case help:
                            string helpMessage = "[调试输出] 可用命令:\n" +
                                                 prefix + " " + modeOn + " 打开调试模式\n" +
                                                 prefix + " " + modeOff + "关闭调试模式\n" +
                                                 prefix + " " + changeSystemMessage + " <string> 更改人设\n" +
                                                 prefix + " " + removeHistory + " 清空对话上下文\n" +
                                                 prefix + " " + changeModel + " <string> 更改模型\n" +
                                                 prefix + " " + listModels + " 展示可用模型\n" +
                                                 prefix + " " + help + " 命令帮助\n";
                            switch (type)
                            {
                                case ChatType.Group:
                                    await Tools.SendGroupMessage(helpMessage, contextId.Id, msgId);
                                    return true;
                                case ChatType.Private:
                                    await Tools.SendPrivateMessage(helpMessage, contextId.Id, msgId);
                                    return true;
                                case ChatType.ChannelPrivate:
                                    await Tools.SendChannelPrivateMessage(helpMessage, guildId ?? string.Empty, msgId);
                                    return true;
                                default:
                                    return false;
                            }
                        default:
                            string defaultMessage = $"[调试输出] 不支持的命令: {cmds[1]}";
                            switch (type)
                            {
                                case ChatType.Group:
                                    await Tools.SendGroupMessage(defaultMessage, contextId.Id, msgId);
                                    return true;
                                case ChatType.Private:
                                    await Tools.SendPrivateMessage(defaultMessage, contextId.Id, msgId);
                                    return true;
                                case ChatType.ChannelPrivate:
                                    await Tools.SendChannelPrivateMessage(defaultMessage, guildId ?? string.Empty,
                                        msgId);
                                    return true;
                                default:
                                    return false;
                            }
                    }
                }
            }
            else
            {
                const string permissionDeniedMessage = "[调试输出] 暂无权限";
                switch (type)
                {
                    case ChatType.Group:
                        await Tools.SendGroupMessage(permissionDeniedMessage, contextId.Id, msgId);
                        return true;
                    case ChatType.Private:
                        await Tools.SendPrivateMessage(permissionDeniedMessage, contextId.Id, msgId);
                        return true;
                    case ChatType.ChannelPrivate:
                        await Tools.SendChannelPrivateMessage(permissionDeniedMessage, guildId ?? string.Empty, msgId);
                        return true;
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    public static ContextId? GetContextId(this string body)
    {
        var response = JsonSerializer.Deserialize<EventPayload>(body);
        if (response == null)
            return null;
        switch (EventTypeEnumHelper.ToEventTypeEnum(response.EventType))
        {
            case EventTypeEnum.GatewayEventName:
                return null;
                break;
            case EventTypeEnum.GroupMessageReceive:
                var groupProcessedA = JsonSerializer.Deserialize<EventPayload<GroupMessageReceive>>(body);
                if (groupProcessedA == null)
                    return null;
                var groupOpenIdA = groupProcessedA.Data.GroupOpenId;
                return new ContextId(ChatType.Group, groupOpenIdA);
                break;
            case EventTypeEnum.GroupAtMessageCreate:
                var groupProcessedB = JsonSerializer.Deserialize<EventPayload<GroupAtMessageCreate>>(body);
                if (groupProcessedB == null)
                    return null;
                var groupOpenIdB = groupProcessedB.Data.GroupOpenId;
                return new ContextId(ChatType.Group, groupOpenIdB);
                break;
            case EventTypeEnum.C2CMessageCreate:
                var privateProcessedA = JsonSerializer.Deserialize<EventPayload<PrivateReceiveMessage>>(body);
                if (privateProcessedA == null)
                    return null;
                var privateOpenIdA = privateProcessedA.Data.Author.OpenId;
                return new ContextId(ChatType.Group, privateOpenIdA);
                break;
            // case EventTypeEnum.C2CMessageReceive:
            //     var privateProcessed = JsonSerializer.Deserialize<EventPayload<PrivateMessage>>(body);
            //     if (privateProcessed == null)
            //         return null;
            //     var privateOpenId = privateProcessed.Data.Author.OpenId;
            //     return new ContextId(ChatType.Group, privateOpenId);
            //     break;

            case EventTypeEnum.ChannelPrivateReceive:
                var channelPrivateProcessed = JsonSerializer.Deserialize<EventPayload<ChannelPrivateReceive>>(body);
                if (channelPrivateProcessed == null)
                    return null;
                return new ContextId(ChatType.ChannelPrivate, channelPrivateProcessed.Data.Author.UnionOpenID);
            default:
                return null;
                break;
        }
    }

    public static string? GetContent(this string body)
    {
        var response = JsonSerializer.Deserialize<EventPayload>(body);
        if (response == null)
            return null;
        switch (EventTypeEnumHelper.ToEventTypeEnum(response.EventType))
        {
            case EventTypeEnum.GatewayEventName:
                return null;

            case EventTypeEnum.GroupMessageReceive:
                var groupProcessed = JsonSerializer.Deserialize<GroupAtMessageCreate>(body);
                if (groupProcessed == null)
                    return null;
                return groupProcessed.Content;

            case EventTypeEnum.C2CMessageReceive:
                var privateProcessed = JsonSerializer.Deserialize<PrivateReceiveMessage>(body);
                if (privateProcessed == null)
                    return null;
                return privateProcessed.Content;

            default:
                return null;
        }
    }

    public static string? GetGuildId(this string body)
    {
        try
        {
            var response = JsonSerializer.Deserialize<EventPayload<ChannelPrivateReceive>>(body);
            if (EventTypeEnumHelper.ToEventTypeEnum(response?.EventType ?? "") != EventTypeEnum.ChannelPrivateReceive)
                return null;
            return response?.Data.GuildId;
        }
        catch
        {
            return null;
        }
    }
}