using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands.Services;
public interface IPluginGlobals
{
    List<CenterClientElement> centerClientOn { get; set; }
    CenterServerElement centerServerOn { get; set; }
    CustomCommandsConfig Config { get; set; }
}
