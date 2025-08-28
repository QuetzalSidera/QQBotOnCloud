using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using DeepSeek.Core;
using DeepSeek.Core.Models;

namespace QQBotOfficial;

public static class Models
{
    public static readonly DeepSeekAssistant DeepSeek = new DeepSeekAssistant();
}

public class DeepSeekAssistant
{
    #region BaseVariables

    private static string _apiKey = "sk-b7b0d00bcf7b465999a09ee49db8804b";
    private static string _model = DeepSeekModels.ChatModel;
    public static readonly List<string> PossibleModel = [DeepSeekModels.ChatModel, DeepSeekModels.ReasonerModel];

    #endregion

    static readonly DeepSeekClient Client = new(_apiKey);
    public static readonly DeepSeekHistoryManager HistoryManager = new();

    public void ChangeApiKey(string apiKey)
    {
        _apiKey = apiKey;
    }

    public bool ChangeModel(string model)
    {
        if (!PossibleModel.Contains(model))
            return false;
        _model = model;
        return true;
    }

    public async Task<string> SendRequest(ContextId id, string name, string message)
    {
        Console.WriteLine($"来源：{name} 消息：{message}");
        //构建上下文
        Message newUserMessage = Message.NewUserMessage(message);
        newUserMessage.Name = name;
        List<Message> context = HistoryManager.GetContext(id);
        context.Add(newUserMessage);
        //发送请求
        var request = new ChatRequest
        {
            Messages = context,
            // 指定模型
            Model = _model
        };
        Console.WriteLine($"上下文条数：{context.Count}");
        foreach (var msg in context)
        {
            Console.WriteLine($"{msg.Role} {msg.Name}:{msg.Content}");
        }

        var result = await Client.ChatAsync(request, CancellationToken.None);
        if (result != null)
        {
            string content = result.Choices.First().Message?.Content ?? "无效响应";
            Message resMessage = Message.NewAssistantMessage(content);
            HistoryManager.TryAddHistory(id, newUserMessage);
            HistoryManager.TryAddHistory(id, resMessage);
            return content;
        }
        else
        {
            return "无效响应";
        }
    }

    public void ChangeSystemMessage(ContextId id, Message message)
    {
        HistoryManager.ChangeSystemMessage(id, message);
    }
}

public class DeepSeekHistoryManager
{
    private int _maxHistoryLength = 20;

    public void ChangeMaxHistoryLength(int maxHistoryLength)
    {
        _maxHistoryLength = maxHistoryLength;
        foreach (var history in _history)
        {
            if (history.Value.Count > _maxHistoryLength)
            {
                history.Value.RemoveRange(0, history.Value.Count - _maxHistoryLength);
            }
        }
    }

    //List索引越小越旧
    readonly ConcurrentDictionary<ContextId, List<Message>> _history = new();

    public bool TryGetHistory(ContextId id, [NotNullWhen(true)] out List<Message>? history)
    {
        return _history.TryGetValue(id, out history);
    }

    public bool TryAddHistory(ContextId id, Message history)
    {
        if (_history.TryGetValue(id, out var before))
        {
            if (before.Count >= _maxHistoryLength)
            {
                before.RemoveAt(0);
            }

            before.Add(history);
            return true;
        }

        _history.TryAdd(id, [history]);
        return true;
    }

    public void RemoveHistory(ContextId id)
    {
        if (_history.TryGetValue(id, out var before))
        {
            before.Clear();
        }
    }

    private readonly ConcurrentDictionary<ContextId, Message> _systemMessage = new();

    public void ChangeSystemMessage(ContextId id, Message message)
    {
        if (_systemMessage.TryGetValue(id, out var before))
        {
            _systemMessage.TryUpdate(id, message, before);
        }
        else
        {
            _systemMessage.TryAdd(id, message);
        }
    }

    public void RemoveSystemMessage(ContextId id)
    {
        _systemMessage.TryRemove(id, out _);
    }

    public bool TryGetSystemMessage(ContextId id, [NotNullWhen(true)] out Message? message)
    {
        return _systemMessage.TryGetValue(id, out message);
    }


    private readonly Message _defaultSystemMessage =
        Message.NewSystemMessage("你的名字是和栗薰子,是日本漫画《薰香花朵凛然绽放》及其衍生番剧的女主角，桔梗女校的二年级学生，成绩优异，靠自己努力考入名门中学，是才学兼备的美少女。性格开朗，待人热情真诚。喜欢甜食，经常光顾凛太郎家的蛋糕店。面对众人对千鸟高校学生的偏见，薰子以热情真诚态度与凛太郎相处，最终二人相恋。");

    public List<Message> GetContext(ContextId id)
    {
        List<Message> ret;
        if (_systemMessage.TryGetValue(id, out var sysMessage))
        {
            ret = [sysMessage];
        }
        else
        {
            ret = [_defaultSystemMessage];
        }

        if (_history.TryGetValue(id, out var histories))
        {
            foreach (var history in histories)
            {
                ret.Add(history);
            }
        }

        return ret;
    }

    private readonly ConcurrentDictionary<ContextId, bool> _isDebugModeDict = new();

    public void ChangeDebugMode(ContextId id, bool isDebugMode)
    {
        try
        {
            _isDebugModeDict[id] = isDebugMode;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public bool TryGetDebugMode(ContextId id, out bool isDebugMode)
    {
        return _isDebugModeDict.TryGetValue(id, out isDebugMode);
    }
}

public class ContextId(ChatType type, string id)
{
    public readonly ChatType Type = type;
    public readonly string Id = id;

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is ContextId other &&
               Type == other.Type &&
               Id == other.Id;
    }
}

public enum ChatType
{
    Group,
    Private,
    ChannelPrivate,
}