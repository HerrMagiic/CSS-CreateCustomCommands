using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface IPluginUtilities
{
    string[] GettingCommandsFromString(string commands);
    string[] AddCSSTagsToAliases(string[] input);
    string[] SplitStringByCommaOrSemicolon(string str);
    void ExecuteServerCommands(Commands cmd, CCSPlayerController player);
    bool RequiresPermissions(CCSPlayerController player, Permission permissions);
}
