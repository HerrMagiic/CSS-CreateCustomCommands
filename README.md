# Custom Commands Plugin Readme

## Overview

The Custom Commands Plugin allows you to create and customize commands for your server. These commands can display messages, perform server-side actions, and more. The configuration for these commands is stored in the `Commands.json` file located in the `./plugins/CustomCommands/` directory.

## Example Configuration

```json
[
  {
    "Title": "Discord",
    "Command": "discord",
    "Message": "{PREFIX}{GREEN}Discord: \n <link>",
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
  },
  {
    "Title": "Enable Surf",
    "Command": "surf",
    "Message": "Surf is now enabled",
    "PrintTo": 0,
    "Description": "Command for enabling Surf gamemode",
    "ServerCommands": [
      "sv_cheats 1",
      "sv_falldamage_scale 0",
      "sv_party_mode 1",
      "mp_freezetime 2.5",
      "mp_round_restart_delay 2.5",
      "cl_ragdoll_gravity 0",
      "sv_accelerate 10",
      "sv_airaccelerate 1400",
      "sv_gravity 800.0"
    ],
    "Permission": {
      "RequiresPermissionOr": false,
      "PermissionList": [
        "@css/cvar",
        "@custom/permission",
        "#css/simple-admin"
      ]
    }
  }
]
```

## Configuration Details

### Title

Just the title for your command. Can be anything or empty.

### Command

Specify the commands as a string, e.g., "discord" or "steam,steamgroup,group". Players can use `!discord` or `/discord` in the chat or css_discord in the console.

### Message

The message you want to send. You can use color codes like {GREEN} for green text.

### CenterMessage

The center message. For special formating use HTML

### PrintTo

Specifies where the message should be shown:

- **0**: Client Chat
- **1**: All Chat
- **2**: Client Center
- **3**: All Center
- **4**: Client Chat & Client Center
- **5**: Client Chat & All Center
- **6**: All Chat & Client Center
- **7**: All Chat & All Center

### Description

A description of what the command does. This will be shown when do css_help

### ServerCommands

An array of server commands to be executed when the command is triggered. Useful for actions like enabling specific game modes.

### Permission

Defines if the command requires specific permissions to execute:

- **ReguiresAllPermissions**
  - Set to true 	= The player needs all permissions in PermissionsList
  - Set to false 	= The player only needs one of the permissions in PermissionsList
- **PermissionList**: A list of permission flags or groups required to execute the command. [More Explenation](https://docs.cssharp.dev/docs/admin-framework/defining-admins.html)

### Colorlist

{DEFAULT}, {RED}, {LIGHTPURPLE}, {GREEN}, {LIME},
{LIGHTGREEN}, {LIGHTRED}, {GRAY}, {LIGHTOLIVE},
{OLIVE}, {LIGHTBLUE}, {BLUE}, {PURPLE}, {GRAYBLUE}



