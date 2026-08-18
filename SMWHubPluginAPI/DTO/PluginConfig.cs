using System.Text.Json.Serialization;

namespace SMWHubPluginAPI.DTO;

public class PluginConfig
{
    [JsonRequired]
    public required string Version { get; init; }
    [JsonRequired]
    public required int GetPackagePriority { get; init; }
    [JsonRequired]
    public required int ProcessPriority { get; init; }
    [JsonRequired]
    public required int InstallationPriority { get; init; }
}
