namespace CustomCommands.Model;

public class Commands :  ICloneable
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public string? Argument { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "Description";
    public string Command { get; set; } = "";
    public dynamic Cooldown { get; set; } = 0;
    public dynamic Message { get; set; } = "";
    public CenterElement CenterMessage { get; set; } = new();
    public Sender PrintTo { get; set; } = Sender.ClientChat;
    public List<string> ServerCommands { get; set; } = new();
    public List<string> ClientCommands { get; set; } = new();
    public List<string> ClientCommandsFromServer { get; set; } = new();
    public Permission? Permission { get; set; } = new();

    public object Clone()
    {
        return new Commands() 
        {
            ID = ID,
            Argument = Argument,
            Title = Title,
            Description = Description,
            Command = Command,
            Cooldown = Cooldown,
            Message = Message,
            CenterMessage = CenterMessage,
            PrintTo = PrintTo,
            ServerCommands = new List<string>(ServerCommands),
            ClientCommands = new List<string>(ClientCommands),
            ClientCommandsFromServer = new List<string>(ClientCommandsFromServer),
            Permission = Permission?.Clone() as Permission
        };
    }
}
public class Cooldown
{
    public int CooldownTime { get; set; } = 0;
    public bool IsGlobal { get; set; } = false;
    public string CooldownMessage { get; set; } = "";
}
public class Permission :  ICloneable
{
    public bool RequiresAllPermissions { get; set; } = false;
    public List<string> PermissionList { get; set; } = new();

    public object Clone()
    {
        return MemberwiseClone();
    }

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
