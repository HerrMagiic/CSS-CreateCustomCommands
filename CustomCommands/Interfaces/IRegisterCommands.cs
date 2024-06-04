using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface IRegisterCommands
{
    /// <summary>
    /// Adds custom commands to the plugin.
    /// </summary>
    /// <param name="com">The command to add.</param>
    void AddCommands(Commands cmd);
    
    /// <summary>
    /// Checks for duplicate commands in the provided list and removes them.
    /// </summary>
    /// <param name="comms">The list of commands to check for duplicates.</param>
    /// <returns>A new list of commands without any duplicates.</returns>
    List<Commands> CheckForDuplicateCommands(List<Commands> comms);
}
