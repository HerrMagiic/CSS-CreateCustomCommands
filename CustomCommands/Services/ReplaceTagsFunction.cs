using System.Text.Json;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CustomCommands.Interfaces;
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API.Core.Plugin;

namespace CustomCommands.Services;
public class ReplaceTagsFunctions : IReplaceTagsFunctions
{
    private readonly IPluginGlobals PluginGlobals;
    private readonly PluginContext PluginContext;
    private readonly ILogger<CustomCommands> Logger;
    
    public ReplaceTagsFunctions(IPluginGlobals PluginGlobals, IPluginContext PluginContext, 
                                    ILogger<CustomCommands> Logger)
    {
        this.PluginGlobals = PluginGlobals;
        this.PluginContext = (PluginContext as PluginContext)!;
        this.Logger = Logger;
    }

    public string[] ReplaceTags(string[] input, CCSPlayerController player)
    {
        string[] output = new string[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            output[i] = ReplaceLanguageTags(input[i]);
            output[i] = ReplaceMessageTags(output[i], player);
            output[i] = ReplaceColorTags(output[i]);
        }

        return output;
    }

    public string ReplaceLanguageTags(string input)
    {
        CustomCommands plugin = (PluginContext.Plugin as CustomCommands)!;

        // Define the regex pattern to find "{LANG=...}"
        string pattern = @"\{LANG=(.*?)\}";

        // Use Regex to find matches
        Match match = Regex.Match(input, pattern);

        // Check if a match is found
        if (match.Success)
        {
            // Return the group captured in the regex, which is the string after "="
            string lang = match.Groups[1].Value;
            return input.Replace(match.Value, plugin.Localizer[lang] ?? "<LANG in CustomCommands/lang/<language.json> not found>");
        }
        else
        {
            // Return the original string if no match is found
            return input;
        }
    }
    public string ReplaceMessageTags(string input, CCSPlayerController player)
    {
        SteamID steamId = new SteamID(player.SteamID);
        
        Dictionary<string, string> replacements = new()
        {
            {"{PREFIX}", PluginGlobals.Config.Prefix ?? "<PREFIX not found>"},
            {"{MAP}", NativeAPI.GetMapName() ?? "<MAP not found>"},
            {"{TIME}", DateTime.Now.ToString("HH:mm:ss") ?? "<TIME not found>"},
            {"{DATE}", DateTime.Now.ToString("dd.MM.yyyy") ?? "<DATE not found>"},
            {"{PLAYERNAME}", player.PlayerName ?? "<PLAYERNAME not found>"},
            {"{USERID}", player.Slot.ToString() ?? "<USERID not found>"},
            {"{STEAMID2}", steamId.SteamId2 ?? "<STEAMID2 not found>"},
            {"{STEAMID3}", steamId.SteamId3 ?? "<STEAMID3 not found>"},
            {"{STEAMID32}", steamId.SteamId32.ToString() ?? "<STEAMID32 not found>"},
            {"{STEAMID64}", steamId.SteamId64.ToString() ?? "<STEAMID64 not found>"},
            {"{SERVERNAME}", ConVar.Find("hostname")!.StringValue ?? "<SERVERNAME not found>"},
            {"{IP}", ConVar.Find("ip")!.StringValue ?? "<IP not found>"},
            {"{PORT}", ConVar.Find("hostport")!.GetPrimitiveValue<int>().ToString() ?? "<PORT not found>"},
            {"{MAXPLAYERS}", Server.MaxPlayers.ToString() ?? "<MAXPLAYERS not found>"},
            {"{PLAYERS}",
                Utilities.GetPlayers().Count(u => u.PlayerPawn.Value != null && u.PlayerPawn.Value.IsValid).ToString() ?? "<PLAYERS not found>"}
        };

        foreach (var pair in replacements)
            input = input.Replace(pair.Key, pair.Value);

        return input;
    }

    public string ReplaceColorTags(string input)
    {
        // PadLeft the color tag if it's not already there because the game doesn't support color tags at the start of a string.
        input = PadLeftColorTag(input);

        Dictionary<string, string> replacements = new()
        {
            {"{DEFAULT}", $"{ChatColors.Default}"},
            {"{WHITE}", $"{ChatColors.White}"},
            {"{DARKRED}", $"{ChatColors.Darkred}"},
            {"{RED}", $"{ChatColors.Red}"},
            {"{LIGHTRED}", $"{ChatColors.LightRed}"},
            {"{GREEN}", $"{ChatColors.Green}"},
            {"{LIME}", $"{ChatColors.Lime}"},
            {"{OLIVE}", $"{ChatColors.Olive}"},
            {"{ORANGE}", $"{ChatColors.Orange}"},
            {"{GOLD}", $"{ChatColors.Gold}"},
            {"{YELLOW}", $"{ChatColors.Yellow}"},
            {"{BLUE}", $"{ChatColors.Blue}"},
            {"{DARKBLUE}", $"{ChatColors.DarkBlue}"},
            {"{LIGHTPURPLE}", $"{ChatColors.LightPurple}"},
            {"{PURPLE}", $"{ChatColors.Purple}"},
            {"{SILVER}", $"{ChatColors.Silver}"},
            {"{BLUEGREY}", $"{ChatColors.BlueGrey}"},
            {"{GREY}", $"{ChatColors.Grey}"},
        };
        
        foreach (var pair in replacements)
            input = input.Replace(pair.Key, pair.Value);

        return input;
    }

    /// <summary>
    /// Splits the input into an array of strings. If the input is a string, it will be split by newlines. If the input is an array, each element will be split by newlines.
    /// </summary>
    /// <param name="input">This should be a string[] or a string</param>
    /// <returns>An array of strings representing the lines of the input.</returns>
    public string[] WrappedLine(dynamic input)
    {
        List<string> output = new List<string>();

        if (input is JsonElement jsonElement)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.String:
                    string result = jsonElement.GetString()!;
                    return result?.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None) ?? Array.Empty<string>();

                case JsonValueKind.Array:
                    foreach (var arrayElement in jsonElement.EnumerateArray())
                    {
                        string[] lines = arrayElement.GetString()?.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None) ?? Array.Empty<string>();
                        output.AddRange(lines);
                    }
                    break;

                default:
                    Logger.LogError($"{PluginGlobals.Config.LogPrefix} Message is not a string or array");
                    return Array.Empty<string>();
            }
        }
        else
        {
            Logger.LogError($"{PluginGlobals.Config.LogPrefix} Invalid input type");
            return Array.Empty<string>();
        }

        return output.ToArray();
    }

    /// <summary>
    /// Moves the color tag one space the the left if it's not already there.
    /// Because the game doesn't support color tags at the start of a string.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string PadLeftColorTag(string input)
    {
        string[] colorTagList = new string[] {
            "{DEFAULT}",
            "{WHITE}",
            "{DARKRED}",
            "{RED}",
            "{LIGHTRED}",
            "{GREEN}",
            "{LIME}",
            "{OLIVE}",
            "{ORANGE}",
            "{GOLD}",
            "{YELLOW}",
            "{BLUE}",
            "{DARKBLUE}",
            "{LIGHTPURPLE}",
            "{PURPLE}",
            "{SILVER}",
            "{BLUEGREY}",
            "{GREY}",
        };
        foreach (var colorTag in colorTagList)
        {
            if (input.StartsWith(colorTag))
            {
                return " " + input;
            }
        }
        return input;
    }
}