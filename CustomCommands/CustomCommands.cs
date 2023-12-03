using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CustomCommands.Model;
using System.Text.Json;

namespace CustomCommands
{
    [MinimumApiVersion(86)]
    public class CustomCommands : BasePlugin, IPluginConfig<CustomCommandsConfig>
    {
        public override string ModuleName => "CustomCommands";

        public override string ModuleVersion => "1.0.1";

        public override string ModuleAuthor => "HerrMagic";

        public override string ModuleDescription => "Create your own commands per config";

        private List<CCSPlayerController> PlayerList = new();

        public CustomCommandsConfig Config { get; set; } = new();

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


            var json = System.IO.File.ReadAllText(Path.Combine(ModuleDirectory, "Commands.json"));
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

                            string message = ReplaceTags(com.Message);

                            switch (com.PrintTo)
                            {
                                case Sender.ClientChat:
                                    player.PrintToChat(message);

                                    break;
                                case Sender.AllChat:
                                    Server.PrintToChatAll(message);

                                    break;
                                case Sender.ClientCenter:
                                    player.PrintToCenterHtml(com.CenterMessage);

                                    break;
                                case Sender.AllCenter:
                                    foreach (var controller in PlayerList)
                                        controller.PrintToCenterHtml(com.CenterMessage);

                                    break;
                                case Sender.ClientChatClientCenter:
                                    player.PrintToChat(message);
                                    player.PrintToCenterHtml(com.CenterMessage);

                                    break;
                                case Sender.ClientChatAllCenter:
                                    player.PrintToChat(message);
                                    foreach (var controller in PlayerList)
                                        controller.PrintToCenterHtml(com.CenterMessage);

                                    break;
                                case Sender.AllChatClientCenter:
                                    Server.PrintToChatAll(message);
                                    player.PrintToCenterHtml(com.CenterMessage);

                                    break;
                                case Sender.AllChatAllCenter:
                                    Server.PrintToChatAll(message);
                                    foreach (var controller in PlayerList)
                                        controller.PrintToCenterHtml(com.CenterMessage);

                                    break;
                                default:
                                    break;
                            }
                        });
                    }
                }
            }
            else
                Console.WriteLine("No Config file found. Please create one");

            if (hotReload)
                InitializeLists();
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
                ReplaceTags(Config.Prefix) ,"\x01", "\x02", "\x03", "\x04", "\x05", "\x06", "\x07", "\x08", "\x09", "\x10", "\x0B", "\x0C", "\x0E",
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