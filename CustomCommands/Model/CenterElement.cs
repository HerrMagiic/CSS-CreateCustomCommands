
namespace CustomCommands.Model;
public class CenterClientElement
{
    public int ClientId { get; set; } = -1;
    public string Message { get; set; } = "";
}
public class CenterServerElement
{
    public string Message { get; set; } = "";
    public bool IsRunning { get; set; } = false;
}