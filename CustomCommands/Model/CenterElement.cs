using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CustomCommands.Model;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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