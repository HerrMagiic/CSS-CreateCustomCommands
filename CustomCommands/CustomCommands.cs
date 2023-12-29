using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CustomCommands;

[MinimumApiVersion(98)]
public partial class CustomCommands : BasePlugin, IPluginConfig<CustomCommandsConfig>
{
    public override string ModuleName => "CustomCommands";
    public override string ModuleVersion => "1.0.7";
    public override string ModuleAuthor => "HerrMagic";
    public override string ModuleDescription => "Create your own commands per config";

    private List<CenterClientElement> centerClientOn = new();
    private CenterServerElement centerServerOn = new();

    private List<CCSPlayerController> PlayerList = new();
    public CustomCommandsConfig Config { get; set; } = new();
    private string PrefixCache = "";

    public void OnConfigParsed(CustomCommandsConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        if (!Config.IsPluginEnabled)
        {
            Console.WriteLine($"{Config.LogPrefix} {ModuleName} is disabled");
            return;
        }

        Console.WriteLine(
            $"CustomCommands has been loaded, and the hot reload flag was {hotReload}, path is {ModulePath}");

        if (Config.Prefix != PrefixCache)
            PrefixCache = Config.Prefix;

        var comms = LoadCommandsFromJson();

        RegisterListeners();

        
        if (comms != null) 
        {
            comms = CheckForDuplicateCommands(comms);
            foreach (var com in comms)
                AddCommands(com);
        }

        if (hotReload)
            InitializeLists();
    }

    private List<Commands>? LoadCommandsFromJson()
    {
        string jsonPath = Path.Combine(ModuleDirectory, "Commands.json");
        if (File.Exists(jsonPath))
        {
            var json = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<List<Commands>>(json);
        }
        else
        {
            Logger.LogWarning("No Config file found. Please create one");
            return null;
        }
    }

    private void InitializeLists()
    {
        Utilities.GetPlayers().ForEach(controller =>
        {
            PlayerList.Add(controller);
        });
    }
}