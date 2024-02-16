## Center Message with color:

It is possible to add HTML to the center Message that means its possible to add any color to it. Here are some examples.


```json
[
    {
        "Title": "Center Message with color",
        "Description": "Center Message with color",
        "Command": "colorcenter",
        "CenterMessage": {
            "Message": "`<font color='#00ff00'>`Cool color here`</font>`",
            "Time": 5 // Seconds
        },
        "PrintTo": 2
    },
    {
        // HTML is possible but not css or js
        "Title": "With html",
        "Description": "With html",
        "Command": "htmlcenter",
        "CenterMessage": {
            "Message": "`<div>`Steam Group`</div><br>``<div><font color='#00ff00'>`https...`</font></div>`",
            "Time": 5 // Seconds
        },
        "PrintTo": 2
    }
]
```
