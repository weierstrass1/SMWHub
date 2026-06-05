using Newtonsoft.Json;

namespace LogRegister;
public sealed class LogMessageTypeDTO
{
    [JsonProperty(Required = Required.Always)]
    public required string Category { get; set; }
    [JsonProperty(Required = Required.Always)]
    public required string Message { get; set; }
    [JsonProperty(Required = Required.Always)]
    public required string MessageType { get; set; }
}
