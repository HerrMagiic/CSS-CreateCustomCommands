using CustomCommands.Interfaces;
using CustomCommands.Model;

namespace UnitTests;

public partial class UnitTests
{
    [Theory]
    [InlineData("test,test2,test3", new[] { "test", "test2", "test3" })]
    [InlineData("test;test2;test3", new[] { "test", "test2", "test3" })]
    [InlineData("test,test2;test3", new[] { "test", "test2", "test3" })]
    [InlineData("test", new[] { "test" })]
    [InlineData("test;", new[] { "test" })]
    [InlineData("", new string[] { })]
    public void Test_SplitStringByCommaOrSemicolon(string input, string[] expected)
    {
        var pluginUtilities = _sp.GetRequiredService<IPluginUtilities>();
        var split = pluginUtilities.SplitStringByCommaOrSemicolon(input);
        Assert.Equal(expected, split);
    }

    [Fact]
    public void Test_ExecuteServerCommands()
    {
        var pluginUtilities = _sp.GetRequiredService<IPluginUtilities>();
        var commands = new Commands
        {
            ServerCommands = new List<string>
            {
                "sv_cheats 1",
                "echo hello"
            }
        };
        pluginUtilities.ExecuteServerCommands(commands);
    }
}
