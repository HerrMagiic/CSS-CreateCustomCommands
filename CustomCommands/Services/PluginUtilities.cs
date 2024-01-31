using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CustomCommands.Interfaces;
using CustomCommands.Model;

namespace CustomCommands.Services;

public class PluginUtilities : IPluginUtilities
{
    private readonly IPluginGlobals PluginGlobals;
    private readonly IReplaceTagsFunctions ReplaceTagsFunctions;
    
    public PluginUtilities(IPluginGlobals PluginGlobals, IReplaceTagsFunctions ReplaceTagsFunctions)
    {
        this.PluginGlobals = PluginGlobals;
        this.ReplaceTagsFunctions = ReplaceTagsFunctions;
    }

    public string[] SplitStringByCommaOrSemicolon(string str)
    {
        return Regex.Split(str, ",|;|\\s")
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToArray();
    }
    
    public void ExecuteServerCommands(Commands cmd, CCSPlayerController player) 
    {
        if (cmd.ServerCommands.Count == 0) return;

        foreach (var serverCommand in cmd.ServerCommands)
        {
            Server.ExecuteCommand(ReplaceTagsFunctions.ReplaceMessageTags(serverCommand, player));
        }
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
