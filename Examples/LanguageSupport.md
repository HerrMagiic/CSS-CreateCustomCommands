## Language support for Commands

If you have a server with players from all over the world you probably want a way to output the message into multiple languages.

Here in the example you can see there is a {LANG} tag. The 'test' after the LANG= points to the language tag in CustomCommands/lang.


```json
[
    {
        "Title": "Sentance",
        "Description": "Send a sentance",
        "Command": "sentance",
        // Note that you also need to add the test tag in the language files in CustomCommands/lang
        "Message": "{LANG=test} test",
        "PrintTo": 2,
        "CenterMessage": {
            "Message": "{LANG=test}",
            "Time": 10
        }
    }
]
```
