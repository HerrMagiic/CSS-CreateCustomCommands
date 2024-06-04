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
        List<string> commandsList = new List<string>();
        // Removes arguments from the command when spaces are present
        for (int i = 0; i < splitCommands.Length; i++)
        {
            if (splitCommands[i].Contains(' '))
            {
                Logger.LogInformation($"Contains space!");
                if (splitCommands[i].IndexOf(' ') == 0)
                {
                    commandsList.Add(splitCommands[i]);
                    continue;
                }
                Logger.LogInformation($"Is multiple args");
                string sub = splitCommands[i].Substring(0, splitCommands[i].IndexOf(' '));
                Logger.LogInformation($"Sub: {sub}");
                if (commandsList.Contains(sub))
                {
                    Logger.LogInformation("In IF");
                    continue;
                }
                
                if (!contains)
                    commandsList.Add(sub);
            }
            else
            {
                if (!commandsList.Contains(splitCommands[i]))
                    commandsList.Add(splitCommands[i]);
            }
        }


        foreach (var command in commandsList)
        {
            Logger.LogInformation($"Command: {command}");
        }

        if (PluginGlobals.Config.RegisterCommandsAsCSSFramework)
            return AddCSSTagsToAliases(commandsList);

        
        return commandsList.ToArray();
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

    public string[] SplitStringByCommaOrSemicolon(string str)
    {
        return Regex.Split(str, "[,;]")
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
