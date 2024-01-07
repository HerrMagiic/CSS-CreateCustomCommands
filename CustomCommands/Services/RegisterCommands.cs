using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core.Plugin;
using CustomCommands.Interfaces;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;

namespace CustomCommands.Services;
public class RegisterCommands : IRegisterCommands
{
    private readonly ILogger<CustomCommands> Logger;
    private readonly IMessageManager MessageManager;
    private readonly IPluginGlobals PluginGlobals;
    private readonly IPermissionsManager PermissionsManager;
    private readonly PluginContext PluginContext;

    public RegisterCommands(ILogger<CustomCommands> Logger, IMessageManager MessageManager, 
                                IPluginGlobals PluginGlobals, IPermissionsManager PermissionsManager, IPluginContext PluginContext)
    {
        this.Logger = Logger;
        this.MessageManager = MessageManager;
        this.PluginGlobals = PluginGlobals;
        this.PermissionsManager = PermissionsManager;
        this.PluginContext = (PluginContext as PluginContext)!;
    }

    public void AddCommands(Commands com)
    {
        CustomCommands plugin = (PluginContext.Plugin as CustomCommands)!;
        
        string[] aliases = com.Command.Split(',');

        for (int i = 0; i < aliases.Length; i++)
        {
            plugin.AddCommand(aliases[i], com.Description, (player, info) =>
            {
                if (player == null) return;
                
                if (com.Permission.PermissionList.Count > 0 && com.Permission != null)
                    if (!PermissionsManager.RequiresPermissions(player, com.Permission)) 
                        return;
                
                MessageManager.SendMessage(player, com);

                ExecuteServerCommands(com);
            });
        }
    }

    public List<Commands> CheckForDuplicateCommands(List<Commands> comms)
    {
        List<Commands> duplicateCommands = new();
        List<Commands> commands = new();
        List<string> commandNames = new();

        foreach (var com in comms)
        {
            string[] aliases = com.Command.Split(',');

            foreach (var alias in aliases)
            {
                if (commandNames.Contains(alias))
                {
                    duplicateCommands.Add(com);
                    continue;
                }
                commandNames.Add(alias);
            }
        }
        
        if (duplicateCommands.Count == 0)
            return comms;

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
    public void ExecuteServerCommands(Commands cmd) 
    {
        if (cmd.ServerCommands.Count == 0) return;

        foreach (var serverCommand in cmd.ServerCommands)
        {
            Server.ExecuteCommand(serverCommand);
        }
    }
}