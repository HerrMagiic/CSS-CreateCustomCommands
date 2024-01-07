using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface IPermissionsManager
{
    bool RequiresPermissions(CCSPlayerController player, Permission permissions);
}
