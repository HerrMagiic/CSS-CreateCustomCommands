using System.Text.Json;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CustomCommands.Interfaces;
using CustomCommands.Model;

namespace CustomCommands.Services;

public class PluginUtilities : IPluginUtilities
{
    private readonly IPluginGlobals PluginGlobals;
    private readonly IReplaceTagsFunctions ReplaceTagsFunctions;
    
    public PluginUtilities(IPluginGlobals PluginGlobals, IReplaceTagsFunctions ReplaceTagsFunctions)
    {
        this.PluginGlobals = PluginGlobals;
        this.ReplaceTagsFunctions = ReplaceTagsFunctions;
    }

    public string[] SplitStringByCommaOrSemicolon(string str)
    {
        return Regex.Split(str, ",|;|\\s")
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToArray();
    }
    
    public void ExecuteServerCommands(Commands cmd, CCSPlayerController player) 
    {
        if (cmd.ServerCommands.Count == 0) return;

        foreach (var serverCommand in cmd.ServerCommands)
        {
            Server.ExecuteCommand(ReplaceTagsFunctions.ReplaceMessageTags(serverCommand, player));
        }
    }
    public bool RequiresPermissions(CCSPlayerController player, Permission permissions)
    {
        if (!permissions.RequiresAllPermissions)
        {
            foreach (var permission in permissions.PermissionList)
            {
                if (AdminManager.PlayerHasPermissions(player, new string[]{permission})) 
                    return true;
            }
            player.PrintToChat($"{PluginGlobals.Config.Prefix}You don't have the required permissions to execute this command");
            return false;
        }
        else
        {
            if (!AdminManager.PlayerHasPermissions(player, permissions.PermissionList.ToArray()))
            {
                player.PrintToChat($"{PluginGlobals.Config.Prefix}You don't have the required permissions to execute this command");
                return false;
            }
            return true;
        }
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

    private bool IsCommandOnCooldownWithCondition(Func<CooldownTimer, bool> predicate, CCSPlayerController player, Commands cmd)
    {
        int index = PluginGlobals.CooldownTimer.FindIndex(x => predicate(x) && x.CooldownTime > DateTime.Now);

        if (index != -1)
        {
            string timeleft = PluginGlobals.CooldownTimer[index].CooldownTime.Subtract(DateTime.Now).Seconds.ToString();
            player.PrintToChat($"{PluginGlobals.Config.Prefix}{cmd.Cooldown.CooldownMessage.Replace("{TIME}", timeleft) 
                ?? $"This command is for {timeleft} seconds on cooldown"}");

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
                    int cooldown = (int)cmd.Cooldown;
                    if (cooldown == 0) 
                        break;

                    AddToCooldownList(false, player.UserId ?? 0, cmd.ID, cooldown);
                    break;

                case JsonValueKind.Object:
                    Cooldown cooldownObject = (Cooldown)cmd.Cooldown;

                    AddToCooldownList(cooldownObject.IsGlobal, player.UserId ?? 0, cmd.ID, cooldownObject.CooldownTime);
                    break;

                default:
                    break;
            }
        }
    }
}
