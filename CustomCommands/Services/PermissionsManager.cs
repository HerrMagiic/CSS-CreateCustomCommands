using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CustomCommands.Interfaces;
using CustomCommands.Model;

namespace CustomCommands.Services;

public class PermissionsManager : IPermissionsManager
{
    private readonly IPluginGlobals PluginGlobals;

    public PermissionsManager(IPluginGlobals PluginGlobals)
    {
        this.PluginGlobals = PluginGlobals;
    }

    public bool RequiresPermissions(CCSPlayerController player, Permission permissions)
    {
        if (!permissions.RequiresAllPermissions)
        {
            foreach (var permission in permissions.PermissionList)
            {
                if (AdminManager.PlayerHasPermissions(player, new string[]{permission})) 
                    return true;
            }
            player.PrintToChat($"{PluginGlobals.Config.Prefix}You don't have the required permissions to execute this command");
            return false;
        }
        else
        {
            if (!AdminManager.PlayerHasPermissions(player, permissions.PermissionList.ToArray()))
            {
                player.PrintToChat($"{PluginGlobals.Config.Prefix}You don't have the required permissions to execute this command");
                return false;
            }
            return true;
        }
    }
}
