
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace CustomCommands;
public partial class CustomCommands
{
    [GameEventHandler(HookMode.Post)]
    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo _)
    {
        if (!PlayerList.Contains(@event.Userid))
            PlayerList.Add(@event.Userid);

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo _)
    {
        centerClientOn.RemoveAll(p => p.ClientId == @event.Userid.UserId);

        return HookResult.Continue;
    }
}