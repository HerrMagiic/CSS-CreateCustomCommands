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

    public List<Commands> GetCommandsFromJsonFiles(string path)
    {
        var comms = new List<Commands>();

        CheckForExampleFile(path);

        var pathofcommands = Path.Combine(path, "Commands");
        var defaultconfigpath = Path.Combine(path, "Commands.json");
        var files = Directory.GetFiles(pathofcommands, "*.json", SearchOption.AllDirectories);

        // Check if the default config file exists in plugins/CustomCommands
        if (File.Exists(defaultconfigpath))
        {
            _ = files.Append(defaultconfigpath);
            Logger.LogInformation("Found default config file.");
        }
        //
        else if (!File.Exists(defaultconfigpath) && files.Length == 0)
        {
            Logger.LogWarning("No Config file found. Please create plugins/CustomCommands/Commands.json or in plugins/CustomCommands/Commands/<name>.json");
            return comms;
        }

        foreach (var file in files)
        {
            var json = File.ReadAllText(file);
            var commands = JsonSerializer.Deserialize<List<Commands>>(json);
            if (ValidateObject(commands, file))
                comms.AddRange(commands!);
            
        }
        return comms;
    }
    // Check if the Command.json file exists. If not replace it with the example file
    public void CheckForExampleFile(string path)
    {
        var defaultconfigpath = Path.Combine(path, "Commands.json");
        var exampleconfigpath = Path.Combine(path, "Commands.example.json");
        if (!File.Exists(defaultconfigpath))
        {
            File.Copy(exampleconfigpath, defaultconfigpath);
            Logger.LogInformation("Created default config file.");
        }
        
    }
    public bool ValidateObject(List<Commands>? comms, string path)
    {
        if (comms == null)
        {
            Logger.LogError($"Invalid JSON format in {path}. Please check the docs on how to create a valid JSON file");
            return false;
        }
        for (int i = comms.Count - 1; i >= 0 ; i--)
        {
            bool commandstatus = true;
            // Title
            if (string.IsNullOrEmpty(comms[i].Title))
            {
                Logger.LogWarning($"Title not set in {path}. Title is not required but recommended");
                commandstatus = false;
            }
            // Description
            if (string.IsNullOrEmpty(comms[i].Description))
            {
                Logger.LogWarning($"Description not set in {path}. Description is not required but recommended. This will be shown in the help command");
                commandstatus = false;
            }
            // Command
            if (string.IsNullOrEmpty(comms[i].Command))
            {
                Logger.LogError($"Command not set in {path}");
                commandstatus = false;
            }
            if (!PrintToCheck(comms[i]))
                commandstatus = false;

            if (!commandstatus)
            {
                Logger.LogError($"Command {comms[i].Command} will not be loaded. Index: {i}");
                LogCommandDetails(comms[i]);
                return false;
            }

            return true;
        }
        return true;
    }

    public void LogCommandDetails(Commands comms)
    {
            Logger.LogInformation($"Title: {comms.Title}");
            Logger.LogInformation($"Description: {comms.Description}");
            Logger.LogInformation($"Command: {comms.Command}");
            Logger.LogInformation($"Message: {comms.Message}");
            Logger.LogInformation($"CenterMessage: {comms.CenterMessage.Message}");
            Logger.LogInformation($"CenterMessageTime: {comms.CenterMessage.Time}");
            Logger.LogInformation($"PrintTo: {comms.PrintTo}");
            Logger.LogInformation($"ServerCommands: {string.Join(", ", comms.ServerCommands)}");
            Logger.LogInformation($"PermissionList: {string.Join(", ", comms.Permission.PermissionList)}");
            Logger.LogInformation($"RequiresAllPermissions: {comms.Permission.RequiresAllPermissions}");
    }

    public bool PrintToCheck(Commands comms)
    {
        if (comms.PrintTo == Sender.ClientChat || comms.PrintTo == Sender.AllChat)
        {
            if (string.IsNullOrEmpty(comms.Message))
            {
                Logger.LogError($"Message not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                    LogCommandDetails(comms);
                return false;
            }
        }
        else if (comms.PrintTo == Sender.ClientCenter || comms.PrintTo == Sender.AllCenter)
        {
            if (string.IsNullOrEmpty(comms.CenterMessage.Message))
            {
                Logger.LogError($"CenterMessage is not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                LogCommandDetails(comms);
                return false;
            }
        } 
        else
        {
            if (string.IsNullOrEmpty(comms.Message) && string.IsNullOrEmpty(comms.CenterMessage.Message))
            {
                Logger.LogError($"Message and CenterMessage are not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                LogCommandDetails(comms);
                return false;
            }
        }
        return true;
    }
}
