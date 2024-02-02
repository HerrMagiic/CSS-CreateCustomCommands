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
    private readonly PluginContext PluginContext;
    private readonly IPluginUtilities PluginUtilities;

    public RegisterCommands(ILogger<CustomCommands> Logger, IMessageManager MessageManager, 
                                IPluginGlobals PluginGlobals, IPluginContext PluginContext, 
                                IPluginUtilities PluginUtilities)
    {
        this.Logger = Logger;
        this.MessageManager = MessageManager;
        this.PluginGlobals = PluginGlobals;
        this.PluginContext = (PluginContext as PluginContext)!;
        this.PluginUtilities = PluginUtilities;
    }

    public void AddCommands(Commands com)
    {
        CustomCommands plugin = (PluginContext.Plugin as CustomCommands)!;
        
        string[] aliases = PluginUtilities.SplitStringByCommaOrSemicolon(com.Command);

        for (int i = 0; i < aliases.Length; i++)
        {
            plugin.AddCommand(aliases[i], com.Description, (player, info) =>
            {
                if (player == null) return;
                
                if (com.Permission.PermissionList.Count > 0 && com.Permission != null)
                    if (!PluginUtilities.RequiresPermissions(player, com.Permission)) 
                        return;
            
                if(PluginUtilities.IsCommandOnCooldown(player, com)) return;

                PluginUtilities.SetCooldown(player, com);

                MessageManager.SendMessage(player, com);

                PluginUtilities.ExecuteServerCommands(com, player);
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
}