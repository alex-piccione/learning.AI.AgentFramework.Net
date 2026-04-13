namespace Tools.Time

open System.ComponentModel

[<Description("Date and Time tools.")>]
type TimeTools () =

    [<Description("The current datetime offset.")>]
    static member GetDateTime () =
        System.DateTimeOffset.Now.ToString()



