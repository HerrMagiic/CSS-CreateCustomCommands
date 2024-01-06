
using CounterStrikeSharp.API.Core;
using CustomCommands.Controller;
using CustomCommands.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CustomCommands;

public class CustomCommandsServiceCollection : IPluginServiceCollection<CustomCommands>
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IMessageManager, MessageManager>();
        services.AddSingleton<IReplaceTagsFunctions, ReplaceTagsFunctions>();
        services.AddSingleton<IRegisterCommands, RegisterCommands>();
        services.AddSingleton<IPluginGlobals, PluginGlobals>();
        services.AddSingleton<ILoadJson, LoadJson>();
        services.AddSingleton<IPermissionsManager, PermissionsManager>();
        services.AddSingleton<IEventManager, EventManager>();
    }
}