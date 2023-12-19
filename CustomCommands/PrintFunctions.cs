using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands;
public partial class CustomCommands
{
    private void PrintToCenterClient(CCSPlayerController player, Commands cmd)
    {
        var CenterClientElement = new CenterClientElement
        {
            ClientId = player.UserId!.Value,
            Message = cmd.CenterMessage.Message
        };
        centerClientOn.Add(CenterClientElement);
        AddTimer(cmd.CenterMessage.Time, () => centerClientOn.Remove(CenterClientElement));
    }
    private void PrintToAllCenter(Commands cmd)
    {
        centerServerOn.Message = cmd.CenterMessage.Message;
        centerServerOn.IsRunning = true;
        AddTimer(cmd.CenterMessage.Time, () =>
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
}