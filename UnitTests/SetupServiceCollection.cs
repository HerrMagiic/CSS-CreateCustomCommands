using CustomCommands.Interfaces;

namespace UnitTests;

public partial class UnitTests
{
    private readonly IServiceProvider _sp;
    public UnitTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.Scan(scan => scan
            .FromAssemblyOf<IRegisterCommands>()
            .AddClasses()
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );
        _sp = serviceCollection.BuildServiceProvider();
    }
}
