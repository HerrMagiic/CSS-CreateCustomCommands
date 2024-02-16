using CounterStrikeSharp.API.Core.Plugin;
using CounterStrikeSharp.API.Modules.Commands;
using CustomCommands.Interfaces;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;

namespace CustomCommands.Services;
public class RegisterCommands : IRegisterCommands
{
    private readonly ILogger<CustomCommands> Logger;
    private readonly IMessageManager MessageManager;
    private readonly IPluginGlobals PluginGlobals;
    private readonly PluginContext PluginContext;
    private readonly IPluginUtilities PluginUtilities;
    private readonly ICooldownManager CooldownManager;

    public RegisterCommands(ILogger<CustomCommands> Logger, IMessageManager MessageManager, 
                                IPluginGlobals PluginGlobals, IPluginContext PluginContext, 
                                IPluginUtilities PluginUtilities, ICooldownManager CooldownManager)
    {
        this.Logger = Logger;
        this.MessageManager = MessageManager;
        this.PluginGlobals = PluginGlobals;
        this.PluginContext = (PluginContext as PluginContext)!;
        this.PluginUtilities = PluginUtilities;
        this.CooldownManager = CooldownManager;
    }

    /// <summary>
    /// Adds custom commands to the plugin.
    /// </summary>
    /// <param name="cmd">The command to add.</param>
    public void AddCommands(Commands cmd)
    {
        CustomCommands plugin = (PluginContext.Plugin as CustomCommands)!;
        
        string[] aliases = PluginUtilities.GettingCommandsFromString(cmd.Command);

        for (int i = 0; i < aliases.Length; i++)
        {
            plugin.AddCommand(aliases[i], cmd.Description, 
                [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
                (player, info) =>
            {
                if (player == null) 
                    return;
                if (info.ArgCount < aliases[i].Split(' ').Length)
                {
                    player.PrintToChat($"This command requires at least {aliases[i].Split(' ').Length-1} arguments.");
                    return;
                }
                    
                var command = cmd;

                // Check if the command has arguments and if it does, check if the command really exists in the list
                if (info.ArgCount > 1)
                {
                    var findcommand = PluginGlobals.CustomCommands.Find(x => x.Command.Contains(aliases[i] + $" {info.ArgString}"));
                    // Check if the command is equal to the found command
                    if (findcommand! != command)
                        return;

                    command = findcommand;
                }

                // Check if the player has the permission to use the command
                if (command!.Permission.PermissionList.Count > 0 && command.Permission != null)
                    if (!PluginUtilities.RequiresPermissions(player, command.Permission)) 
                        return;
            
                // Check if the command is on cooldown
                if(CooldownManager.IsCommandOnCooldown(player, command)) return;

                // Set the cooldown for the command if it has a cooldown set
                CooldownManager.SetCooldown(player, command);

                // Sending the message to the player
                MessageManager.SendMessage(player, command);

                // Execute the server commands
                PluginUtilities.ExecuteServerCommands(command, player);

                // Execute the client commands
                PluginUtilities.ExecuteClientCommands(command, player);

                // Execute the client commands from the server
                PluginUtilities.ExecuteClientCommandsFromServer(command, player);
            });
        }
    }

    /// <summary>
    /// Checks for duplicate commands in the provided list and removes them.
    /// </summary>
    /// <param name="comms">The list of commands to check for duplicates.</param>
    /// <returns>A new list of commands without any duplicates.</returns>
    public List<Commands> CheckForDuplicateCommands(List<Commands> comms)
    {
        List<Commands> duplicateCommands = new();
        List<Commands> commands = new();
        List<string> commandNames = new();

        foreach (var cmd in comms)
        {
            string[] aliases = cmd.Command.Split(',');

            foreach (var alias in aliases)
            {
                if (commandNames.Contains(alias.ToLower()))
                {
                    duplicateCommands.Add(cmd);
                    continue;
                }
                commandNames.Add(alias.ToLower());
            }
        }
        
        if (duplicateCommands.Count == 0)
            return comms;

        // Log the duplicate commands
        Logger.LogError($"------------------------------------------------------------------------");
        Logger.LogError($"{PluginGlobals.Config.LogPrefix} Duplicate commands found, removing them from the list. Please check your config file for duplicate commands and remove them.");
        for (int i = 0; i < comms.Count; i++)
        {
            if(duplicateCommands.Contains(comms[i]))
            {
                Logger.LogError($"{PluginGlobals.Config.LogPrefix} Duplicate command found index {i+1}: ");
                Logger.LogError($"{PluginGlobals.Config.LogPrefix} - {comms[i].Title} ");
                Logger.LogError($"{PluginGlobals.Config.LogPrefix} - {comms[i].Description}");
                Logger.LogError($"{PluginGlobals.Config.LogPrefix} - {comms[i].Command}");
                continue;
            }
            
            commands.Add(comms[i]);
        }
        Logger.LogError($"------------------------------------------------------------------------");

        return commands;
    }
}