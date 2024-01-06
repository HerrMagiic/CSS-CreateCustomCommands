using CustomCommands.Model;

namespace CustomCommands.Services;

public interface IRegisterCommands
{
    void AddCommands(Commands cmd);
    List<Commands> CheckForDuplicateCommands(List<Commands> comms);
    void ExecuteServerCommands(Commands cmd);
}
