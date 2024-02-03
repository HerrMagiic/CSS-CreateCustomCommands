using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;

namespace CustomCommands.Interfaces;

public class CooldownManager : ICooldownManager
{
    public IPluginGlobals PluginGlobals { get; }
    public CooldownManager(IPluginGlobals PluginGlobals)
    {
        this.PluginGlobals = PluginGlobals;
    }
    
    /// <summary>
    /// Checks if the command is on cooldown
    /// </summary>
    /// <param name="player"></param>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public bool IsCommandOnCooldown(CCSPlayerController player, Commands cmd)
    {
        // Check global cooldown
        if (IsCommandOnCooldownWithCondition(x => x.IsGlobal == true && x.CommandID == cmd.ID, player, cmd))
            return true;

        // Check player cooldown
        if (IsCommandOnCooldownWithCondition(x => x.PlayerID == player.UserId && x.CommandID == cmd.ID, player, cmd))
            return true;

        return false;
    }

    public bool IsCommandOnCooldownWithCondition(Func<CooldownTimer, bool> predicate, CCSPlayerController player, Commands cmd)
    {
        int index = PluginGlobals.CooldownTimer.FindIndex(x => predicate(x) && x.CooldownTime > DateTime.Now);

        if (index != -1)
        {
            string timeleft = PluginGlobals.CooldownTimer[index].CooldownTime.Subtract(DateTime.Now).TotalSeconds.ToString();
            string message = "";

            // Check if cmd.Cooldown is a Cooldown object
            if (cmd.Cooldown is Cooldown cooldown)
                message = cooldown.CooldownMessage.Replace("{TIMELEFT}", timeleft);
            else
                message = $"This command is for {timeleft} seconds on cooldown";

            player.PrintToChat($"{PluginGlobals.Config.Prefix}{message}");

            return true;
        }

        return false;
    }
    
    /// <summary>
    /// Adds the command to the cooldown list
    /// </summary>
    /// <param name="isGlobal"></param>
    /// <param name="playerID"></param>
    /// <param name="commandID"></param>
    /// <param name="cooldownTime"></param>
    public void AddToCooldownList(bool isGlobal, int playerID, Guid commandID, int cooldownTime)
    {
        var timer = new CooldownTimer() {
            IsGlobal = isGlobal, 
            CommandID = commandID, 
            CooldownTime = DateTime.Now.AddSeconds(cooldownTime)
        };

        if (isGlobal)
        {
            int index = PluginGlobals.CooldownTimer.FindIndex(x => 
                x.IsGlobal == true 
                && x.CommandID == commandID);

            if (index != -1)
                PluginGlobals.CooldownTimer[index].CooldownTime = timer.CooldownTime;
            else
                PluginGlobals.CooldownTimer.Add(timer);
        }
        else
        {
            timer.PlayerID = playerID;
            int index = PluginGlobals.CooldownTimer.FindIndex(x => 
                x.PlayerID == playerID 
                && x.CommandID == commandID);
            if (index != -1)
                PluginGlobals.CooldownTimer[index].CooldownTime = timer.CooldownTime;
            else
                PluginGlobals.CooldownTimer.Add(timer);
        }
    }

    /// <summary>
    /// Sets the cooldown for the command
    /// </summary>
    /// <param name="player">Need to add the player if the Cooldown is only for a specific player</param>
    /// <param name="cmd"></param>
    public void SetCooldown(CCSPlayerController player, Commands cmd)
    {
        if (cmd.Cooldown is JsonElement jsonElement)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Number:

                    int cooldown = cmd.Cooldown.GetInt32();
                    if (cooldown == 0) 
                        break;

                    AddToCooldownList(false, player.UserId ?? 0, cmd.ID, cooldown);
                    break;

                case JsonValueKind.Object:

                    var cooldownObject = JsonSerializer.Deserialize<Cooldown>(cmd.Cooldown.GetRawText());
                    AddToCooldownList(cooldownObject.IsGlobal, player.UserId ?? 0, cmd.ID, cooldownObject.CooldownTime);
                    
                    break;

                default:
                    break;
            }
        }
    }
}
