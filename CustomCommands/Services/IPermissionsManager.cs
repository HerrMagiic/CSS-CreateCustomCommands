using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands.Services;

public interface IPermissionsManager
{
    bool RequiresPermissions(CCSPlayerController player, Permission permissions);
}
