using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Plugin;
using CustomCommands.Interfaces;
using CustomCommands.Model;

namespace CustomCommands.Services;
public class MessageManager : IMessageManager
{
    private readonly IPluginGlobals _pluginGlobals;
    private readonly IReplaceTagsFunctions _replaceTagsFunctions;
    private readonly PluginContext _pluginContext;

    public MessageManager(IPluginGlobals PluginGlobals, IReplaceTagsFunctions ReplaceTagsFunctions, IPluginContext PluginContext)
    {
        _pluginGlobals = PluginGlobals;
        _replaceTagsFunctions = ReplaceTagsFunctions;
        _pluginContext = (PluginContext as PluginContext)!;
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
        var context = (_pluginContext.Plugin as CustomCommands)!;
        var message  = _replaceTagsFunctions.ReplaceLanguageTags(cmd.CenterMessage.Message);
        message         = _replaceTagsFunctions.ReplaceMessageTags(message, player);

        var CenterClientElement = new CenterClientElement
        {
            ClientId = player.UserId!.Value,
            Message = message
        };
        _pluginGlobals.centerClientOn.Add(CenterClientElement);
        context.AddTimer(cmd.CenterMessage.Time, () => _pluginGlobals.centerClientOn.Remove(CenterClientElement));
    }

    public void PrintToAllCenter(Commands cmd)
    {
        var context = (_pluginContext.Plugin as CustomCommands)!;
        _pluginGlobals.centerServerOn.Message    = cmd.CenterMessage.Message;
        _pluginGlobals.centerServerOn.IsRunning  = true;
        
        context.AddTimer(cmd.CenterMessage.Time, () =>
        {
            _pluginGlobals.centerServerOn.IsRunning = false;
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
        var msg = _replaceTagsFunctions.ReplaceTags(message, player);

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