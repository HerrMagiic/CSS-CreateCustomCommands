using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Model
{
    public class Commands
    {
        public string Title { get; set; } = "";
        public string Command { get; set; } = "testtesttest";
        public string Message { get; set; } = "testtesttest";
        public string CenterMessage { get; set; } = "test";
        public int CenterMessageTime { get; set; } = 1;
        public Sender PrintTo { get; set; } = Sender.ClientChat;
        public string Description { get; set; } = "Description";
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
}
