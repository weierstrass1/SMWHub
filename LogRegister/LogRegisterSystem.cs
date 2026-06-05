using Newtonsoft.Json;

namespace LogRegister;

public sealed class LogRegisterSystem
{
    private readonly List<ILoggingEntry> _events = [];
    private readonly ILogCategory[] _possibleCategories;
    private readonly Dictionary<string, LogMessageType> _messageTypes;
    public LogRegisterSystem(string loggingFile, params ILogCategory[] possibleCategories)
    {
        if (!File.Exists(loggingFile))
            throw new FileNotFoundException(nameof(loggingFile), loggingFile);
        _possibleCategories = possibleCategories != null ?
        [.. possibleCategories.DistinctBy(c => c.GetType().Name)] :
        [];

        var dtos = JsonConvert.DeserializeObject<Dictionary<string, LogMessageTypeDTO>>(loggingFile);
        _messageTypes = dtos!.ToDictionary(dto => dto.Key, dto => LogMessageType.FromDTO(this, dto.Value));
    }
    public LogMessageType GetMessageType(string type)
    {
        return _messageTypes[type];
    }
    public ILogCategory? GetCategory(string name)
    {
        return _possibleCategories.FirstOrDefault(c => c.GetType().Name == name);
    }
    public void Add(ILoggingEntry logEntry)
    {
        _events.Add(logEntry);
    }
    public IReadOnlyList<ILoggingEntry> GetEntries()
    {
        return _events.AsReadOnly();
    }
    public bool HasLogsOfType<T>() where T : ILogCategory
    {
        Type type = typeof(T);
        return _events.Any(e => GetMessageType(e.MessageTypeKey).Category.GetType() == type);
    }
}
