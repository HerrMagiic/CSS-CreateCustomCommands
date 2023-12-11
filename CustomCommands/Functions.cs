using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
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
                TriggerMessage(player, com);
            });
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

    private void PrintToCenterClient(CCSPlayerController player, Commands cmd)
    {
        var CenterClientElement = new CenterClientElement
        {
            ClientId = player.UserId!.Value,
            Message = cmd.CenterMessage
        };
        centerClientOn.Add(CenterClientElement);
        AddTimer(cmd.CenterMessageTime, () => centerClientOn.Remove(CenterClientElement));
    }
    private void PrintToAllCenter(Commands cmd)
    {
        centerServerOn.Message = cmd.CenterMessage;
        centerServerOn.IsRunning = true;
        AddTimer(cmd.CenterMessageTime, () =>
        {
            centerServerOn.IsRunning = false;
        });
    }

    private void PrintToChatAndCenter(Receiver receiver, CCSPlayerController player, Commands cmd)
    {
        PrintToChat(receiver, player, cmd.Message);
        PrintToCenterClient(player, cmd);
    }

    private void PrintToChatAndAllCenter(Receiver receiver, CCSPlayerController player, Commands cmd)
    {
        PrintToChat(receiver, player, cmd.Message);
        PrintToAllCenter(cmd);
    }
    
    private void PrintToChat(Receiver printToChat, CCSPlayerController player, string message)
    {
        string[] msg = ReplaceTags(message).Split('\n');

        switch (printToChat)
        {
            case Receiver.Client:
                PrintToChatClient(player, msg);
                break;
            case Receiver.Server:
                PrintToChatServer(msg);
                break;
            default:
                break;
        }
    }

    private void PrintToChatClient(CCSPlayerController player, string[] msg)
    {
        foreach (var line in msg)
            player.PrintToChat(line);
    }

    private void PrintToChatServer(string[] msg)
    {
        foreach (var line in msg)
            Server.PrintToChatAll(line);
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