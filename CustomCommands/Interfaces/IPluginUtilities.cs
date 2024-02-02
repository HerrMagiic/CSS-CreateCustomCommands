
using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface IPluginUtilities
{
    string[] SplitStringByCommaOrSemicolon(string str);
    void ExecuteServerCommands(Commands cmd, CCSPlayerController player);
    bool RequiresPermissions(CCSPlayerController player, Permission permissions);
    bool IsCommandOnCooldown(CCSPlayerController player, Commands cmd);
    void AddToCooldownList(bool isGlobal, int playerID, Guid commandID, int cooldownTime);
    void SetCooldown(CCSPlayerController player, Commands cmd);
}
