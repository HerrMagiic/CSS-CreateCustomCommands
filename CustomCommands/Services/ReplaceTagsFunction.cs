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
public partial class ReplaceTagsFunctions : IReplaceTagsFunctions
{
    private readonly IPluginGlobals _pluginGlobals;
    private readonly PluginContext _pluginContext;
    private readonly ILogger<CustomCommands> _logger;
    
    public ReplaceTagsFunctions(IPluginGlobals PluginGlobals, IPluginContext PluginContext, 
                                    ILogger<CustomCommands> Logger)
    {
        _pluginGlobals = PluginGlobals;
        _pluginContext = (PluginContext as PluginContext)!;
        _logger = Logger;
    }

    public string[] ReplaceTags(dynamic input, CCSPlayerController player)
    {
        var output = WrappedLine(input);

        for (int i = 0; i < output.Count; i++)
            output[i] = ReplaceLanguageTags(output[i]);

        output = WrappedLine(output.ToArray());

        for (int i = 0; i < output.Count; i++)
        {
            output[i] = ReplaceMessageTags(output[i], player, false);
            output[i] = ReplaceRandomTags(output[i]);
            output[i] = ReplaceColorTags(output[i]);
        }

        return output.ToArray();
    }

    [GeneratedRegex(@"\{LANG=(.*?)\}")]
    private static partial Regex ReplaceLanguageTagsRegex();

    public string ReplaceLanguageTags(string input)
    {
        // Use Regex to find matches
        var match = ReplaceLanguageTagsRegex().Match(input);

        // Check if a match is found
        if (match.Success)
        {
            // Return the group captured in the regex, which is the string after "="
            var lang = match.Groups[1].Value;
            var context = (_pluginContext.Plugin as CustomCommands)!;

            return input.Replace(match.Value, context.Localizer[lang] ?? "<LANG in CustomCommands/lang/<language.json> not found>");
        }
        else
        {
            // Return the original string if no match is found
            return input;
        }
    }
 
    /// <summary>
    /// Use regex to find the RNDNO tag pattern {RNDNO={min, max}}
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"\{RNDNO=\((\d+(?:\.\d+)?),\s*(\d+(?:\.\d+)?)\)\}")]
    private static partial Regex ReplaceRandomTagsRegex();

    public string ReplaceRandomTags(string message)
    {
        // Replace all occurrences of the RNDNO tag in the message
        var match = ReplaceRandomTagsRegex().Match(message);

        // Extract min and max from the regex match groups
        string minStr = match.Groups[1].Value;
        string maxStr = match.Groups[2].Value;

        // Determine if the min and max are integers or floats
        bool isMinFloat = float.TryParse(minStr, out float minFloat);
        bool isMaxFloat = float.TryParse(maxStr, out float maxFloat);

        var random = new Random();

        if (isMinFloat || isMaxFloat)
        {
            // Generate a random float between min and max (inclusive)
            float randomFloat = (float)(random.NextDouble() * (maxFloat - minFloat) + minFloat);
            
            // Determine the maximum precision from the min and max values
            int maxDecimalPlaces = Math.Max(GetDecimalPlaces(minStr), GetDecimalPlaces(maxStr));

            // Use the determined precision to format the float
            message = message.Replace(match.Value, randomFloat.ToString($"F{maxDecimalPlaces}"));
        }
        else
        {
            // Parse as integers
            int min = int.Parse(minStr);
            int max = int.Parse(maxStr);

            // Generate a random integer between min and max (inclusive)
            int randomValue = random.Next(min, max + 1); // max is exclusive, so add 1
            message = message.Replace(match.Value, randomValue.ToString());
        }
        
        return message;
    }

    // Method to get the number of decimal places in a number string
    private static int GetDecimalPlaces(string numberStr)
    {
        int decimalIndex = numberStr.IndexOf('.');
        if (decimalIndex == -1)
        {
            return 0; // No decimal point, return 0
        }
        return numberStr.Length - decimalIndex - 1; // Count digits after the decimal point
    }

    public string ReplaceMessageTags(string input, CCSPlayerController player, bool safety = true)
    {
        var steamId = new SteamID(player.SteamID);
        
        Dictionary<string, string> replacements = new()
        {
            {"{PREFIX}", _pluginGlobals.Config.Prefix ?? "<PREFIX not found>"},
            {"{MAP}", NativeAPI.GetMapName() ?? "<MAP not found>"},
            {"{TIME}", DateTime.Now.ToString("HH:mm:ss") ?? "<TIME not found>"},
            {"{DATE}", DateTime.Now.ToString("dd.MM.yyyy") ?? "<DATE not found>"},
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
                Utilities.GetPlayers().Count(u => u.PlayerPawn.Value != null && u.PlayerPawn.Value.IsValid).ToString() ?? "<PLAYERS not found>"},
        };

        // Prevent vounrability by not replacing {PLAYERNAME} if safety is true/ServerCommands are being executed
        if (!safety)
            replacements.Add("{PLAYERNAME}", player.PlayerName ?? "<PLAYERNAME not found>");

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
            {"{DARKRED}", $"{ChatColors.DarkRed}"},
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

    public List<string> WrappedLine(dynamic input)
    {
        var output = new List<string>();

        if (input is JsonElement jsonElement)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.String:
                    var result = jsonElement.GetString()!;
                    output.AddRange(result.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
                    break;
                case JsonValueKind.Array:
                    foreach (var arrayElement in jsonElement.EnumerateArray())
                    {
                        var lines = arrayElement.GetString()?.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None) ?? Array.Empty<string>();
                        output.AddRange(lines);
                    }
                    break;

                default:
                    _logger.LogError($"{_pluginGlobals.Config.LogPrefix} Message is not a string or array");
                    break;
            }
        } else if (input is Array inputArray)
        {
            foreach (string arrayElement in inputArray)
            {
                var lines = arrayElement.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None) ?? Array.Empty<string>();
                output.AddRange(lines);
            }
        }
        else
        {
            _logger.LogError($"{_pluginGlobals.Config.LogPrefix} Invalid input type");
        }

        return output;
    }

    /// <summary>
    /// Moves the color tag one space the the left if it's not already there.
    /// Because the game doesn't support color tags at the start of a string.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string PadLeftColorTag(string input)
    {
        var colorTagList = new string[] {
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