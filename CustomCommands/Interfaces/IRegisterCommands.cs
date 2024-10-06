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
    void CheckForDuplicateCommands();

    /// <summary>
    /// Converts custom commands stored in the global plugin state by processing each command string, 
    /// splitting it by commas or semicolons, and adjusting the command structure.
    /// 
    /// The method clones each command, assigns a new unique ID, and processes its arguments.
    /// Each split command is then added back to the global list with updated formatting.
    /// </summary>
    void ConvertingCommandsForRegister();
}
