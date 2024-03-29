using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Cvars;
using CustomCommands.Interfaces;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;

namespace CustomCommands.Services;

public class PluginUtilities : IPluginUtilities
{
    private readonly IPluginGlobals PluginGlobals;
    private readonly IReplaceTagsFunctions ReplaceTagsFunctions;
    private readonly ILogger<CustomCommands> Logger;
    
    public PluginUtilities(IPluginGlobals PluginGlobals, IReplaceTagsFunctions ReplaceTagsFunctions,
                            ILogger<CustomCommands> Logger)
    {
        this.PluginGlobals = PluginGlobals;
        this.ReplaceTagsFunctions = ReplaceTagsFunctions;
        this.Logger = Logger;
    }

    public string[] SplitStringByCommaOrSemicolon(string str)
    {
        return Regex.Split(str, ",|;|\\s")
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToArray();
    }
    /// <summary>
    /// Executes the server commands from the command object
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="player"></param>
    public void ExecuteServerCommands(Commands cmd, CCSPlayerController player) 
    {
        if (cmd.ServerCommands.Count == 0) return;

        foreach (var serverCommand in cmd.ServerCommands)
        {
            // If the command starts with "toggle" we want to toggle the cvar
            if (serverCommand.StartsWith("toggle"))
            {
                HandleToggleCommand(serverCommand);
                continue;
            }

            Server.ExecuteCommand(ReplaceTagsFunctions.ReplaceMessageTags(serverCommand, player));
        }
    }
    /// <summary>
    /// Handles the toggle command
    /// </summary>
    /// <param name="serverCommand"></param>
    private void HandleToggleCommand(string serverCommand)
    {
        string commandWithoutToggle = serverCommand.Replace("toggle ", "");
        var commandCvar = ConVar.Find(commandWithoutToggle);
        if (commandCvar != null)
        {
            if(commandCvar.GetPrimitiveValue<bool>())
                Server.ExecuteCommand($"{commandWithoutToggle} 0");
            else
                Server.ExecuteCommand($"{commandWithoutToggle} 1");
        }
        else
        {
            Logger.LogError($"Couldn't toggle {commandWithoutToggle}. Please check if this command is toggleable");
        }
    }
    /// <summary>
    /// Checks if the player has the required permissions to execute the command
    /// </summary>
    /// <param name="player"></param>
    /// <param name="permissions"></param>
    /// <returns></returns>
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
