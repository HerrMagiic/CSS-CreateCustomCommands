using CounterStrikeSharp.API.Core;
using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface IPluginUtilities
{
    /// <summary>
    /// Splits a string by comma, semicolon, or whitespace characters.
    /// </summary>
    /// <param name="str">The string to be split.</param>
    /// <returns>An array of strings containing the split substrings.</returns>
    string[] SplitStringByCommaOrSemicolon(string str);
    /// <summary>
    /// Executes the server commands from the command object
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="player"></param>
    void ExecuteServerCommands(Commands cmd, CCSPlayerController player);
    /// <summary>
    /// Issue the specified command to the specified client (mimics that client typing the command at the console).
    /// Note: Only works for some commands, marked with the FCVAR_CLIENT_CAN_EXECUTE flag (not many).
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="player"></param>
    void ExecuteClientCommands(Commands cmd, CCSPlayerController player);
    /// <summary>
    /// Issue the specified command directly from the server (mimics the server executing the command with the given player context).
    /// <remarks>Works with server commands like `kill`, `explode`, `noclip`, etc. </remarks>
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="player"></param>
    void ExecuteClientCommandsFromServer(Commands cmd, CCSPlayerController player);
    /// <summary>
    /// Checks if the player has the required permissions to execute the command
    /// </summary>
    /// <param name="player"></param>
    /// <param name="permissions"></param>
    /// <returns></returns>
    bool RequiresPermissions(CCSPlayerController player, Permission permissions);
}
