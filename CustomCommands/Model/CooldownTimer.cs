namespace CustomCommands.Model;

public class CooldownTimer
{
    public bool IsGlobal { get; set; } = false;
    public int PlayerID { get; set; }
    public required Guid CommandID { get; set; }
    public required DateTime CooldownTime { get; set; }
}