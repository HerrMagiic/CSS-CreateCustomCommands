using System.Text.Json;
using CustomCommands.Interfaces;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CustomCommands.Services;

public class LoadJson : ILoadJson
{
    private readonly ILogger<CustomCommands> Logger;
    
    public LoadJson(ILogger<CustomCommands> Logger)
    {
        this.Logger = Logger;
    }

    public List<Commands> GettingCommandsFromJsonsFiles(string path)
    {
        var comms = new List<Commands>();

        var pathofcommands = Path.Combine(path, "Commands");
        var defaultconfigpath = Path.Combine(path, "Commands.json");
        var files = Directory.GetFiles(pathofcommands, "*.json", SearchOption.AllDirectories);

        // Check if the default config file exists in plugins/CustomCommands
        if (File.Exists(defaultconfigpath))
        {
            files.Append(defaultconfigpath);
            Logger.LogInformation("Found default config file");
        }
        //
        else if (!File.Exists(defaultconfigpath) && files.Length == 0)
        {
            Logger.LogWarning("No Config file found. Please create plugins/CustomCommands/Commands.json or  in plugins/CustomCommands/Commands/<name>.json");
            return comms;
        }

        foreach (var file in files)
        {
            var json = File.ReadAllText(file);
            var commands = JsonSerializer.Deserialize<List<Commands>>(json);
            if (commands != null)
                comms.AddRange(commands);
            
        }
        return comms;
    }
    public bool ValidateObject(Commands comms, string path)
    {
        switch (comms)
        {
            case null:
                Logger.LogError($"Config files is empty or not valid: {Path.GetFullPath(path)}");
                return false;
            case { }:
            
            default:
        }
    }
}
