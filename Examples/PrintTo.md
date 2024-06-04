## Print To / Decide where the message gets displayed

Here are all combinations listed where you can print the message.

The default values are

0 - Client Chat = Only the player who executed the command gets the message

1 - All Chat = The whole server gets the message when a player executes the command

2 - Client Center = The player who executed the command gets a message in the center

3 - All Center = The whole server gets the message to the center when a player executes the command

```json
[
    {
        "Title": "Client Chat",
        "Description": "Client Chat message",
        "Command": "clientchat",
        "Message": "ClientChat",
        "PrintTo": 0
    },
    {
        "Title": "All Chat",
        "Description": "All Chat message",
        "Command": "allchat",
        "Message": "All Chat",
        "PrintTo": 1
    },
    {
        "Title": "Client Center",
        "Description": "Client Center message",
        "Command": "clientcenter",
        "CenterMessage": {
            "Message": "Client Center",
            "Time": 5 // Seconds
        },
        "PrintTo": 2
    },
    {
        "Title": "All Center",
        "Description": "All players Center message",
        "Command": "allcenter",
        "CenterMessage": {
            "Message": "All Center",
            "Time": 7
        },
        "PrintTo": 3
    },
    {
        "Title": "Client Chat & Client Center",
        "Description": "Client Chat & Client Center message",
        "Command": "CCCC",
        "Message": "Client Chat",
        "CenterMessage": {
            "Message": "Client Center",
            "Time": 10
        },
        "PrintTo": 4
    },
    {
        "Title": "Client Chat & All Center",
        "Description": "Client Chat & All Center",
        "Command": "CCAC",
        "Message": "Client Chat",
        "CenterMessage": {
            "Message": "All Center",
            "Time": 3
        },
        "PrintTo": 5
    },
    {
        "Title": "All Chat & Client Center",
        "Description": "All Chat & Client Center",
        "Command": "ACCC",
        "Message": "All Chat",
        "CenterMessage": {
            "Message": "Client Center",
            "Time": 4
        },
        "PrintTo": 6
    },
    {
        "Title": "All Chat & All Center",
        "Description": "All Chat & All Center",
        "Command": "ACAC",
        "Message": "All Chat",
        "CenterMessage": {
            "Message": "All Center",
            "Time": 10
        },
        "PrintTo": 7
    }
]
```
