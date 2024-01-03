using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CustomCommands;
public partial class CustomCommands
{
    private string[] ReplaceTags(string[] input, CCSPlayerController player)
    {
        string[] output = new string[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            output[i] = ReplaceMessageTags(input[i], player);
            output[i] = ReplaceColorTags(output[i]);
        }

        return output;
    }

    private string ReplaceMessageTags(string input, CCSPlayerController player)
    {
        SteamID steamId = new SteamID(player.SteamID);

        Dictionary<string, string> replacements = new()
        {
            {"{PREFIX}", PrefixCache ?? "<PREFIX not found>"},
            {"{MAP}", NativeAPI.GetMapName() ?? "<MAP not found>"},
            {"{TIME}", DateTime.Now.ToString("HH:mm:ss") ?? "<TIME not found>"},
            {"{DATE}", DateTime.Now.ToString("dd.MM.yyyy") ?? "<DATE not found>"},
            {"{PLAYERNAME}", player.PlayerName ?? "<PLAYERNAME not found>"},
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

    private string ReplaceColorTags(string input)
    {
        Dictionary<string, string> replacements = new()
        {
            {"{DEFAULT}", "\u0001"},
            {"{WHITE}", "\u0001"},
            {"{DARKRED}", "\u0002"},
            {"{RED}", "\x03"},
            {"{LIGHTRED}", "\u000f"},
            {"{GREEN}", "\u0004"},
            {"{LIME}", "\u0006"},
            {"{OLIVE}", "\u0005"},
            {"{ORANGE}", "\u0010"},
            {"{GOLD}", "\u0010"},
            {"{YELLOW}", "\t"},
            {"{BLUE}", "\v"},
            {"{DARKBLUE}", "\f"},
            {"{LIGHTPURPLE}", "\u0003"},
            {"{PURPLE}", "\u000e"},
            {"{SILVER}", $"{ChatColors.Silver}"},
            {"{BLUEGREY}", "\x0A"},
            {"{GREY}", "\x08"},
        };

        foreach (var pair in replacements)
            input = input.Replace(pair.Key, pair.Value);

        return input;
    }
}