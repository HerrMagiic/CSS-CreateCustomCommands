using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CustomCommands.Model;

namespace CustomCommands;
public partial class CustomCommands
{
    private void RegisterListeners()
    {
        RegisterListener<Listeners.OnTick>(() =>
        {
            // Client Print To Center
            foreach (var player in centerClientOn)
            {
                Utilities.GetPlayerFromUserid(player.ClientId).PrintToCenterHtml(player.Message, 1);
            }
            if (centerServerOn.IsRunning)
            {
                Utilities.GetPlayers().ForEach(controller =>
                {
                    if (controller == null || !controller.IsValid) return;

                    controller.PrintToCenterHtml(centerServerOn.Message, 1);
                });
            }

        });
    }

    private void AddCommands(Commands com)
    {
        string[] aliases = com.Command.Split(',');

        for (int i = 0; i < aliases.Length; i++)
        {
            AddCommand(aliases[i], com.Description, (player, info) =>
            {
                if (player == null) return;
                
                if (com.Permission.PermissionList.Count >= 1)
                    if (!RequiresPermissions(player, com.Permission)) 
                        return;
                
                TriggerMessage(player, com);
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
            PrintToChat(Receiver.Client, player, "You don't have the required permissions to execute this command");
            return false;
        }
        else
        {
            if (!AdminManager.PlayerHasPermissions(player, permissions.PermissionList.ToArray()))
            {
                PrintToChat(Receiver.Client, player, "You don't have the required permissions to execute this command");
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
    private void TriggerMessage(CCSPlayerController player, Commands cmd) 
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

    private string ReplaceTags(string input)
    {
        Dictionary<string, string> replacements = new()
        {
            {"{PREFIX}", PrefixCache},
            {"{DEFAULT}", "\x01"},
            {"{RED}", "\x02"},
            {"{LIGHTPURPLE}", "\x03"},
            {"{GREEN}", "\x04"},
            {"{LIME}", "\x05"},
            {"{LIGHTGREEN}", "\x06"},
            {"{LIGHTRED}", "\x07"},
            {"{GRAY}", "\x08"},
            {"{LIGHTOLIVE}", "\x09"},
            {"{OLIVE}", "\x10"},
            {"{LIGHTBLUE}", "\x0B"},
            {"{BLUE}", "\x0C"},
            {"{PURPLE}", "\x0E"},
            {"{GRAYBLUE}", "\x0A"}
        };

        foreach (var pair in replacements)
            input = input.Replace(pair.Key, pair.Value);

        return input;
    }
}