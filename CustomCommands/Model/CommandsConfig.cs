
namespace CustomCommands.Model;
public class Commands
{
    public required string Title { get; set; } = "";
    public string Description { get; set; } = "Description";
    public required string Command { get; set; } = "testtesttest";
    public string Message { get; set; } = "";
    public CenterElement CenterMessage { get; set; } = new();
    public required Sender PrintTo { get; set; } = Sender.ClientChat;
    public List<string> ServerCommands { get; set; } = new();
    public PermissionsElement Permissions { get; set; } = new();
}
public class PermissionsElement
{
    public bool ReguiresAllPermissions { get; set; } = false;
    public List<string> PermissionList { get; set; } = new();
}
public class CenterElement
{
    public string Message { get; set; } = "";
    public int Time { get; set; } = 1;
}
public enum Sender
{
    ClientChat = 0,
    AllChat = 1,
    ClientCenter = 2,
    AllCenter = 3,
    ClientChatClientCenter = 4,
    ClientChatAllCenter = 5,
    AllChatClientCenter = 6,
    AllChatAllCenter = 7
}

