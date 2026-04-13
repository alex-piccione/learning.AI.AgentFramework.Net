module Helper

open Microsoft.Extensions.Configuration
open System.Runtime.CompilerServices
open Microsoft.Extensions.AI

let renderUsage (usage:UsageDetails) = 
    let output = $"""
| Token Type | Count |
|-----|-----|
| Input Tokens | {if usage.InputTokenCount.HasValue then string usage.InputTokenCount.Value else "N/A"} |
| Output Tokens | {if usage.OutputTokenCount.HasValue then string usage.OutputTokenCount.Value else "N/A"} |
| Reasoning Tokens | {if usage.ReasoningTokenCount.HasValue then string usage.ReasoningTokenCount.Value else "N/A"} |
| Total Tokens | {if usage.TotalTokenCount.HasValue then string usage.TotalTokenCount.Value else "N/A"} |

Additional Token Counts:
{match usage.AdditionalCounts with
 | null -> "None"
 | counts -> String.concat "\n" [for kvp in counts -> $"- {kvp.Key}: {kvp.Value}"]}
"""
    try 
        // TODO: avoid exception to be printed on Console

        // this one is always printed on console
        //task {
        //    do! ConsoleMarkdownRenderer.Displayer.DisplayMarkdownAsync(output)
        //} |> Async.AwaitTask |> Async.RunSynchronously

        // this one is always printed on console
        //async {
        //    do! ConsoleMarkdownRenderer.Displayer.DisplayMarkdownAsync(output) |> Async.AwaitTask
        //} |> Async.RunSynchronously

        System.Console.WriteLine output
    with ex ->
        System.Console.WriteLine $"ConsoleMarkdownRenderer.Displayer.DisplayMarkdownAsync failed. {ex}"
        System.Console.WriteLine output


[<Extension>]
type ConfigurationRootExtensions =

    /// Gets required config value or fails with descriptive error.
    /// Usage: config.Require("mykey")
    [<Extension>]
    static member Get(this: IConfigurationRoot, key: string) : string =
        match this[key] with
        | null -> failwithf "Missing required config: %s" key
        | value -> value

    /// Gets required config value as 'T or fails.
    /// Usage: config.RequireAs<int>("Port")
    [<Extension>]
    static member GetAs<'T>(this: IConfigurationRoot, key: string) : 'T =
        let value = this.Get(key)
        match System.Convert.ChangeType(value, typeof<'T>) with
        | (:? 'T as result) -> result
        | _ -> failwithf "Invalid config type for %s: expected %s" key (typeof<'T>.Name)


/// AwaitTask + RunSynchronously
let runTask task = task |> Async.AwaitTask |> Async.RunSynchronously