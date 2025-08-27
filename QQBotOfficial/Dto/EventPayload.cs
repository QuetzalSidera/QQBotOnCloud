using System.Text.Json.Serialization;
using System.Text.Json;

namespace QQBotOfficial.Dto;

public class EventPayload
{
    [JsonPropertyName("id")] public string EventId { get; set; }
    [JsonPropertyName("op")] public int OpCode { get; set; }
    [JsonPropertyName("d")] public object Data { get; set; }
    [JsonPropertyName("s")] public int Sequence { get; set; }
    [JsonPropertyName("t")] public string EventType { get; set; }
}

public class EventPayload<TEvent>
{
    [JsonPropertyName("id")] public string EventId { get; set; }
    [JsonPropertyName("op")] public int OpCode { get; set; }
    [JsonPropertyName("d")] public TEvent Data { get; set; }
    [JsonPropertyName("s")] public int Sequence { get; set; }
    [JsonPropertyName("t")] public string EventType { get; set; }
}

public class CallbackValidationEvent
{
    [JsonPropertyName("plain_token")] public string PlainToken { get; set; }
    [JsonPropertyName("event_ts")] public string EventTs { get; set; }
}

public class CallbackValidationEventRet
{
    [JsonPropertyName("plain_token")] public string PlainToken { get; set; }
    [JsonPropertyName("signature")] public string Signature { get; set; }
}

public enum OperationCodeEnum
{
    /// <summary>
    /// 服务端进行消息推送
    /// </summary>
    Dispatch = 0,

    /// <summary>
    /// 仅用于 http 回调模式的回包，代表机器人收到了平台推送的数据
    /// </summary>
    CallbackAak = 12,

    /// <summary>
    /// 开放平台对机器人服务端进行验证
    /// </summary>
    CallbackValidation = 13,
}

public static class OperationCodeEnumHelper
{
    public static OperationCodeEnum ToOperationCodeEnum(int opCode)
    {
        return (OperationCodeEnum)opCode;
    }
}

/// <summary>
/// OperationCodeEnum=0时用
/// </summary>
public enum EventTypeEnum
{
    Unknown = 0,
    GatewayEventName = 1,
    C2CMessageCreate = 2,
    GroupAtMessageCreate = 3
}

public static class EventTypeEnumHelper
{
    public static EventTypeEnum ToEventTypeEnum(string eventType)
    {
        return eventType switch
        {
            "GATEWAY_EVENT_NAME" => EventTypeEnum.GatewayEventName,
            "C2C_MESSAGE_CREATE" => EventTypeEnum.C2CMessageCreate,
            "GROUP_AT_MESSAGE_CREATE" => EventTypeEnum.GroupAtMessageCreate,
            "GATEWAY_EVENT_CREATE" => EventTypeEnum.GroupAtMessageCreate,
            _ => EventTypeEnum.Unknown
        };
    }
}