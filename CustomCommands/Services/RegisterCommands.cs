using CounterStrikeSharp.API.Core.Plugin;
using CounterStrikeSharp.API.Modules.Commands;
using CustomCommands.Interfaces;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;

namespace CustomCommands.Services;
public class RegisterCommands : IRegisterCommands
{
    private readonly ILogger<CustomCommands> _logger;
    private readonly IMessageManager _messageManager;
    private readonly IPluginGlobals _pluginGlobals;
    private readonly PluginContext _pluginContext;
    private readonly IPluginUtilities _pluginUtilities;
    private readonly ICooldownManager _cooldownManager;

    public RegisterCommands(ILogger<CustomCommands> Logger, IMessageManager MessageManager, 
                                IPluginGlobals PluginGlobals, IPluginContext PluginContext, 
                                IPluginUtilities PluginUtilities, ICooldownManager CooldownManager)
    {
        _logger = Logger;
        _messageManager = MessageManager;
        _pluginGlobals = PluginGlobals;
        _pluginContext = (PluginContext as PluginContext)!;
        _pluginUtilities = PluginUtilities;
        _cooldownManager = CooldownManager;
    }

    public void AddCommands(Commands cmd)
    {
        if (!cmd.IsRegisterable)
            return;

        var context = (_pluginContext.Plugin as CustomCommands)!;
        
        context.AddCommand(cmd.Command, cmd.Description, 
            [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
            (player, info) =>
        {
            if (player == null) 
                return;

            var command = cmd;

            // Check if the command has arguments and if it does, check if the command exists and is the right one
            if (info.ArgCount > 1)
            {
                var findcommand = _pluginGlobals.CustomCommands.Find(x => x.Command == command.Command && x.Argument == info.ArgString);
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
                if (!_pluginUtilities.RequiresPermissions(player, command.Permission)) 
                    return;
        
            // Check if the command is on cooldown
            if(_cooldownManager.IsCommandOnCooldown(player, command)) return;

            // Set the cooldown for the command if it has a cooldown set
            _cooldownManager.SetCooldown(player, command);

            // Sending the message to the player
            _messageManager.SendMessage(player, command);

            // Execute the client commands
            _pluginUtilities.ExecuteClientCommands(command, player);

            // Execute the client commands from the server
            _pluginUtilities.ExecuteClientCommandsFromServer(command, player);

            // Execute the server commands
            _pluginUtilities.ExecuteServerCommands(command, player);
        });
    }

    public void CheckForDuplicateCommands()
    {
        var comms = _pluginGlobals.CustomCommands;
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
        _logger.LogError($"------------------------------------------------------------------------");
        _logger.LogError($"{_pluginGlobals.Config.LogPrefix} Duplicate commands found, removing them from the list. Please check your config file for duplicate commands and remove them.");
        for (int i = 0; i < comms.Count; i++)
        {
            if(duplicateCommands.Contains(comms[i]))
            {
                _logger.LogError($"{_pluginGlobals.Config.LogPrefix} Duplicate command found index {i+1}: ");
                _logger.LogError($"{_pluginGlobals.Config.LogPrefix} - {comms[i].Title} ");
                _logger.LogError($"{_pluginGlobals.Config.LogPrefix} - {comms[i].Description}");
                _logger.LogError($"{_pluginGlobals.Config.LogPrefix} - {comms[i].Command}");
                continue;
            }
            
            comms.Add(comms[i]);
        }
        _logger.LogError($"------------------------------------------------------------------------");
    }

    public void ConvertingCommandsForRegister() 
    {
        var newCmds = new List<Commands>();

        foreach (var cmd in _pluginGlobals.CustomCommands)
        {
            var splitCommands = _pluginUtilities.SplitStringByCommaOrSemicolon(cmd.Command);
            splitCommands = _pluginUtilities.AddCSSTagsToAliases(splitCommands.ToList()); 

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

        _pluginGlobals.CustomCommands = newCmds;
    }
}