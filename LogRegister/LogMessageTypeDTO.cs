namespace LogRegister;
public sealed class LogMessageTypeDTO
{
    public string Category { get; set; }
    public string Message { get; set; }
    public string MessageType { get; set; }
    public LogMessageDTOVariableType[] variables;
}
public sealed class LogMessageDTOVariableType
{
    public string Name;
    public string Type;
}
