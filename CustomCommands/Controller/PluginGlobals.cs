using CustomCommands.Model;
using CustomCommands.Services;

namespace CustomCommands.Controller;
public class PluginGlobals : IPluginGlobals
{
    public List<CenterClientElement> centerClientOn { get; set; } = new();
    public CenterServerElement centerServerOn { get; set; } = new();
    public CustomCommandsConfig Config { get; set; } = new();
}
