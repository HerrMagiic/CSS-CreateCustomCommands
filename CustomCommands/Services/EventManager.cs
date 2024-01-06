
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Plugin;
using CustomCommands.Interfaces;

namespace CustomCommands.Services;

public class EventManager : IEventManager
{
    private readonly IPluginGlobals PluginGlobals;
    private readonly CustomCommands PluginContext;
    private readonly IReplaceTagsFunctions ReplaceTagsFunctions;
    
    public EventManager(IPluginGlobals PluginGlobals, IPluginContext PluginContext, IReplaceTagsFunctions ReplaceTagsFunctions)
    {
        this.PluginGlobals = PluginGlobals;
        this.PluginContext = ((PluginContext as PluginContext)!.Plugin as CustomCommands)!;
        this.ReplaceTagsFunctions = ReplaceTagsFunctions;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo _)
    {
        PluginGlobals.centerClientOn.RemoveAll(p => p.ClientId == @event.Userid.UserId);

        return HookResult.Continue;
    }

    public void RegisterListeners()
    {
        PluginContext.RegisterListener<Listeners.OnTick>(() =>
        {
            // Client Print To Center
            if(PluginGlobals.centerClientOn != null && PluginGlobals.centerClientOn.Count !> 0 )
            {
                foreach (var player in PluginGlobals.centerClientOn)
                {
                    var targetPlayer = Utilities.GetPlayerFromUserid(player.ClientId);
                    if (player != null && targetPlayer != null)
                    {
                        targetPlayer.PrintToCenterHtml(player.Message, 1);
                    }
                }
            }
            
            // Server Print To Center
            if (PluginGlobals.centerServerOn.IsRunning)
            {
                Utilities.GetPlayers().ForEach(controller =>
                {
                    if (controller == null || !controller.IsValid) return;

                    string message = ReplaceTagsFunctions.ReplaceMessageTags(PluginGlobals.centerServerOn.Message, controller);
                    controller.PrintToCenterHtml(message, 1);
                });
            }
        });
    }
}