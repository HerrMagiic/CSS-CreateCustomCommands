## Execute Server Commands

If you want to execute server commands with you command this is the way of doing it. 


### Important!

Its always recommended to Permission to the command so not every player can use the command and execute commands on your server


```json
[
    {
        "Title": "Restart",
        "Description": "Command for restating the game",
        "Command": "rg,restart,restartgame",
        "Message": "Game is restarting",
        "PrintTo": 0,
        "ServerCommands": [
            "mp_restartgame 1"
        ],
        // you should add permissions to server commands for security reasons. Do it on your own risk
        "Permission": {
            "RequiresPermissionOr": false, // true = Requires one of the permissions in the list | false = Requires all of the permissions in the list
            "PermissionList": [
                "#css/moderator" // needs to be in the simple-admin group
            ]
        }
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
            "RequiresPermissionOr": true,
            "PermissionList": [
                "@css/cvar",
                "@customcommands/surf"
            ]
        }
    },
    {
        "Title": "Kickme",
        "Description": "command that kicks yourself",
        "Command": "kickme",
        "Message": "Getting Kicked",
        "PrintTo": 0,
        "ServerCommands": [
            "css_kick #{USERID}" // Tags work for commands as well. This command works with Simple Admin
        ]
    }
]
```
