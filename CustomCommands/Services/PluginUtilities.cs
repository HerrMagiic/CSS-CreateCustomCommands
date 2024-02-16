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

    public string[] GettingCommandsFromString(string commands)
    {
        string[] splitCommands = SplitStringByCommaOrSemicolon(commands);

        if (PluginGlobals.Config.RegisterCommandsAsCSSFramework)
            return AddCSSTagsToAliases(splitCommands);

        return splitCommands;
    }

    /// <summary>
    /// Adds the css_ prefix to each alias.
    /// This will help cs# tell this command belongs to the framework.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string[] AddCSSTagsToAliases(string[] input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (!input[i].StartsWith("css_"))
                input[i] = "css_" + input[i];
        }
        return input;
    }

    /// <summary>
    /// Splits a string by comma, semicolon, or whitespace characters.
    /// </summary>
    /// <param name="str">The string to be split.</param>
    /// <returns>An array of strings containing the split substrings.</returns>
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
            // If the command starts with "toggle" we want to toggle the cvar
            if (serverCommand.StartsWith("toggle"))
            {
                HandleToggleCommand(serverCommand);
                continue;
            }

            Server.ExecuteCommand(ReplaceTagsFunctions.ReplaceMessageTags(serverCommand, player));
        }
    }
    
    public void ExecuteClientCommands(Commands cmd, CCSPlayerController player)
    {
        if (cmd.ClientCommands.Count == 0) return;

        foreach (var clientCommand in cmd.ClientCommands)
        {
            player.ExecuteClientCommand(ReplaceTagsFunctions.ReplaceMessageTags(clientCommand, player));
        }
    }
    
    public void ExecuteClientCommandsFromServer(Commands cmd, CCSPlayerController player)
    {
        if (cmd.ClientCommandsFromServer.Count == 0) return;

        foreach (var clientCommandsFromServer in cmd.ClientCommandsFromServer)
        {
            player.ExecuteClientCommandFromServer(ReplaceTagsFunctions.ReplaceMessageTags(clientCommandsFromServer, player));
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
            if (commandCvar.GetPrimitiveValue<bool>())
                Server.ExecuteCommand($"{commandWithoutToggle} 0");
            else
                Server.ExecuteCommand($"{commandWithoutToggle} 1");
        }
        else
        {
            Logger.LogError($"Couldn't toggle {commandWithoutToggle}. Please check if this command is toggleable");
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
