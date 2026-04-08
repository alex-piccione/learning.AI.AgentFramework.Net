module UnitTests.Tools.Models.OpenMeteo

open NUnit.Framework
open Swensen.Unquote

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ``Forecast response is deserialized`` () =
    let json = resource_helper.readResource "openmeteo forecast.response.json"
    let response = Tools.OpenMeteo.Models.deserialize<Tools.OpenMeteo.Models.Forecast.Response> json
    test <@ response.current.rain = 0.00 @>

[<Test>]
let ``List contains element`` () =
    let list = [1; 2; 3]
    test <@ List.contains 2 list @>