using System.Text.Json;
using System.Text.Json.Serialization;
using DeepSeek.Core.Models;
using QQBotOfficial.Dto;

namespace QQBotOfficial;

public static class Tools
{
    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="openId">QQ 用户的 openid，可在各类事件中获得。</param>
    /// <param name="eventId">前置收到的事件 ID，用于发送被动消息，支持事件："INTERACTION_CREATE"、"C2C_MSG_RECEIVE"、"FRIEND_ADD"</param>
    /// <param name="msgId">前置收到的用户发送过来的消息 ID，用于发送被动消息（回复）</param>
    public static async Task SendPrivateMessage(string message, string openId, string? eventId = null,
        string? msgId = null)
    {
        Console.WriteLine("in SendPrivateMessage 1");
        string path = $"/v2/users/{openId}/messages";
        var request = new HttpRequestMessage(HttpMethod.Post, Config.BaseUrl + path);
        var bodyStr = JsonSerializer.Serialize(new SendPrivateMessageParams
        {
            Content = message,
            MessageType = 0,
            EventId = eventId ?? "",
            MsgId = msgId ?? "",
        });
        request.Content = new StringContent(bodyStr);
        if (TokenManager.AccessToken != string.Empty)
        {
            Console.WriteLine("in SendPrivateMessage 2");
            TokenManager.AddAuthHeader(request);
        }

        var res = await HttpClientService.Client.SendAsync(request);
        Console.WriteLine(res.Content.ReadAsStringAsync().Result);
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

        await HttpClientService.Client.SendAsync(request);
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
}

public static class Commands
{
    /// <summary>
    /// 常用命令实现
    /// </summary>
    /// Debug 
    public async static Task<bool> Handler(string body, ChatType type, string msgId)
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
                                await Tools.SendGroupMessage(contextId.Id, "[调试模式] 打开", msgId);
                                return true;
                            case ChatType.Private:
                                DeepSeekAssistant.HistoryManager.ChangeDebugMode(contextId, true);
                                await Tools.SendPrivateMessage(contextId.Id, "[调试模式] 打开", msgId);
                                return true;
                            default:
                                return false;
                        }
                    //关闭调试模式
                    case modeOff:
                        switch (type)
                        {
                            case ChatType.Group:
                                DeepSeekAssistant.HistoryManager.ChangeDebugMode(contextId, false);
                                await Tools.SendGroupMessage(contextId.Id, "[调试模式] 关闭", msgId);
                                return true;
                            case ChatType.Private:
                                DeepSeekAssistant.HistoryManager.ChangeDebugMode(contextId, false);
                                await Tools.SendPrivateMessage(contextId.Id, "[调试模式] 关闭", msgId);
                                return true;
                            default:
                                return false;
                        }
                }

                if (!DeepSeekAssistant.HistoryManager.TryGetDebugMode(contextId, out var debugMode) ||
                    debugMode != true)
                {
                    switch (type)
                    {
                        case ChatType.Group:
                            await Tools.SendGroupMessage(contextId.Id, "[调试输出] 调试模式未打开", msgId);
                            return false;
                        case ChatType.Private:
                            await Tools.SendPrivateMessage(contextId.Id, "[调试输出] 调试模式未打开", msgId);
                            return false;
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
                                    await Tools.SendGroupMessage(contextId.Id, changeSystemMessageMessage, msgId);
                                    return true;
                                case ChatType.Private:
                                    await Tools.SendPrivateMessage(contextId.Id, changeSystemMessageMessage, msgId);
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
                                    await Tools.SendGroupMessage(contextId.Id, removeHistoryMessage, msgId);
                                    return true;
                                case ChatType.Private:
                                    await Tools.SendPrivateMessage(contextId.Id, removeHistoryMessage, msgId);
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
                                        await Tools.SendGroupMessage(contextId.Id, changeModelMessage, msgId);
                                        return true;
                                    case ChatType.Private:
                                        await Tools.SendPrivateMessage(contextId.Id, changeModelMessage, msgId);
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
                                        await Tools.SendGroupMessage(contextId.Id, changeModelErrorMessage, msgId);
                                        return true;
                                    case ChatType.Private:
                                        await Tools.SendPrivateMessage(contextId.Id, changeModelErrorMessage, msgId);
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
                                    await Tools.SendGroupMessage(contextId.Id, listModelsMessage, msgId);
                                    return true;
                                case ChatType.Private:
                                    await Tools.SendPrivateMessage(contextId.Id, listModelsMessage, msgId);
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
                                    await Tools.SendGroupMessage(contextId.Id, helpMessage, msgId);
                                    return true;
                                case ChatType.Private:
                                    await Tools.SendPrivateMessage(contextId.Id, helpMessage, msgId);
                                    return true;
                                default:
                                    return false;
                            }
                        default:
                            string defaultMessage = $"[调试输出] 不支持的命令: {cmds[1]}";
                            switch (type)
                            {
                                case ChatType.Group:
                                    await Tools.SendGroupMessage(contextId.Id, defaultMessage, msgId);
                                    return true;
                                case ChatType.Private:
                                    await Tools.SendPrivateMessage(contextId.Id, defaultMessage, msgId);
                                    return true;
                                default:
                                    return false;
                            }
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case ChatType.Group:
                        await Tools.SendGroupMessage(contextId.Id, "[调试输出] 暂无权限", msgId);
                        return true;
                    case ChatType.Private:
                        await Tools.SendPrivateMessage(contextId.Id, "[调试输出] 暂无权限", msgId);
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
            case EventTypeEnum.GroupAtMessageReceive:
                var groupProcessed = JsonSerializer.Deserialize<EventPayload<GroupAtMessageCreate>>(body);
                if (groupProcessed == null)
                    return null;
                var groupOpenId = groupProcessed.Data.GroupOpenId;
                return new ContextId(ChatType.Group, groupOpenId);
                break;
            case EventTypeEnum.C2CMessageCreate:
                var privateProcessed = JsonSerializer.Deserialize<EventPayload<PrivateReceiveMessage>>(body);
                if (privateProcessed == null)
                    return null;
                var privateOpenId = privateProcessed.Data.Author.OpenId;
                return new ContextId(ChatType.Group, privateOpenId);
                break;
            // case EventTypeEnum.C2CMessageCreate:
            //     var privateProcessed = JsonSerializer.Deserialize<EventPayload<PrivateReceiveMessage>>(body);
            //     if (privateProcessed == null)
            //         return null;
            //     var privateOpenId = privateProcessed.Data.Author.OpenId;
            //     return new ContextId(ChatType.Group, privateOpenId);
            //     break;
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

            case EventTypeEnum.GroupAtMessageReceive:
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
}