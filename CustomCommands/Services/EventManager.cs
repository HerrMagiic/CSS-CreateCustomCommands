using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Plugin;
using CustomCommands.Interfaces;

namespace CustomCommands.Services;

public class EventManager : IEventManager
{
    private readonly IPluginGlobals _pluginGlobals;
    private readonly PluginContext _pluginContext;
    private readonly IReplaceTagsFunctions _replaceTagsFunctions;
    
    public EventManager(IPluginGlobals PluginGlobals, IPluginContext PluginContext, IReplaceTagsFunctions ReplaceTagsFunctions)
    {
        _pluginGlobals          = PluginGlobals;
        _pluginContext          = (PluginContext as PluginContext)!;
        _replaceTagsFunctions   = ReplaceTagsFunctions;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo _)
    {
        _pluginGlobals.centerClientOn.RemoveAll(p => p.ClientId == @event.Userid?.UserId);
        _pluginGlobals.CooldownTimer.RemoveAll(p => p.PlayerID == @event.Userid?.UserId);

        return HookResult.Continue;
    }

    public void RegisterListeners()
    {
        var plugin = (_pluginContext.Plugin as CustomCommands)!;

        // Register the OnTick event for PrintToCenterHtml duration
        plugin.RegisterListener<Listeners.OnTick>(() =>
        {
            // Client Print To Center
            if(_pluginGlobals.centerClientOn != null && _pluginGlobals.centerClientOn.Count !> 0 )
            {
                foreach (var player in _pluginGlobals.centerClientOn)
                {
                    var targetPlayer = Utilities.GetPlayerFromUserid(player.ClientId);
                    if (player != null && targetPlayer != null)
                    {
                        targetPlayer.PrintToCenterHtml(player.Message, 1);
                    }
                }
            }
            
            // Server Print To Center
            if (_pluginGlobals.centerServerOn.IsRunning)
            {
                Utilities.GetPlayers().ForEach(controller =>
                {
                    if (!controller.IsValid || controller.SteamID == 0) 
                        return;
                    
                    var message = _replaceTagsFunctions.ReplaceLanguageTags(_pluginGlobals.centerServerOn.Message);
                    message = _replaceTagsFunctions.ReplaceMessageTags(message, controller);
                    controller.PrintToCenterHtml(message, 1);
                });
            }
        });

        plugin.RegisterListener<Listeners.OnMapEnd>(() =>
        {
            _pluginGlobals.centerClientOn.Clear();
            _pluginGlobals.CooldownTimer.Clear();
        });
    }
}