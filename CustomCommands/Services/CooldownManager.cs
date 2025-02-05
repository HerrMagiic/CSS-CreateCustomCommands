using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public class CooldownManager : ICooldownManager
{
    public IPluginGlobals _pluginGlobals { get; }
    public IReplaceTagsFunctions _replaceTagsFunctions { get; }
    
    public CooldownManager(IPluginGlobals PluginGlobals, IReplaceTagsFunctions ReplaceTagsFunctions)
    {
        _pluginGlobals          = PluginGlobals;
        _replaceTagsFunctions   = ReplaceTagsFunctions;
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

    /// <summary>
    /// Checks if a command is on cooldown based on a given condition.
    /// </summary>
    /// <param name="predicate">The condition to check for each cooldown timer.</param>
    /// <param name="player">The player controller.</param>
    /// <param name="cmd">The command.</param>
    /// <returns>True if the command is on cooldown, false otherwise.</returns>
    public bool IsCommandOnCooldownWithCondition(Func<CooldownTimer, bool> predicate, CCSPlayerController player, Commands cmd)
    {
        var index = _pluginGlobals.CooldownTimer.FindIndex(x => predicate(x) && x.CooldownTime > DateTime.Now);

        if (index != -1)
        {
            var totalSeconds         = (double)_pluginGlobals.CooldownTimer[index].CooldownTime.Subtract(DateTime.Now).TotalSeconds;
            var totalSecondsRounded     = (int)Math.Round(totalSeconds);
            var timeleft             = totalSecondsRounded.ToString();
            var message              = "";
            
            try
            {
                var cooldown = JsonSerializer.Deserialize<Cooldown>(cmd.Cooldown.GetRawText());
                string[] replaceTimeleft = {cooldown.CooldownMessage.Replace("{TIMELEFT}", timeleft)};
                message = _replaceTagsFunctions.ReplaceTags(replaceTimeleft, player)[0];
            }
            catch (JsonException)
            {
                message = $"This command is for {timeleft} seconds on cooldown";
            }
                
            player.PrintToChat($"{_pluginGlobals.Config.Prefix}{message}");

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
            int index = _pluginGlobals.CooldownTimer.FindIndex(x => 
                x.IsGlobal == true 
                && x.CommandID == commandID);

            if (index != -1)
                _pluginGlobals.CooldownTimer[index].CooldownTime = timer.CooldownTime;
            else
                _pluginGlobals.CooldownTimer.Add(timer);
        }
        else
        {
            timer.PlayerID = playerID;
            int index = _pluginGlobals.CooldownTimer.FindIndex(x => 
                x.PlayerID == playerID 
                && x.CommandID == commandID);
            if (index != -1)
                _pluginGlobals.CooldownTimer[index].CooldownTime = timer.CooldownTime;
            else
                _pluginGlobals.CooldownTimer.Add(timer);
        }
    }

    /// <summary>
    /// Sets the cooldown for the command
    /// </summary>
    /// <param name="player">Need to add the player if the Cooldown is only for a specific player</param>
    /// <param name="cmd"></param>
    public void SetCooldown(CCSPlayerController player, Commands cmd)
    {
        if (cmd.Cooldown is not JsonElement jsonElement)
        {
            return;
        }
        
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Number:

                var cooldown = cmd.Cooldown.GetInt32();
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
