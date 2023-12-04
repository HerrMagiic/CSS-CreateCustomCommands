using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CustomCommands;

[MinimumApiVersion(98)]
public class CustomCommands : BasePlugin, IPluginConfig<CustomCommandsConfig>
{
    public override string ModuleName => "CustomCommands";
    public override string ModuleVersion => "1.0.3";
    public override string ModuleAuthor => "HerrMagic";
    public override string ModuleDescription => "Create your own commands per config";

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
            PrefixCache = ReplaceTags(Config.Prefix);

        var comms = LoadCommandsFromJson();

        if (comms != null)
        {
            foreach (var com in comms)
            {
                AddCommands(com);
            }
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

    private void AddCommands(Commands com)
    {
        string[] aliases = com.Command.Split(',');

        for (int i = 0; i < aliases.Length; i++)
        {
            AddCommand(aliases[i], com.Description, (player, info) =>
            {
                if (player == null) return;
                TriggerMessage(player, com);
            });
        }
    }
    private void TriggerMessage(CCSPlayerController player, Commands cmd) 
    {
        switch (cmd.PrintTo)
        {
            case Sender.ClientChat:
                PrintToChat(Receiver.Client, player, cmd.Message);
                break;
            case Sender.AllChat:
                PrintToChat(Receiver.Server, player, cmd.Message);
                break;
            case Sender.ClientCenter:
                player.PrintToCenterHtml(cmd.CenterMessage, cmd.CenterMessageTime);
                break;
            case Sender.AllCenter:
                PrintToAllCenter(cmd);
                break;
            case Sender.ClientChatClientCenter:
                PrintToChatAndCenter(Receiver.Client, player, cmd);
                break;
            case Sender.ClientChatAllCenter:
                PrintToChatAndAllCenter(Receiver.Client, player, cmd);
                break;
            case Sender.AllChatClientCenter:
                PrintToChatAndCenter(Receiver.Server, player, cmd);
                break;
            case Sender.AllChatAllCenter:
                PrintToChatAndAllCenter(Receiver.Server, player, cmd);
                break;
            default:
                break;
        }
    }

    private void PrintToAllCenter(Commands cmd)
    {
        foreach (var controller in PlayerList)
            controller.PrintToCenterHtml(cmd.CenterMessage, cmd.CenterMessageTime);
    }

    private void PrintToChatAndCenter(Receiver receiver, CCSPlayerController player, Commands cmd)
    {
        PrintToChat(receiver, player, cmd.Message);
        player.PrintToCenterHtml(cmd.CenterMessage, cmd.CenterMessageTime);
    }

    private void PrintToChatAndAllCenter(Receiver receiver, CCSPlayerController player, Commands cmd)
    {
        PrintToChat(receiver, player, cmd.Message);
        PrintToAllCenter(cmd);
    }
    
    
    private void PrintToChat(Receiver printToChat, CCSPlayerController player, string message)
    {
        string[] msg = ReplaceTags(message).Split('\n');

        switch (printToChat)
        {
            case Receiver.Client:
                PrintToChatClient(player, msg);
                break;
            case Receiver.Server:
                PrintToChatServer(msg);
                break;
            default:
                break;
        }
    }

    private void PrintToChatClient(CCSPlayerController player, string[] msg)
    {
        foreach (var line in msg)
            player.PrintToChat(line);
    }

    private void PrintToChatServer(string[] msg)
    {
        foreach (var line in msg)
            Server.PrintToChatAll(line);
    }

    private string ReplaceTags(string input)
    {
        Dictionary<string, string> replacements = new()
        {
            {"{PREFIX}", PrefixCache},
            {"{DEFAULT}", "\x01"},
            {"{RED}", "\x02"},
            {"{LIGHTPURPLE}", "\x03"},
            {"{GREEN}", "\x04"},
            {"{LIME}", "\x05"},
            {"{LIGHTGREEN}", "\x06"},
            {"{LIGHTRED}", "\x07"},
            {"{GRAY}", "\x08"},
            {"{LIGHTOLIVE}", "\x09"},
            {"{OLIVE}", "\x10"},
            {"{LIGHTBLUE}", "\x0B"},
            {"{BLUE}", "\x0C"},
            {"{PURPLE}", "\x0E"},
            {"{GRAYBLUE}", "\x0A"}
        };

        foreach (var pair in replacements)
            input = input.Replace(pair.Key, pair.Value);

        return input;
    }

    private void InitializeLists()
    {
        Utilities.GetPlayers().ForEach(controller =>
        {
            PlayerList.Add(controller);
        });
    }

    [GameEventHandler(HookMode.Post)]
    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo _)
    {
        if (!PlayerList.Contains(@event.Userid))
            PlayerList.Add(@event.Userid);

        return HookResult.Continue;
    }
}