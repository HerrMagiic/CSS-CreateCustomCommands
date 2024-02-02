using CustomCommands.Interfaces;
using CustomCommands.Model;

namespace CustomCommands.Services;
public class PluginGlobals : IPluginGlobals
{
    public List<CenterClientElement> centerClientOn { get; set; } = new();
    public CenterServerElement centerServerOn { get; set; } = new();
    public CustomCommandsConfig Config { get; set; } = new();
    public List<CooldownTimer> CooldownTimer { get; set; } = new();
    public List<Commands> CustomCommands { get; set; } = new();
}
