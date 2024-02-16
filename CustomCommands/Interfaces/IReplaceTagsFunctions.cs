using CounterStrikeSharp.API.Core;

namespace CustomCommands.Interfaces;

public interface IReplaceTagsFunctions
{
    string[] ReplaceTags(dynamic input, CCSPlayerController player);
    string ReplaceLanguageTags(string input);
    string ReplaceMessageTags(string input, CCSPlayerController player, bool safety = true);
    string ReplaceColorTags(string input);
    List<string> WrappedLine(dynamic message);
}