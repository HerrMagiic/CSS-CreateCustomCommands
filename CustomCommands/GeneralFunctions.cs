using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;

namespace CustomCommands;
public partial class CustomCommands
{
    private void RegisterListeners()
    {
        RegisterListener<Listeners.OnTick>(() =>
        {
            // Client Print To Center
            foreach (var player in centerClientOn)
                Utilities.GetPlayerFromUserid(player.ClientId).PrintToCenterHtml(player.Message, 1);

            // Server Print To Center
            if (centerServerOn.IsRunning)
            {
                Utilities.GetPlayers().ForEach(controller =>
                {
                    if (controller == null || !controller.IsValid) return;

                    string message = ReplaceMessageTags(centerServerOn.Message, controller);
                    controller.PrintToCenterHtml(message, 1);
                });
            }
        });
    }

    private List<Commands> CheckForDuplicateCommands(List<Commands> comms)
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
        Logger.LogError($"{Config.LogPrefix} Duplicate commands found, removing them from the list. Please check your config file for duplicate commands and remove them.");
        for (int i = 0; i < comms.Count; i++)
        {
            if(duplicateCommands.Contains(comms[i]))
            {
                Logger.LogError($"{Config.LogPrefix} Duplicate command found index {i+1}: ");
                Logger.LogError($"{Config.LogPrefix} - {comms[i].Title} ");
                Logger.LogError($"{Config.LogPrefix} - {comms[i].Description}");
                Logger.LogError($"{Config.LogPrefix} - {comms[i].Command}");
                continue;
            }
            
            commands.Add(comms[i]);
        }
        Logger.LogError($"------------------------------------------------------------------------");

        return commands;
    }
    private void AddCommands(Commands com)
    {
        string[] aliases = com.Command.Split(',');

        for (int i = 0; i < aliases.Length; i++)
        {
            AddCommand(aliases[i], com.Description, (player, info) =>
            {
                if (player == null) return;
                
                if (com.Permission.PermissionList.Count > 0 && com.Permission != null)
                    if (!RequiresPermissions(player, com.Permission)) 
                        return;
                
                SendMessage(player, com);

                ExecuteServerCommands(com);
            });
        }
    }

    private bool RequiresPermissions(CCSPlayerController player, Permission permissions)
    {
        if (!permissions.ReguiresAllPermissions)
        {
            foreach (var permission in permissions.PermissionList)
            {
                if (AdminManager.PlayerHasPermissions(player, new string[]{permission})) 
                    return true;
            }
            player.PrintToChat($"{PrefixCache}You don't have the required permissions to execute this command");
            return false;
        }
        else
        {
            if (!AdminManager.PlayerHasPermissions(player, permissions.PermissionList.ToArray()))
            {
                player.PrintToChat($"{PrefixCache}You don't have the required permissions to execute this command");
                return false;
            }
            return true;
        }
    }

    private void ExecuteServerCommands(Commands cmd) 
    {
        if (cmd.ServerCommands.Count == 0) return;

        foreach (var serverCommand in cmd.ServerCommands)
        {
            Server.ExecuteCommand(serverCommand);
        }
    }
    
    private void SendMessage(CCSPlayerController player, Commands cmd) 
    {
        switch (cmd.PrintTo)
        {
            case Sender.ClientChat:
                PrintToChat(Receiver.Client, player, cmd.Message);
                break;
            case Sender.AllChat:
                PrintToChat(Receiver.Server, player, cmd.Message);
                break;
            case Sender.ClientCenter:
                PrintToCenterClient(player, cmd);
                break;
            case Sender.AllCenter:
                PrintToAllCenter(cmd);
                break;
            case Sender.ClientChatClientCenter:
                PrintToChatAndCenter(Receiver.Client, player, cmd);
                break;
            case Sender.ClientChatAllCenter:
                PrintToChatAndAllCenter(Receiver.Client, player, cmd);
                break;
            case Sender.AllChatClientCenter:
                PrintToChatAndCenter(Receiver.Server, player, cmd);
                break;
            case Sender.AllChatAllCenter:
                PrintToChatAndAllCenter(Receiver.Server, player, cmd);
                break;
            default:
                break;
        }
    }

    private string[] WrappedLine(dynamic input)
    {
        List<string> output = new List<string>();

        if (input is JsonElement jsonElement)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.String:
                    string result = jsonElement.GetString()!;
                    return result?.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None) ?? Array.Empty<string>();

                case JsonValueKind.Array:
                    foreach (var arrayElement in jsonElement.EnumerateArray())
                    {
                        string[] lines = arrayElement.GetString()?.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None) ?? Array.Empty<string>();
                        output.AddRange(lines);
                    }
                    break;

                default:
                    Logger.LogError($"{Config.LogPrefix} Message is not a string or array");
                    return Array.Empty<string>();
            }
        }
        else
        {
            Logger.LogError($"{Config.LogPrefix} Invalid input type");
            return Array.Empty<string>();
        }

        return output.ToArray();
    }
}