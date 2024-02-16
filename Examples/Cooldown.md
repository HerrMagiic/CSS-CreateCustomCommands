## Cooldown for commands

Here are two ways to add a cooldown for a command.

The simple command is just a cooldown for the current player who used the command

In the Advanced command example you can add a "IsGlobal" flag so the whole server has a cooldown for this command not just the Player who uses it


```json
[
    //Simple cooldown command
    {
        "Title": "SimpleCooldown",
        "Description": "Cooldown example command",
        "Command": "cooldown",
        "Cooldown": 10, // Cooldown in seconds
        "Message": "Cool cooldown message!",
        "PrintTo": 0
    },
    //Advanced cooldown command
    {
        "Title": "AdvCooldown",
        "Description": "Cooldown example command",
        "Command": "cooldown",
        "Cooldown": {
            "CooldownTime": 5, // required, Cooldown in seconds
            "IsGlobal": false, // If true, cooldown will be global for all users
            "CooldownMessage": "This command is on cooldown for {TIME} more seconds!"
        },
        "Message": "Cool cooldown message!",
        "PrintTo": 0
    }
]
```
