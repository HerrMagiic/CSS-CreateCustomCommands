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

    public void AddCommands(Commands cmd)
    {
        if (!cmd.IsRegisterable)
            return;

        var pluginContext = (PluginContext.Plugin as CustomCommands)!;

        pluginContext.AddCommand(cmd.Command, cmd.Description, 
            [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
            (player, info) =>
        {
            if (player == null) 
                return;

            var command = cmd;

            // Check if the command has arguments and if it does, check if the command exists and is the right one
            if (info.ArgCount > 1)
            {
                var findcommand = PluginGlobals.CustomCommands.Find(x => x.Command == command.Command && x.Argument == info.ArgString);
                // Check if the command is the right one with the right arguments
                if (findcommand is null) 
                {
                    player.PrintToChat("This command requires no Arguments or you added to many");
                    return;
                }
                
                if (!findcommand.Argument!.Equals(info.ArgString)) 
                {
                    player.PrintToChat("Wrong Arguments");
                    return;
                }

                command = findcommand;
            }

            // This will exit the command if the command has arguments but the client didn't provide any
            if (info.ArgCount <= 1 && !string.IsNullOrEmpty(command.Argument!)) 
            {
                player.PrintToChat("This command requires Arguments");
                return;
            }
            
            // Check if the player has the permission to use the command
            if (command!.Permission?.PermissionList.Count > 0 && command.Permission != null)
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

    public void CheckForDuplicateCommands()
    {
        var comms = PluginGlobals.CustomCommands;
        var duplicateCommands = new List<Commands>();
        var commandNames = new List<string>();

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
            return;

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
            
            comms.Add(comms[i]);
        }
        Logger.LogError($"------------------------------------------------------------------------");
    }

    public void ConvertingCommandsForRegister() 
    {
        var newCmds = new List<Commands>();

        foreach (var cmd in PluginGlobals.CustomCommands)
        {
            var splitCommands = PluginUtilities.SplitStringByCommaOrSemicolon(cmd.Command);
            splitCommands = PluginUtilities.AddCSSTagsToAliases(splitCommands.ToList()); 

            foreach (var split in splitCommands)
            {
                var args = split.Split(' ');

                if(args.Length == 1)
                {
                    var newCmd = cmd.Clone() as Commands;
                    newCmd!.ID = Guid.NewGuid();
                    newCmd.Command = split.Trim();
                    newCmds.Add(newCmd);
                } 
                else if (args.Length > 1)
                {
                    var newCmd = cmd.Clone() as Commands;

                    if (newCmds.Any(p => p.Command.Contains(args[0])))
                        newCmd!.IsRegisterable = false;

                    newCmd!.ID = Guid.NewGuid();
                    newCmd.Command = args[0].Trim();
                    args[0] = "";
                    newCmd.Argument = string.Join(" ", args).Trim();
                    newCmds.Add(newCmd);
                }
            }
        }

        PluginGlobals.CustomCommands = newCmds;
    }
}