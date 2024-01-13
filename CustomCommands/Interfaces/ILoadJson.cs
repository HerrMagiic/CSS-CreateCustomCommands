using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface ILoadJson
{
    List<Commands> GetCommandsFromJsonFiles(string path);
    void CheckForExampleFile(string path);
    bool ValidateObject(List<Commands>? comms, string path);
    void LogCommandDetails(Commands comms);
    bool PrintToCheck(Commands comms);
}