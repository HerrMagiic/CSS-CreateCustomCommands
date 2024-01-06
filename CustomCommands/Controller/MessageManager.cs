using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Plugin;
using CustomCommands.Model;
using CustomCommands.Services;

namespace CustomCommands.Controller;
public class MessageManager : IMessageManager
{
    private readonly IPluginGlobals PluginGlobals;
    private readonly IReplaceTagsFunctions ReplaceTagsFunctions;
    private readonly CustomCommands PluginContext;

    public MessageManager(IPluginGlobals PluginGlobals, IReplaceTagsFunctions ReplaceTagsFunctions, IPluginContext PluginContext)
    {
        this.PluginGlobals = PluginGlobals;
        this.ReplaceTagsFunctions = ReplaceTagsFunctions;
        this.PluginContext = ((PluginContext as PluginContext)!.Plugin as CustomCommands)!;
    }

    public void SendMessage(CCSPlayerController player, Commands cmd) 
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

    public void PrintToCenterClient(CCSPlayerController player, Commands cmd)
    {
        string message = ReplaceTagsFunctions.ReplaceMessageTags(cmd.CenterMessage.Message, player);

        var CenterClientElement = new CenterClientElement
        {
            ClientId = player.UserId!.Value,
            Message = message
        };
        PluginGlobals.centerClientOn.Add(CenterClientElement);
        PluginContext.AddTimer(cmd.CenterMessage.Time, () => PluginGlobals.centerClientOn.Remove(CenterClientElement));
    }

    public void PrintToAllCenter(Commands cmd)
    {
        PluginGlobals.centerServerOn.Message = cmd.CenterMessage.Message;
        PluginGlobals.centerServerOn.IsRunning = true;
        
        PluginContext.AddTimer(cmd.CenterMessage.Time, () =>
        {
            PluginGlobals.centerServerOn.IsRunning = false;
        });
    }

    public void PrintToChatAndCenter(Receiver receiver, CCSPlayerController player, Commands cmd)
    {
        PrintToChat(receiver, player, cmd.Message);
        PrintToCenterClient(player, cmd);
    }

    public void PrintToChatAndAllCenter(Receiver receiver, CCSPlayerController player, Commands cmd)
    {
        PrintToChat(receiver, player, cmd.Message);
        PrintToAllCenter(cmd);
    }
    
    public void PrintToChat(Receiver printToChat, CCSPlayerController player, dynamic message)
    {
        string[] msg = ReplaceTagsFunctions.WrappedLine(message);
        msg = ReplaceTagsFunctions.ReplaceTags(msg, player);

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

    public void PrintToChatClient(CCSPlayerController player, string[] msg)
    {
        foreach (var line in msg)
            player.PrintToChat(line);
    }

    public void PrintToChatServer(string[] msg)
    {
        foreach (var line in msg)
            Server.PrintToChatAll(line);
    }
}