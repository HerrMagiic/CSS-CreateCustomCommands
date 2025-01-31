## Execute Client Commands

If you want to execute Commands for the client here are to different ways of doing it.

#### Execute Client Command:

This means you are simulating typing the command in the players console. Be aware this only works for commands that can be executed by the client.

Example:

```json
[
    {
        "Title": "Open Buy menu",
        "Description": "Opens the buy menu for the player",
        "Command": "buy",
        "Message": "Opening buy Menu...",
        "PrintTo": 0,
        "ClientCommands": [
            "buymenu"
        ]
    }
]
```


#### Execute Client Command from Server:

Issue the specified command directly from the server (mimics the server executing the command with the given player context).

Works with server commands like `kill`, `explode`, `noclip`, etc.

Example:

```json
[
    {
        "Title": "Noclip",
        "Description": "Noclips player",
        "Command": "noclip",
        "Message": "Noclip...",
        "PrintTo": 0,
        "ClientCommandsFromServer ": [
            "noclip"
        ]
    }
]
```


### Important!

Its always recommended to Permission to the command so not every player can use the command and execute commands on your server
