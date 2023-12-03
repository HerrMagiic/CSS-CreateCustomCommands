using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CustomCommands.Model;
using System.Text.Json;

namespace CustomCommands
{
    [MinimumApiVersion(98)]
    public class CustomCommands : BasePlugin, IPluginConfig<CustomCommandsConfig>
    {
        public override string ModuleName => "CustomCommands";

        public override string ModuleVersion => "1.0.1";

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

            var json = File.ReadAllText(Path.Combine(ModuleDirectory, "Commands.json"));
            var comms = JsonSerializer.Deserialize<List<Commands>>(json);

            if (comms != null)
            {
                foreach (var com in comms)
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
            }
            else
                Console.WriteLine("No Config file found. Please create one");

            if (hotReload)
                InitializeLists();
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
                    foreach (var controller in PlayerList)
                        controller.PrintToCenterHtml(cmd.CenterMessage, cmd.CenterMessageTime);

                    break;
                case Sender.ClientChatClientCenter:
                    PrintToChat(Receiver.Client, player, cmd.Message);
                    player.PrintToCenterHtml(cmd.CenterMessage, cmd.CenterMessageTime);

                    break;
                case Sender.ClientChatAllCenter:
                    PrintToChat(Receiver.Client, player, cmd.Message);
                    foreach (var controller in PlayerList)
                        controller.PrintToCenterHtml(cmd.CenterMessage, cmd.CenterMessageTime);

                    break;
                case Sender.AllChatClientCenter:
                    PrintToChat(Receiver.Server, player, cmd.Message);
                    player.PrintToCenterHtml(cmd.CenterMessage, cmd.CenterMessageTime);

                    break;
                case Sender.AllChatAllCenter:
                    PrintToChat(Receiver.Server, player, cmd.Message);
                    foreach (var controller in PlayerList)
                        controller.PrintToCenterHtml(cmd.CenterMessage, cmd.CenterMessageTime);

                    break;
                default:
                    break;
            }
        }
        
        
        private void PrintToChat(Receiver printToChat, CCSPlayerController player, string message)
        {
            string[] msg = ReplaceTags(message).Split("\\n");

            switch (printToChat)
            {
                case Receiver.Client:
                    foreach (var line in msg)
                        player.PrintToChat(line);
                    break;
                case Receiver.Server:
                    foreach (var line in msg)
                        Server.PrintToChatAll(line);
                    break;
                default:
                    break;
            }
        }

        private string ReplaceTags(string input)
        {
            string[] patterns =
            {
                "{PREFIX}", "{DEFAULT}", "{RED}", "{LIGHTPURPLE}", "{GREEN}", "{LIME}", "{LIGHTGREEN}", "{LIGHTRED}", "{GRAY}",
                "{LIGHTOLIVE}", "{OLIVE}", "{LIGHTBLUE}", "{BLUE}", "{PURPLE}", "{GRAYBLUE}"
            };
            string[] replacements =
            {
                PrefixCache ,"\x01", "\x02", "\x03", "\x04", "\x05", "\x06", "\x07", "\x08", "\x09", "\x10", "\x0B", "\x0C", "\x0E",
                "\x0A"
            };

            for (var i = 0; i < patterns.Length; i++)
                input = input.Replace(patterns[i], replacements[i]);

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
}