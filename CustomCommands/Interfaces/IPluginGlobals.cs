using CustomCommands.Model;

namespace CustomCommands.Interfaces;
public interface IPluginGlobals
{
    List<CenterClientElement> centerClientOn { get; set; }
    CenterServerElement centerServerOn { get; set; }
    CustomCommandsConfig Config { get; set; }
    List<CooldownTimer> CooldownTimer { get; set; }
    List<Commands> CustomCommands { get; set; }
}
