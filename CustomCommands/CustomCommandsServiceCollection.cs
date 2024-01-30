
using CounterStrikeSharp.API.Core;
using CustomCommands.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CustomCommands;

public class CustomCommandsServiceCollection : IPluginServiceCollection<CustomCommands>
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<IRegisterCommands>()
            .AddClasses()
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );
    }
}