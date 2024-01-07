using CounterStrikeSharp.API.Core;

namespace CustomCommands.Interfaces;

public interface IEventManager
{
    HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo _);
    void RegisterListeners();
}