using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Cvars;
using CustomCommands.Interfaces;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;

namespace CustomCommands.Services;

public partial class PluginUtilities : IPluginUtilities
{
    private readonly IPluginGlobals _pluginGlobals;
    private readonly IReplaceTagsFunctions _replaceTagsFunctions;
    private readonly ILogger<CustomCommands> _logger;

    public PluginUtilities(IPluginGlobals PluginGlobals, IReplaceTagsFunctions ReplaceTagsFunctions,
                            ILogger<CustomCommands> Logger)
    {
        _pluginGlobals = PluginGlobals;
        _replaceTagsFunctions = ReplaceTagsFunctions;
        _logger = Logger;
    }
    
    public string[] AddCSSTagsToAliases(List<string> input)
    {
        for (int i = 0; i < input.Count; i++)
        {
            if (!input[i].StartsWith("css_"))
                input[i] = "css_" + input[i];
        }
        return input.ToArray();
    }
    
    [GeneratedRegex("[,;]")]
    private static partial Regex SplitStringByCommaOrSemicolonRegex();

    public string[] SplitStringByCommaOrSemicolon(string str)
    {
        return SplitStringByCommaOrSemicolonRegex().Split(str)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToArray();
    }
    
    public void ExecuteServerCommands(Commands cmd, CCSPlayerController player)
    {
        if (cmd.ServerCommands.Count == 0) 
            return;

        foreach (var serverCommand in cmd.ServerCommands)
        {
            // If the command starts with "toggle" we want to toggle the cvar
            if (serverCommand.StartsWith("toggle"))
            {
                HandleToggleCommand(serverCommand);
                continue;
            }


            Server.ExecuteCommand(_replaceTagsFunctions.ReplaceMessageTags(serverCommand, player));
        }
    }
    
    public void ExecuteClientCommands(Commands cmd, CCSPlayerController player)
    {
        if (cmd.ClientCommands.Count == 0) 
            return;

        foreach (var clientCommand in cmd.ClientCommands)
        {
            player.ExecuteClientCommand(_replaceTagsFunctions.ReplaceMessageTags(clientCommand, player));
        }
    }
    
    public void ExecuteClientCommandsFromServer(Commands cmd, CCSPlayerController player)
    {
        if (cmd.ClientCommandsFromServer.Count == 0) 
            return;

        foreach (var clientCommandsFromServer in cmd.ClientCommandsFromServer)
        {
            player.ExecuteClientCommandFromServer(_replaceTagsFunctions.ReplaceMessageTags(clientCommandsFromServer, player));
        }
    }
    
    /// <summary>
    /// Handles the toggle command
    /// </summary>
    /// <param name="serverCommand"></param>
    private void HandleToggleCommand(string serverCommand)
    {
        var commandWithoutToggle = serverCommand.Replace("toggle ", "");
        var commandCvar = ConVar.Find(commandWithoutToggle);

        if (commandCvar != null)
        {
            if (commandCvar.GetPrimitiveValue<bool>())
                Server.ExecuteCommand($"{commandWithoutToggle} 0");
            else
                Server.ExecuteCommand($"{commandWithoutToggle} 1");
        }
        else
        {
            _logger.LogError($"Couldn't toggle {commandWithoutToggle}. Please check if this command is toggleable");
        }
    }
    
    public bool RequiresPermissions(CCSPlayerController player, Permission permissions)
    {
        if (!permissions.RequiresAllPermissions)
        {
            foreach (var permission in permissions.PermissionList)
            {
                if (AdminManager.PlayerHasPermissions(player, new string[] { permission }))
                    return true;
            }
            player.PrintToChat($"{_pluginGlobals.Config.Prefix}You don't have the required permissions to execute this command");
            return false;
        }
        else
        {
            if (!AdminManager.PlayerHasPermissions(player, permissions.PermissionList.ToArray()))
            {
                player.PrintToChat($"{_pluginGlobals.Config.Prefix}You don't have the required permissions to execute this command");
                return false;
            }
            return true;
        }
    }
}
