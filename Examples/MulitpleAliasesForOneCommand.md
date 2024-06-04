## Multiple Aliases for one Command

Here is a way of creating aliases for a command.

For example there are many ways a user would try to get the discord link. Using a "," or ";" solve the issue


```json
[
    {
        "Title": "Discord",
        "Description": "Command for Discord link",
        "Command": "discord,dc", // User can type !discord or /discord, !dc or /dc and !Discord or /discord in chat
        "Message": "{PREFIX}Discord: https://discord.gg/Z9JfJ9Y57C",
        "PrintTo": 0
    }
]
```
