using CounterStrikeSharp.API.Core;

namespace CustomCommands.Interfaces;

public interface IReplaceTagsFunctions
{
    /// <summary>
    /// Replaces tags in the input array with their corresponding values.
    /// </summary>
    /// <param name="input">The array of strings containing tags to be replaced.</param>
    /// <param name="player">The CCSPlayerController object used for tag replacement.</param>
    /// <returns>The array of strings with tags replaced.</returns>
    string[] ReplaceTags(dynamic input, CCSPlayerController player);

    /// <summary>
    /// Replaces language tags in the input string with the corresponding localized value.
    /// Language tags are defined within curly braces, e.g. "{LANG=LocalizerTag}".
    /// If a language tag is found, it is replaced with the localized value from the CustomCommands plugin's Localizer.
    /// If the localized value is not found, a default message is returned.
    /// </summary>
    /// <param name="input">The input string to process.</param>
    /// <returns>The input string with language tags replaced with localized values.</returns>
    string ReplaceLanguageTags(string input);

    /// <summary>
    /// Replaces tags in the input string with corresponding values based on the provided player information.
    /// </summary>
    /// <param name="input">The input string containing tags to be replaced.</param>
    /// <param name="player">The CCSPlayerController object representing the player.</param>
    /// <param name="safety">A boolean value indicating whether to replace the {PLAYERNAME} tag. Default is true.</param>
    /// <returns>The modified string with replaced tags.</returns>
    string ReplaceMessageTags(string input, CCSPlayerController player, bool safety = true);
    string ReplaceColorTags(string input);

    /// <summary>
    /// Splits the input into an array of strings. If the input is a string, it will be split by newlines. If the input is an array, each element will be split by newlines.
    /// </summary>
    /// <param name="input">This should be a string[] or a string</param>
    /// <returns>An array of strings representing the lines of the input.</returns>
    List<string> WrappedLine(dynamic message);

    string ReplaceRandomTags(string message);
}