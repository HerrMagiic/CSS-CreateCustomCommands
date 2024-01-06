using CustomCommands.Model;

namespace CustomCommands.Services;

public interface ILoadJson
{
    List<Commands>? LoadCommandsFromJson(string path);
}