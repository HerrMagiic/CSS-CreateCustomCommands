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
            {"{PREFIX}", Config.Prefix ?? "<PREFIX not found>"},
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
}