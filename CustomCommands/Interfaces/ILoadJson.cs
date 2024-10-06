using CustomCommands.Model;

namespace CustomCommands.Interfaces;

public interface ILoadJson
{
    /// <summary>
    /// Retrieves a list of commands from JSON files located in the specified path.
    /// </summary>
    /// <param name="path">The path where the JSON files are located.</param>
    /// <returns>A list of commands.</returns>
    Task<List<Commands>> GetCommandsFromJsonFiles(string path);

    /// <summary>
    /// Checks if an .example file exists in the specified path and creates a default config file if it doesn't exist.
    /// </summary>
    /// <param name="path">The path to check for the example file.</param>
    void CheckForExampleFile(string path);

    /// <summary>
    /// Checks if the JSON file has a valid syntax.
    /// </summary>
    /// <param name="path">The path to the JSON file.</param>
    /// <param name="json">The json string.</param>
    /// <returns>True if the JSON syntax is valid, false otherwise.</returns>
    bool IsValidJsonSyntax(string json, string path);

    /// <summary>
    /// Validates the list of commands loaded from a JSON file.
    /// </summary>
    /// <param name="comms">The list of commands to validate.</param>
    /// <param name="path">The path of the JSON file.</param>
    /// <returns>True if all commands are valid, false otherwise.</returns>
    bool ValidateObject(List<Commands>? comms, string path);

    /// <summary>
    /// Logs the details of a command.
    /// </summary>
    /// <param name="comms">The command to log.</param>
    void LogCommandDetails(Commands comms);

    /// <summary>
    /// Validates the PrintTo property of the Commands object and checks if the required message properties are set based on the PrintTo value.
    /// </summary>
    /// <param name="comms">The Commands object to validate.</param>
    /// <returns>True if the PrintTo property is valid and the required message properties are set; otherwise, false.</returns>
    bool PrintToCheck(Commands comms);

    /// <summary>
    /// Validates a dynamic message by checking if it is a string or an array.
    /// </summary>
    /// <param name="message">The dynamic message to validate.</param>
    /// <returns>True if the message is a string or an array, otherwise false.</returns>
    bool ValidateMessage(dynamic message);
}