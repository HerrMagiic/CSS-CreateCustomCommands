using CounterStrikeSharp.API.Core;

namespace CustomCommands.Interfaces;

public interface IReplaceTagsFunctions
{
    string[] ReplaceTags(string[] input, CCSPlayerController player);
    string ReplaceLanguageTags(string input);
    string ReplaceMessageTags(string input, CCSPlayerController player);
    string ReplaceColorTags(string input);
    string[] WrappedLine(dynamic message);
}