using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CustomCommands.Interfaces;
using Microsoft.Extensions.Logging;

namespace CustomCommands;

[MinimumApiVersion(142)]
public partial class CustomCommands : BasePlugin, IPluginConfig<CustomCommandsConfig>
{
    public override string ModuleName => "CustomCommands";
    public override string ModuleVersion => "1.0.8";
    public override string ModuleAuthor => "HerrMagic";
    public override string ModuleDescription => "Create your own commands per config";

    public CustomCommandsConfig Config { get; set; } = new();
    private readonly IRegisterCommands RegisterCommands;
    private readonly IPluginGlobals PluginGlobals;
    private readonly ILoadJson LoadJson;
    private readonly IEventManager EventManager;

    public CustomCommands(IRegisterCommands RegisterCommands, ILogger<CustomCommands> Logger, 
                            IPluginGlobals PluginGlobals, ILoadJson LoadJson, IEventManager EventManager)
    {
        this.Logger = Logger;
        this.RegisterCommands = RegisterCommands;
        this.PluginGlobals = PluginGlobals;
        this.LoadJson = LoadJson;
        this.EventManager = EventManager;
    }

    public void OnConfigParsed(CustomCommandsConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        if (!Config.IsPluginEnabled)
        {
            Logger.LogInformation($"{Config.LogPrefix} {ModuleName} is disabled");
            return;
        }
        
        Logger.LogInformation(
            $"{ModuleName} loaded!");

        PluginGlobals.Config = Config;

        var comms = LoadJson.LoadCommandsFromJson(ModuleDirectory);

        EventManager.RegisterListeners();

        if (comms != null) 
        {
            comms = RegisterCommands.CheckForDuplicateCommands(comms);
            // Add commands from the JSON file to the server
            foreach (var com in comms)
                RegisterCommands.AddCommands(com);
        }
    }
}