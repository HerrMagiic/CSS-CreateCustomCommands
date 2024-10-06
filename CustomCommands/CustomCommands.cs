using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CustomCommands.Interfaces;
using Microsoft.Extensions.Logging;

namespace CustomCommands;

[MinimumApiVersion(213)]
public partial class CustomCommands : BasePlugin, IPluginConfig<CustomCommandsConfig>
{
    public override string ModuleName => "CustomCommands";
    public override string ModuleVersion => "3.0.0";
    public override string ModuleAuthor => "HerrMagic";
    public override string ModuleDescription => "Create your own commands per config";

    public CustomCommandsConfig Config { get; set; } = new();
    private readonly IRegisterCommands _registerCommands;
    private readonly IPluginGlobals _pluginGlobals;
    private readonly ILoadJson _loadJson;
    private readonly IEventManager _eventManager;
    private readonly IReplaceTagsFunctions _replaceTagsFunctions;

    public CustomCommands(IRegisterCommands RegisterCommands, ILogger<CustomCommands> Logger, 
                            IPluginGlobals PluginGlobals, ILoadJson LoadJson, IEventManager EventManager, IReplaceTagsFunctions ReplaceTagsFunctions)
    {
        this.Logger = Logger;
        _registerCommands = RegisterCommands;
        _pluginGlobals = PluginGlobals;
        _loadJson = LoadJson;
        _eventManager = EventManager;
        _replaceTagsFunctions = ReplaceTagsFunctions;
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

        _pluginGlobals.Config = Config;
        Config.Prefix = _replaceTagsFunctions.ReplaceColorTags(Config.Prefix);

        var comms = Task.Run(async () => await _loadJson.GetCommandsFromJsonFiles(ModuleDirectory)).Result;

        if (comms == null)
        {
            Logger.LogError("No commands found please create a config file");
            return;
        }
        
        _eventManager.RegisterListeners();

        if (comms != null) 
        {
            _pluginGlobals.CustomCommands = comms;

            _registerCommands.CheckForDuplicateCommands();
            _registerCommands.ConvertingCommandsForRegister();

            // Add commands from the JSON file to the server
            foreach (var cmd in _pluginGlobals.CustomCommands)
                _registerCommands.AddCommands(cmd);
        }
    }
}