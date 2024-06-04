## Toggling Server Commands

Do you dont want to create two commands for switching Server Commands on and of here is a way to do this easier.

If you add toggle befor the command it will toggle the value. If it was 1 it will be 0 and if it was 0 it will be 1
if you add toggle to the start of the command you dont need to add a number after it
Be aware this only works with commands that have 0 or 1 as a value!

```json
[
    {
        "Title": "Toggle sv_cheats",
        "Description": "Command for toggling sv_cheats",
        "Command": "cheats",
        "Message": "Toggled sv_cheats",
        "PrintTo": 0,
        "ServerCommands": [
            "toggle sv_cheats" 
        ],
        "Permission": {
            "RequiresPermissionOr": false,
            "PermissionList": [
                "#css/moderator"
            ]
        }
    }
]
```
