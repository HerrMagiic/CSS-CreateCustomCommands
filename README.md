## Custom Commands Plugin

Create your custom commands in ./plugins/CustomCommands/Commands.json

#### Example

```json
[
  {
    "Title": "Discord",
    "Command": "discord",
    "Message": "{PREFIX}{GREEN}Discord: \n <link>",
    "CenterMessage": "",
    "CenterMessageTime": 1,
    "PrintTo": 0,
    "Description": "Command for Discord"
  },
  {
    "Title": "Steam",
    "Command": "steam,steamgroup,group",
    "Message": "SteamGroup: <link>",
    "CenterMessage": "<div>Steam Group</div><br><div><font color='#00ff00'>https...</font></div>",
    "CenterMessageTime": 2,
    "PrintTo": 7,
    "Description": "Command for SteamGroup"
  }
]
```

**Title**: Just the title for your

**Command:** just type the name. !`<command>` and /`<command>` will work normally

**Message**: The message you want to send.

Colors:

* {DEFAULT}
* {RED}
* {LIGHTPURPLE}
* {GREEN}
* {LIME}
* {LIGHTGREEN}
* {LIGHTRED}
* {GRAY}
* {LIGHTOLIVE}
* {OLIVE}
* {LIGHTBLUE}
* {BLUE}
* {PURPLE}
* {GRAYBLUE}

**CenterMessage**: The Center Message. HTML works as well.

**PrintTo**: Where the message should be shown

| Desc                        | Nr |
| --------------------------- | -- |
| Client Chat                 | 0  |
| All Chat                    | 1  |
| Client Center               | 2  |
| All Center                  | 3  |
| Client Chat & Client Center | 4  |
| Client Chat & All Center    | 5  |
| All Chat & Client Center    | 6  |
| All Chat & All Center      | 7  |

**Description**: What the Description for the command should be
