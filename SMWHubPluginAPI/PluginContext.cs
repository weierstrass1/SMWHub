using ZWXStateMachine;
using ZWXStateMachine.Interfaces;

namespace SMWHubPluginAPI;
/// <summary>
/// Represents the context of a plugin, which can be used to store shared data between Resource Plugins and Format Plugins. It implements the IHaveStateData interface, allowing it to hold state data that can be accessed and modified by the plugins.
/// </summary>
public class PluginContext(string pluginName) : IHaveStateData
{
    public string PluginName { get; } = pluginName;
    public StateData StateData { get; } = new();
}
