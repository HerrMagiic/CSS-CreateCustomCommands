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
            if (ValidateObject(commands, file))
                comms.AddRange(commands!);
            
        }
        return comms;
    }
    public bool ValidateObject(List<Commands>? comms, string path)
    {
        if (comms == null)
        {
            Logger.LogError($"Invalid object in {path}");
            return false;
        }
        foreach (var command in comms)
        {
            bool commandstatus = true;
            if (command.Title == null || command.Title == "")
            {
                Logger.LogWarning($"Title not set in {path}. Title is not required but recommended");
                commandstatus = false;
            }
            if (command.Description == null || command.Description == "")
            {
                Logger.LogWarning($"Description not set in {path}. Description is not required but recommended. This will be shown in the help command");
                commandstatus = false;
            }
            if (command.Command == null || command.Command == "")
            {
                Logger.LogError($"Command not set in {path}");
                commandstatus = false;
            }
            if (commad)
            if (!PrintToCheck(command))
                return false;

            if (!commandstatus)
            {
                SendCommandInfo(command);
                return false;
            }

            return true;
        }
    }
    public void SendCommandInfo(Commands comms)
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
            if (comms.Message == null || comms.Message == "")
            {
                Logger.LogError($"Message not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                Logger.LogError($"Title: {comms.Title}");
                Logger.LogError($"Description: {comms.Description}");
                Logger.LogError($"Command: {comms.Command}");
                return false;
            }
        }
        else if (comms.PrintTo == Sender.ClientCenter || comms.PrintTo == Sender.AllCenter)
        {
            if (comms.CenterMessage.Message == null || comms.CenterMessage.Message == "")
            {
                Logger.LogError($"CenterMessage is not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                Logger.LogError($"Title: {comms.Title}");
                Logger.LogError($"Description: {comms.Description}");
                Logger.LogError($"Command: {comms.Command}");
                return false;
            }
        } 
        else
        {
            if (comms.Message == null || comms.Message == "" && comms.CenterMessage.Message != null || comms.CenterMessage.Message != "")
            {
                Logger.LogError($"Message not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                Logger.LogError($"Title: {comms.Title}");
                Logger.LogError($"Description: {comms.Description}");
                Logger.LogError($"Command: {comms.Command}");
                return false;
            }
            else if (comms.CenterMessage.Message == null || comms.CenterMessage.Message == "" && comms.Message != null || comms.Message != "")
            {
                Logger.LogError($"CenterMessage is not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                Logger.LogError($"Title: {comms.Title}");
                Logger.LogError($"Description: {comms.Description}");
                Logger.LogError($"Command: {comms.Command}");
                return false;
            }
            else
            {
                Logger.LogError($"Message and CenterMessage are not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                Logger.LogError($"Title: {comms.Title}");
                Logger.LogError($"Description: {comms.Description}");
                Logger.LogError($"Command: {comms.Command}");
                return false;
            }
        }
        return true;
    }
}
