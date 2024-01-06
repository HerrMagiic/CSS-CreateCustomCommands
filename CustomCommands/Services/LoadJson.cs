using System.Text.Json;
using CustomCommands.Interfaces;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;

namespace CustomCommands.Services;

public class LoadJson : ILoadJson
{
    private readonly ILogger<CustomCommands> Logger;
    
    public LoadJson(ILogger<CustomCommands> Logger)
    {
        this.Logger = Logger;
    }

    /// <summary>
    /// Load the commands from the JSON file
    /// </summary>
    /// <returns>Returns the Commands as a List</returns>
    public List<Commands>? LoadCommandsFromJson(string path)
    {
        string jsonPath = Path.Combine(path, "Commands.json");
        if (File.Exists(jsonPath))
        {
            var json = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<List<Commands>>(json);
        }
        else
        {
            Logger.LogWarning("No Config file found. Please create one");
            return null;
        }
    }
}
