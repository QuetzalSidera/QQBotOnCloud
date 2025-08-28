using System.Text.Json.Serialization;

namespace QQBotOfficial.Dto;

public class PrivateReceiveMessage
{
    /// <summary>
    /// 平台方消息ID，可以用于被动消息发送
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 发送者
    /// </summary>
    [JsonPropertyName("author")]
    public AuthorPrivateDto Author { get; set; }

    /// <summary>
    /// 文本消息内容
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }

    /// <summary>
    /// 消息生产时间（RFC3339）
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }

    [JsonPropertyName("message_scene")] public object MessageScene { get; set; }
    [JsonPropertyName("message_type")] public int MessageType { get; set; }
}

public class AuthorPrivateDto
{
    /// <summary>
    /// 用户 openid
    /// </summary>
    [JsonPropertyName("user_openid")]
    public string OpenId { get; set; }

    [JsonPropertyName("union_openid")] public string UnionOpenId { get; set; }
    [JsonPropertyName("id")] public string Id { get; set; }
}

public class AttachmentsDto
{
    /// <summary>
    /// 	文件类型，"image/jpeg","image/png","image/gif"，"file"，"video/mp4"，"voice"
    /// </summary>
    [JsonPropertyName("content_type")]
    public string ContentType { get; set; }

    /// <summary>
    /// 文件名称
    /// </summary>
    [JsonPropertyName("filename")]
    public string FileName { get; set; }

    /// <summary>
    /// 图片高度
    /// </summary>
    [JsonPropertyName("height")]
    public string Height { get; set; }

    /// <summary>
    /// 图片宽度
    /// </summary>
    [JsonPropertyName("width")]
    public string Width { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    [JsonPropertyName("size")]
    public string Size { get; set; }

    /// <summary>
    /// 文件链接
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public class GroupAtMessageCreate
{
    /// <summary>
    /// 平台方消息 ID，可以用于被动消息发送
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 发送者
    /// </summary>
    [JsonPropertyName("author")]
    public AuthorGorupDto Author { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }

    /// <summary>
    /// 消息生产时间（RFC3339）
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }

    /// <summary>
    /// 群聊的 openid
    /// </summary>
    [JsonPropertyName("group_openid")]
    public string GroupOpenId { get; set; }

    /// <summary>
    /// 富媒体文件附件，文件类型："图片，语音，视频，文件"
    /// </summary>
    [JsonPropertyName("attachments")]
    public AttachmentsDto Attachments { get; set; }
}

public class AuthorGorupDto
{
    /// <summary>
    /// 用户在本群的 member_openid
    /// </summary>
    [JsonPropertyName("member_openid")]
    public string MemberOpenId { get; set; }
}

public class GroupAtMessageReceive
{
    [JsonPropertyName("timestamp")] public long Timestamp { get; set; }
    [JsonPropertyName("group_openid")] public string GroupOpenId { get; set; }
    [JsonPropertyName("op_member_openid")] public string OpMemberOpenId { get; set; }
}