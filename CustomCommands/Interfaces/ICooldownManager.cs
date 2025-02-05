using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface ICooldownManager
{
    /// <summary>
    /// Checks if the command is on cooldown
    /// </summary>
    /// <param name="player"></param>
    /// <param name="cmd"></param>
    /// <returns></returns>
    bool IsCommandOnCooldown(CCSPlayerController player, Commands cmd);

    /// <summary>
    /// Adds the command to the cooldown list
    /// </summary>
    /// <param name="isGlobal"></param>
    /// <param name="playerID"></param>
    /// <param name="commandID"></param>
    /// <param name="cooldownTime"></param>
    void AddToCooldownList(bool isGlobal, int playerID, Guid commandID, int cooldownTime);

    /// <summary>
    /// Checks if a command is on cooldown based on a given condition.
    /// </summary>
    /// <param name="predicate">The condition to check for each cooldown timer.</param>
    /// <param name="player">The player controller.</param>
    /// <param name="cmd">The command.</param>
    /// <returns>True if the command is on cooldown, false otherwise.</returns>
    bool IsCommandOnCooldownWithCondition(Func<CooldownTimer, bool> predicate, CCSPlayerController player, Commands cmd);

    /// <summary>
    /// Sets the cooldown for the command
    /// </summary>
    /// <param name="player">Need to add the player if the Cooldown is only for a specific player</param>
    /// <param name="cmd"></param>
    void SetCooldown(CCSPlayerController player, Commands cmd);
}
