## New Lines

Here are two ways of breaking the message into two or more lines.


```json
[
    {
        "Title": "Steam group",
        "Description": "Link for steam groups",
        "Command": "steam,group,steamgroup",
        "Message": "{PREFIX}Steamgroup: \nhttps://steamcommunity.com/groups/OrizonSurf", // \n is a new line
        "PrintTo": 0
    },
    {
        "Title": "Numbers",
        "Description": "From one to ten",
        "Command": "numbers",
        "Message": "1\n2\n3\n4\n5\n6\n7\n8\n9\r\n10", // \n or \r is a new line
        "PrintTo": 0
    },
    {
        "Title": "Numbers",
        "Description": "From one to ten with arrays as new lines",
        "Command": "numbers2",
        "Message": [ // If you want to use arrays as new lines, do this
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"
        ],
        "PrintTo": 0
    }
]
```
