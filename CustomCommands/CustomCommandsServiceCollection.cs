
using CounterStrikeSharp.API.Core;
using CustomCommands.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CustomCommands;

public class CustomCommandsServiceCollection : IPluginServiceCollection<CustomCommands>
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Scans the interface in the CustomCommands.Interfaces namespace and adds the classes that implement the interface to the service collection automatically
        services.Scan(scan => scan
            .FromAssemblyOf<IRegisterCommands>()
            .AddClasses()
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );
    }
}