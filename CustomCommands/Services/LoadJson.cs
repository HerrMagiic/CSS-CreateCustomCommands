using System.Text;
using System.Text.Json;
using CustomCommands.Interfaces;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;

namespace CustomCommands.Services;

public class LoadJson : ILoadJson
{
    private readonly ILogger<CustomCommands> _logger;
    
    public LoadJson(ILogger<CustomCommands> Logger)
    {
        _logger = Logger;
    }

    public async Task<List<Commands>> GetCommandsFromJsonFiles(string path)
    {
        var comms = new List<Commands>();

        var defaultconfigpath = Path.Combine(path, "Commands.json");

        CheckForExampleFile(path);

        var pathofcommands = Path.Combine(path, "Commands");

        var files = new List<string>();

        if (Directory.Exists(pathofcommands))
            files.AddRange(Directory.GetFiles(pathofcommands, "*.json", SearchOption.AllDirectories));

        // Check if the default config file exists in plugins/CustomCommands
        if (File.Exists(defaultconfigpath))
        {
            files.Add(defaultconfigpath);
            _logger.LogInformation("Found default config file.");
        }
        else if (!File.Exists(defaultconfigpath) && files.Count == 0)
        {
            _logger.LogWarning("No Config file found. Please create plugins/CustomCommands/Commands.json or in plugins/CustomCommands/Commands/<name>.json");
            return comms;
        }

        // Create a list of tasks to handle each file asynchronously
        var tasks = files.Select(async file =>
        {
            var json = string.Empty;

            // Read Unicode Characters asynchronously
            using (var sr = new StreamReader(file, Encoding.UTF8))
                json = await sr.ReadToEndAsync();

            // Validate the JSON file
            if (!IsValidJsonSyntax(json, file))
                return;

            // Deserialize and validate the commands
            var commands = JsonSerializer.Deserialize<List<Commands>>(json);
            if (ValidateObject(commands, file))
            {
                lock (comms) // Ensure thread-safety while adding to the shared list
                {
                    comms.AddRange(commands!);
                }
            }
        });

        await Task.WhenAll(tasks);

        return comms;
    }
    
    public void CheckForExampleFile(string path)
    {
        if (Directory.Exists(Path.Combine(path, "Commands")))
        {
            var files = Directory.GetFiles(Path.Combine(path, "Commands"), "*.json", SearchOption.AllDirectories);
            if (files.Length > 0)
                return;
        }

        var defaultconfigpath = Path.Combine(path, "Commands.json");
        var exampleconfigpath = Path.Combine(path, "Commands.example.json");
        if (!File.Exists(defaultconfigpath))
        {
            File.Copy(exampleconfigpath, defaultconfigpath);
            _logger.LogInformation("Created default config file.");
        }
    }

    public bool IsValidJsonSyntax(string json, string path)
    {
        try
        {
            var document = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Invalid JSON syntax in {path}. Please check the docs on how to create a valid JSON file");
            _logger.LogError(ex.Message);
            return false;
        }
    }

    public bool ValidateObject(List<Commands>? comms, string path)
    {
        if (comms == null)
        {
            _logger.LogError($"Invalid JSON format in {path}. Please check the docs on how to create a valid JSON file");
            return false;
        }
        var commandstatus = true;
        for (int i = 0; i < comms.Count; i++)
        {
            commandstatus = true;
            // Title
            if (string.IsNullOrEmpty(comms[i].Title))
            {
                _logger.LogWarning($"Title not set in {path}. Title is not required but recommended");
                commandstatus = false;
            }
            // Description
            if (string.IsNullOrEmpty(comms[i].Description))
            {
                _logger.LogWarning($"Description not set in {path}. Description is not required but recommended. This will be shown in the help command");
                commandstatus = false;
            }
            // Command
            if (string.IsNullOrEmpty(comms[i].Command))
            {
                _logger.LogError($"Command not set in {path}");
                commandstatus = false;
            }
            if (!PrintToCheck(comms[i]))
                commandstatus = false;

            if (!commandstatus)
            {
                _logger.LogError($"Command {comms[i].Command} will not be loaded. Index: {i}");
                LogCommandDetails(comms[i]);
            }
        }
        if (!commandstatus)
            return false;
        return true;
    }

    public void LogCommandDetails(Commands comms)
    {
        _logger.LogInformation($"-- Title: {comms.Title}");
        _logger.LogInformation($"-- Description: {comms.Description}");
        _logger.LogInformation($"-- Command: {comms.Command}");
        _logger.LogInformation($"-- Message: {comms.Message}");
        _logger.LogInformation($"-- CenterMessage: {comms.CenterMessage.Message}");
        _logger.LogInformation($"-- CenterMessageTime: {comms.CenterMessage.Time}");
        _logger.LogInformation($"-- PrintTo: {comms.PrintTo}");
        _logger.LogInformation($"-- ServerCommands: {JsonSerializer.Serialize(comms.ServerCommands)}");
        _logger.LogInformation($"-- PermissionList: {JsonSerializer.Serialize(comms.Permission)}");
        _logger.LogInformation("--------------------------------------------------");
    }

    public bool PrintToCheck(Commands comms)
    {
        if (comms.PrintTo == Sender.ClientChat || comms.PrintTo == Sender.AllChat)
        {

            if (!ValidateMessage(comms.Message))
            {
                _logger.LogError($"Message not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                return false;
            }
        }
        else if (comms.PrintTo == Sender.ClientCenter || comms.PrintTo == Sender.AllCenter)
        {
            if (string.IsNullOrEmpty(comms.CenterMessage.Message))
            {
                _logger.LogError($"CenterMessage is not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                return false;
            }
        } 
        else
        {
            if (!ValidateMessage(comms.Message) && string.IsNullOrEmpty(comms.CenterMessage.Message))
            {
                _logger.LogError($"Message and CenterMessage are not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                return false;
            }
        }
        return true;
    }

    public bool ValidateMessage(dynamic message)
    {
        if (message is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.String)
                return true;

            if (jsonElement.ValueKind == JsonValueKind.Array)
                return true;

            return false;
        }
        return false;
    }
}
