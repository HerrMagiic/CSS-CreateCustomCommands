using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface ILoadJson
{
    List<Commands>? LoadCommandsFromJson(string path);
}