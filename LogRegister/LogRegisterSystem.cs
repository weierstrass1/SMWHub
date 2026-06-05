using System.Reflection;
using System.Text.Json;

namespace LogRegister;

public sealed class LogRegisterSystem
{
    private readonly static LogMessageType _unknownMessageType = new(UnknownCategory.KEY,
        "Key {key} not found in Log file.",
        new UnknownCategory());
    private readonly static JsonSerializerOptions _deserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };
    private readonly List<ILoggingEntry> _events = [];
    private readonly Dictionary<string, ILogCategory> _possibleCategories;
    private readonly Dictionary<string, LogMessageType> _messageTypes;
    public LogRegisterSystem(string loggingFileContent, Assembly assembly)
    {
        var possibleCategories = getCategoriesInAssembly(assembly);
        _possibleCategories = possibleCategories != null ?
        possibleCategories
            .DistinctBy(c => c.GetType().Name)
            .ToDictionary(c => c.GetType().Name, c => c) :
        [];

        var dtos = JsonSerializer.Deserialize<Dictionary<string, LogMessageTypeDTO>>(loggingFileContent, _deserializeOptions) ??
            throw new InvalidOperationException("Failed to deserialize logging file.");
        _messageTypes = dtos!.ToDictionary(dto => dto.Key, dto => LogMessageType.FromDTO(this, dto.Key, dto.Value));
    }
    public LogRegisterSystem(string loggingFileContent, params ILogCategory[] possibleCategories)
    {
        _possibleCategories = possibleCategories != null ?
        possibleCategories
            .DistinctBy(c => c.GetType().Name)
            .ToDictionary(c => c.GetType().Name, c => c) :
        [];

        var dtos = JsonSerializer.Deserialize<Dictionary<string, LogMessageTypeDTO>>(loggingFileContent, _deserializeOptions) ??
            throw new InvalidOperationException("Failed to deserialize logging file.");
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
        logEntry = GetMessageType(logEntry.MessageTypeKey) == _unknownMessageType ?
            new UnknownLogEntry(logEntry.MessageTypeKey) :
            logEntry;
        GetMessageType(logEntry.MessageTypeKey).Validate(logEntry);
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
    private static IEnumerable<ILogCategory> getCategoriesInAssembly(Assembly assembly)
    {
        return assembly.GetTypes()
        .Where(t => t.IsClass
                 && !t.IsAbstract
                 && typeof(ILogCategory).IsAssignableFrom(t)
                 && t.GetConstructors().Any(c => c.GetParameters().Length == 0))
        .Select(Activator.CreateInstance)
        .Cast<ILogCategory>()
        .ToList();
    }
}
