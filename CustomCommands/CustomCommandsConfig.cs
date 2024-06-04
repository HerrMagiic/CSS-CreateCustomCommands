using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json.Serialization;

namespace CustomCommands;

public class CustomCommandsConfig : BasePluginConfig
{
    public override int Version { get; set; } = 2;

    [JsonPropertyName("IsPluginEnabled")]
    public bool IsPluginEnabled { get; set; } = true;

    [JsonPropertyName("LogPrefix")]
    public string LogPrefix { get; set; } = "CSSharp";

    [JsonPropertyName("Prefix")]
    public string Prefix { get; set; } = $"[{ChatColors.Yellow}Info{ChatColors.Default}] ";

    [JsonPropertyName("RegisterCommandsAsCSSFramework")]
    public bool RegisterCommandsAsCSSFramework { get; set; } = true;
}