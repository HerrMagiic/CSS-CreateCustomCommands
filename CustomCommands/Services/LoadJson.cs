using System.Text;
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
    /// Retrieves a list of commands from JSON files located in the specified path.
    /// </summary>
    /// <param name="path">The path where the JSON files are located.</param>
    /// <returns>A list of commands.</returns>
    public List<Commands> GetCommandsFromJsonFiles(string path)
    {
        var comms = new List<Commands>();

        string defaultconfigpath = Path.Combine(path, "Commands.json");

        CheckForExampleFile(path);

        var pathofcommands = Path.Combine(path, "Commands");

        var files = new List<string>();

        if (Directory.Exists(pathofcommands))
            files.AddRange(Directory.GetFiles(pathofcommands, "*.json", SearchOption.AllDirectories));

        // Check if the default config file exists in plugins/CustomCommands
        if (File.Exists(defaultconfigpath))
        {
            files.Add(defaultconfigpath);
            Logger.LogInformation("Found default config file.");
        }
        else if (!File.Exists(defaultconfigpath) && files.Count == 0)
        {
            Logger.LogWarning("No Config file found. Please create plugins/CustomCommands/Commands.json or in plugins/CustomCommands/Commands/<name>.json");
            return comms;
        }

        foreach (var file in files)
        {
            string json;

            // Read Unicode Characters
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
                json = sr.ReadToEnd();

            // Validate the JSON file
            if (!IsValidJsonSyntax(file))
                continue;

            var commands = JsonSerializer.Deserialize<List<Commands>>(json);
            if (ValidateObject(commands, file))
                comms.AddRange(commands!);
        }
        return comms;
    }
    

    /// <summary>
    /// Checks if an .example file exists in the specified path and creates a default config file if it doesn't exist.
    /// </summary>
    /// <param name="path">The path to check for the example file.</param>
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
            Logger.LogInformation("Created default config file.");
        }
    }

    /// <summary>
    /// Checks if the JSON file has a valid syntax.
    /// </summary>
    /// <param name="path">The path to the JSON file.</param>
    /// <returns>True if the JSON syntax is valid, false otherwise.</returns>
    public bool IsValidJsonSyntax(string path)
    {
        try
        {
            var json = File.ReadAllText(path);
            var document = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException ex)
        {
            Logger.LogError($"Invalid JSON syntax in {path}. Please check the docs on how to create a valid JSON file");
            Logger.LogError(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Validates the list of commands loaded from a JSON file.
    /// </summary>
    /// <param name="comms">The list of commands to validate.</param>
    /// <param name="path">The path of the JSON file.</param>
    /// <returns>True if all commands are valid, false otherwise.</returns>
    public bool ValidateObject(List<Commands>? comms, string path)
    {
        if (comms == null)
        {
            Logger.LogError($"Invalid JSON format in {path}. Please check the docs on how to create a valid JSON file");
            return false;
        }
        bool commandstatus = true;
        for (int i = 0; i < comms.Count; i++)
        {
            commandstatus = true;
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
            }
        }
        if (!commandstatus)
            return false;
        return true;
    }

    /// <summary>
    /// Logs the details of a command.
    /// </summary>
    /// <param name="comms">The command to log.</param>
    public void LogCommandDetails(Commands comms)
    {
        Logger.LogInformation($"-- Title: {comms.Title}");
        Logger.LogInformation($"-- Description: {comms.Description}");
        Logger.LogInformation($"-- Command: {comms.Command}");
        Logger.LogInformation($"-- Message: {comms.Message}");
        Logger.LogInformation($"-- CenterMessage: {comms.CenterMessage.Message}");
        Logger.LogInformation($"-- CenterMessageTime: {comms.CenterMessage.Time}");
        Logger.LogInformation($"-- PrintTo: {comms.PrintTo}");
        Logger.LogInformation($"-- ServerCommands: {JsonSerializer.Serialize(comms.ServerCommands)}");
        Logger.LogInformation($"-- PermissionList: {JsonSerializer.Serialize(comms.Permission)}");
        Logger.LogInformation("--------------------------------------------------");
    }

    /// <summary>
    /// Validates the PrintTo property of the Commands object and checks if the required message properties are set based on the PrintTo value.
    /// </summary>
    /// <param name="comms">The Commands object to validate.</param>
    /// <returns>True if the PrintTo property is valid and the required message properties are set; otherwise, false.</returns>
    public bool PrintToCheck(Commands comms)
    {
        if (comms.PrintTo == Sender.ClientChat || comms.PrintTo == Sender.AllChat)
        {

            if (!ValidateMessage(comms.Message))
            {
                Logger.LogError($"Message not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                return false;
            }
        }
        else if (comms.PrintTo == Sender.ClientCenter || comms.PrintTo == Sender.AllCenter)
        {
            if (string.IsNullOrEmpty(comms.CenterMessage.Message))
            {
                Logger.LogError($"CenterMessage is not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                return false;
            }
        } 
        else
        {
            if (!ValidateMessage(comms.Message) && string.IsNullOrEmpty(comms.CenterMessage.Message))
            {
                Logger.LogError($"Message and CenterMessage are not set but needs to be set because PrintTo is set to {comms.PrintTo}");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Validates a dynamic message by checking if it is a string or an array.
    /// </summary>
    /// <param name="message">The dynamic message to validate.</param>
    /// <returns>True if the message is a string or an array, otherwise false.</returns>
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
