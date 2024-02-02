
using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface ICooldownManager
{
    bool IsCommandOnCooldown(CCSPlayerController player, Commands cmd);
    void AddToCooldownList(bool isGlobal, int playerID, Guid commandID, int cooldownTime);
    bool IsCommandOnCooldownWithCondition(Func<CooldownTimer, bool> predicate, CCSPlayerController player, Commands cmd);
    void SetCooldown(CCSPlayerController player, Commands cmd);
}
