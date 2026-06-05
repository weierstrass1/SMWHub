using Newtonsoft.Json;

namespace LogRegister;

public sealed class LogRegisterSystem
{
    private readonly static LogMessageType _unknownMessageType = new(UnknownCategory.KEY,
        "Key {key} not found in Log file.",
        new UnknownCategory());
    private readonly List<ILoggingEntry> _events = [];
    private readonly Dictionary<string, ILogCategory> _possibleCategories;
    private readonly Dictionary<string, LogMessageType> _messageTypes;
    public LogRegisterSystem(string loggingFile, params ILogCategory[] possibleCategories)
    {
        if (!File.Exists(loggingFile))
            throw new FileNotFoundException(nameof(loggingFile), loggingFile);
        _possibleCategories = possibleCategories != null ?
        possibleCategories
            .DistinctBy(c => c.GetType().Name)
            .ToDictionary(c => c.GetType().Name, c=> c) :
        [];

        var dtos = JsonConvert.DeserializeObject<Dictionary<string, LogMessageTypeDTO>>(loggingFile);
        _messageTypes = dtos!.ToDictionary(dto => dto.Key, dto => LogMessageType.FromDTO(this, dto.Value));
    }
    public LogMessageType GetMessageType(string key)
    {
        if (!_possibleCategories.ContainsKey(key))
            return _unknownMessageType;
        return _messageTypes[key];
    }
    public ILogCategory? GetCategory(string name)
    {
        if (!_possibleCategories.ContainsKey(name))
            return _unknownMessageType.Category;
        return _possibleCategories[name];
    }
    public void Add(ILoggingEntry logEntry)
    {
        _events.Add(GetMessageType(logEntry.MessageTypeKey) == _unknownMessageType ?
            new UnknownLogEntry(logEntry.MessageTypeKey) :
            logEntry);
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
