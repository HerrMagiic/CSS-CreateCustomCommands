using CounterStrikeSharp.API.Core;

namespace CustomCommands.Services;

public interface IEventManager
{
    HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo _);
    void RegisterListeners();
}