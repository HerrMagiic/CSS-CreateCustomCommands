using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface IMessageManager
{
    void SendMessage(CCSPlayerController player, Commands cmd);
    void PrintToCenterClient(CCSPlayerController player, Commands cmd);
    void PrintToAllCenter(Commands cmd);
    void PrintToChatAndCenter(Receiver receiver, CCSPlayerController player, Commands cmd);
    void PrintToChatAndAllCenter(Receiver receiver, CCSPlayerController player, Commands cmd);
    void PrintToChat(Receiver printToChat, CCSPlayerController player, dynamic message);
    void PrintToChatClient(CCSPlayerController player, string[] msg);
    void PrintToChatServer(string[] msg);
}
