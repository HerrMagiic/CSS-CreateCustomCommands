using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface ILoadJson
{
    List<Commands> GettingCommandsFromJsonsFiles(string path);
}