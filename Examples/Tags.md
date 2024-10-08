## Tags for Dynamic Command

Here is a list of tags you can add to the message. Its also possible to use some tags with the combination of Executing commands.

The tags are always in {TAG} 


```json
[
    {
        "Title": "Prefix",
        "Description": "Displays Prefix",
        "Command": "prefix",
        "Message": "This is the prefix from the plugin: {PREFIX}", 
        "PrintTo": 0
    },
    {
        "Title": "Map Name",
        "Description": "Displays Map Name",
        "Command": "mapname",
        "Message": "Current mapname: {MAPNAME}",
        "PrintTo": 0
    },
    {
        "Title": "Time",
        "Description": "Displays the Time",
        "Command": "time",
        "Message": "Current time: {TIME}",
        "PrintTo": 0
    },
    {
        "Title": "Date",
        "Description": "Displays the Date",
        "Command": "date",
        "Message": "Current date: {DATE}",
        "PrintTo": 0
    },
    {
        "Title": "PlayerName",
        "Description": "Displays the player name of the player who executed the command",
        "Command": "playername",
        "Message": "Current user: {PLAYERNAME}",
        "PrintTo": 0
    },
    {
        "Title": "STEAMIDs",
        "Description": "Displays STEAMIDs of the current player",
        "Command": "steamids",
        "Message": "STEAMID2: {STEAMID2} STEAMID3: {STEAMID3} STEAMID32: {STEAMID32} STEAMID64: {STEAMID64}",
        "PrintTo": 0
    },
    {
        "Title": "Servername",
        "Description": "Displays Servername",
        "Command": "servername",
        "Message": "Current servername: {SERVERNAME}",
        "PrintTo": 0
    },
    {
        "Title": "IP",
        "Description": "Displays IP",
        "Command": "ip",
        "Message": "Current serverip: {IP}",
        "PrintTo": 0
    },
    {
        "Title": "Port",
        "Description": "Displays Port",
        "Command": "port",
        "Message": "Current port: {PORT}",
        "PrintTo": 0
    },
    {
        "Title": "MAXPLAYERS",
        "Description": "Displays Maxplayers of the server",
        "Command": "maxplayers",
        "Message": "Maxplayers: {MAXPLAYERS}",
        "PrintTo": 0
    },
    {
        "Title": "Player Count",
        "Description": "Displays Player Count of the server",
        "Command": "playercount",
        "Message": "Current playercount: {PLAYERS}",
        "PrintTo": 0
    },
    {
        "Title": "UserID",
        "Description": "Displays Player #userid",
        "Command": "userid",
        "Message": "Current players userid: {USERID}", // If you want to use this tag for commands add a # before the tag
        "PrintTo": 0
    }
    },
    {
        "Title": "Random number",
        "Description": "Displays a random number",
        "Command": "random",
        "Message": "This is a random number: {RNDNO=(10,20)}", // This generates a random number between 10 and 20
        "PrintTo": 0
    }
]
```
