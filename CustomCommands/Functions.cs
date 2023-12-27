using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
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
    private string[] WrappedLine(string input)
    {
        return input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    }

    private string ReplaceMessageTags(string input, CCSPlayerController player)
    {
        SteamID steamId = new SteamID((ulong?)player.UserId!.Value ?? 0);

        Dictionary<string, string> replacements = new()
        {
            {"{PREFIX}", PrefixCache},
            {"{MAP}", NativeAPI.GetMapName()},
            {"{TIME}", DateTime.Now.ToString("HH:mm:ss")},
            {"{DATE}", DateTime.Now.ToString("dd.MM.yyyy")},
            {"{PLAYERNAME}", player.PlayerName},
            {"{STEAMID2}", steamId.SteamId2},
            {"{STEAMID3}", steamId.SteamId3},
            {"{STEAMID32}", steamId.SteamId32.ToString()},
            {"{STEAMID64}", steamId.SteamId64.ToString()},
            {"{SERVERNAME}", ConVar.Find("hostname")!.StringValue},
            {"{IP}", ConVar.Find("ip")!.StringValue},
            {"{PORT}", ConVar.Find("hostport")!.GetPrimitiveValue<int>().ToString()},
            {"{MAXPLAYERS}", Server.MaxPlayers.ToString()},
            {"{PLAYERS}",
                Utilities.GetPlayers().Count(u => u.PlayerPawn.Value != null && u.PlayerPawn.Value.IsValid).ToString()}
        };

        foreach (var pair in replacements)
            input = input.Replace(pair.Key, pair.Value);

        return input;
    }

    private string ReplaceColorTags(string input)
    {
        Dictionary<string, string> replacements = new()
        {
            {"{DEFAULT}", "\u0001"},
            {"{WHITE}", "\u0001"},
            {"{DARKRED}", "\u0002"},
            {"{RED}", "\x03"},
            {"{LIGHTRED}", "\u000f"},
            {"{GREEN}", "\u0004"},
            {"{LIME}", "\u0006"},
            {"{OLIVE}", "\u0005"},
            {"{ORANGE}", "\u0010"},
            {"{GOLD}", "\u0010"},
            {"{YELLOW}", "\t"},
            {"{BLUE}", "\v"},
            {"{DARKBLUE}", "\f"},
            {"{LIGHTPURPLE}", "\u0003"},
            {"{PURPLE}", "\u000e"},
            {"{SILVER}", $"{ChatColors.Silver}"},
            {"{BLUEGREY}", "\x0A"},
            {"{GREY}", "\x08"},
        };

        foreach (var pair in replacements)
            input = input.Replace(pair.Key, pair.Value);

        return input;
    }
}