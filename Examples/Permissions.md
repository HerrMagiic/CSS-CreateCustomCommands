## Permissions

Adding Permission check who can execute the command.

This is just like adding Permissions for player just for the command

[How it works:](https://docs.cssharp.dev/docs/admin-framework/defining-admins.html)

```json
[
    {
        "Title": "Ping",
        "Description": "Send back Pong! to the user",
        "Command": "ping",
        "Message": "Pong!",
        "PrintTo": 0,
        "Permission": {
            "RequiresPermissionOr": false, // true = Requires one of the permissions in the list | false = Requires all of the permissions in the list
            "PermissionList": [
                "@css/cvar", // normal css permissions
                "@custom/permission", // create your own permissions
                "#css/simple-admin" // needs to be in the simple-admin group
            ]
        }
    }
]
```
