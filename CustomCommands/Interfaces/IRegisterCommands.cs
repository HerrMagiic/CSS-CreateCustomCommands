using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface IRegisterCommands
{
    void AddCommands(Commands cmd);
    List<Commands> CheckForDuplicateCommands(List<Commands> comms);
}
