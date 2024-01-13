using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface ILoadJson
{
    List<Commands> GettingCommandsFromJsonsFiles(string path);
    bool ValidateObject(Commands comms, string path);
    void CheckForExampleFile(string path);
}