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
    public bool OnCooldown(CCSPlayerController player, Commands cmd)
    {
        
        
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

                    var timer = new CooldownTimer() {
                        IsGlobal = false, 
                        PlayerID = player.UserId ?? 0, 
                        CommandID = cmd.ID, 
                        CooldownTime = DateTime.Now.AddSeconds(cooldown)
                    };
                    PluginGlobals.CooldownTimer.Add(timer);

                    break;
                case JsonValueKind.Object:
                    Cooldown cooldownObject = (Cooldown)cmd.Cooldown;

                    if (cooldownObject.IsGlobal)
                    {
                        var timerObj = new CooldownTimer() {
                            IsGlobal = true, 
                            CommandID = cmd.ID, 
                            CooldownTime = DateTime.Now.AddSeconds(cooldownObject.CooldownTime)
                        };
                        PluginGlobals.CooldownTimer.Add(timerObj);
                    } else {
                        var timerObj = new CooldownTimer() {
                            IsGlobal = false, 
                            PlayerID = player.UserId ?? 0, 
                            CommandID = cmd.ID, 
                            CooldownTime = DateTime.Now.AddSeconds(cooldownObject.CooldownTime)
                        };
                        PluginGlobals.CooldownTimer.Add(timerObj);
                    }

                    break;

                default:
                    break;
            }
        }
    }
}
