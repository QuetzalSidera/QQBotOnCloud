using System.Text.Json.Serialization;
using System.Text.Json;

namespace QQBotOfficial.Dto;

public class EventPayload<TEvent>
{
    [JsonPropertyName("id")] public string EventId;
    [JsonPropertyName("op")] public int OpCode;
    [JsonPropertyName("d")] public TEvent Data;
    [JsonPropertyName("s")] public int Sequence;
    [JsonPropertyName("t")] public string EventType;
}

public enum OperationCodeEnum
{
    Dispatch = 0,
    CallbackAak = 12,
    CallbackValidation = 13
}

public enum EventTypeEnum
{
    Unknown = 0,
}

public class CallbackValidationEvent
{
    [JsonPropertyName("plain_token")] public int PlainToken;
    [JsonPropertyName("event_ts")] public string EventTs;
}