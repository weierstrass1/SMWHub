using Newtonsoft.Json;
using System;

namespace LogRegister;

public sealed class LogRegisterSystem
{
    private readonly List<ILoggingRegister> _events = [];
    private readonly ILogCategory[] _possibleCategories;
    private readonly Dictionary<string, Type> _types;
    private readonly Dictionary<string, LogMessageType> _messageTypes;
    public LogRegisterSystem(string loggingFile, params ILogCategory[] possibleCategories)
    {
        if (!File.Exists(loggingFile))
            throw new FileNotFoundException(nameof(loggingFile), loggingFile);
        _possibleCategories = possibleCategories != null ?
        [.. possibleCategories.DistinctBy(c => c.GetType().Name)] :
        [];
        _types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .ToDictionary(t => t.Name);

        var dtos = JsonConvert.DeserializeObject<Dictionary<string, LogMessageTypeDTO>>(loggingFile);
        _messageTypes = dtos.ToDictionary(dto => dto.Key, dto => LogMessageType.FromDTO(this, dto.Value));
    }
    public LogMessageType GetMessageType(string type)
    {
        return _messageTypes[type];
    }
    public LogMessageVariableType CreateVariableType(string name, string type)
    {
        Type genericType = typeof(LogMessageVariableType<>)
            .MakeGenericType(_types[type]);
        return (LogMessageVariableType)Activator.CreateInstance(genericType, name)!;
    }
    public ILogCategory? GetCategory(string name)
    {
        return _possibleCategories.FirstOrDefault(c => c.GetType().Name == name);
    }
    public void Add(ILoggingRegister logRegister)
    {
        _events.Add(logRegister);
    }
    public IReadOnlyList<ILoggingRegister> GetRegisters()
    {
        return _events.AsReadOnly();
    }
    public bool HasLogsOfType<T>() where T : ILogCategory
    {
        Type type = typeof(T);
        return _events.Any(e => e.MessageType.Category.GetType() == type);
    }
}
