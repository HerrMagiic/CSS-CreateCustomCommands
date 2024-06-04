using CustomCommands.Interfaces;
using CustomCommands.Model;

namespace CustomCommands.Services;
public class PluginGlobals : IPluginGlobals
{
    /// <summary>
    /// List of clients that have a message printed to their center.
    /// </summary>
    public List<CenterClientElement> centerClientOn { get; set; } = new();
    /// <summary>
    /// The message that is printed to all players' center.
    /// </summary>
    public CenterServerElement centerServerOn { get; set; } = new();
    /// <summary>
    /// The configuration for the plugin.
    /// </summary>
    public CustomCommandsConfig Config { get; set; } = new();
    /// <summary>
    /// List of cooldown timers for each player.
    /// </summary>
    public List<CooldownTimer> CooldownTimer { get; set; } = new();
    /// <summary>
    /// List of custom commands.
    /// </summary>
    public List<Commands> CustomCommands { get; set; } = new();
}
