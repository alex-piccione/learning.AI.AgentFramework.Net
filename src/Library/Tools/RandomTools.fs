module Tools.Random

open System
open System.Threading.Tasks
open System.ComponentModel

type RandomTools () =

    let rng = Random()
    let colors = [| "Red"; "Blue"; "Green"; "Yellow"; "Purple" |]

    [<Description("Generates a random integer between 1 and 10 inclusive.")>]
    member __.GetRandomNumber () =
        let result = rng.Next(1, 11) // Il limite superiore è esclusivo
        result


    [<Description("Returns a random color from a predefined list of 5 colors.")>]
    member __.GetRandomColor (): Task<string> = task {
        let index = rng.Next(0, colors.Length)
        return colors[index]
    }

    [<Description("Returns the given number doubled.")>]
    member __.DoubleTheNumber number: Task<int> = task {
        return number * 2
    }