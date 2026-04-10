namespace Tools

open System.Reflection
open System.ComponentModel
open Microsoft.Extensions.AI
open Microsoft.Extensions.Logging

type ToolsBase (logger:ILogger) =
    abstract GetTools: unit -> AITool seq // System.Collections.Generic.IList<AITool>
    abstract LogCall: string -> string option -> unit
    abstract LogError: string -> exn -> unit

    default this.GetTools() =
        this.GetType().GetMethods(BindingFlags.Public ||| BindingFlags.Instance)
        |> Seq.choose (fun m ->
            // Only pick methods that have a Description attribute
            let attr = m.GetCustomAttribute<DescriptionAttribute>()
            if isNull attr then None
            else
                // Create the AIFunction and upcast to AITool
                Some (AIFunctionFactory.Create(m, this) :> AITool)
        )

    default this.LogCall method info =
        if logger.IsEnabled LogLevel.Debug then
            match info with
            | None      -> logger.LogInformation($"{this.GetType().Name} | Call to {method} | Start")
            | Some info -> logger.LogInformation($"{this.GetType().Name} | Call to {method} | Start | {info}")

    default this.LogError method ex =
        logger.LogError($"{this.GetType().Name} | Failed to call {method} | {ex}")


module helper =

    /// Convert a sequence of AITool sequence to a List that is required for the Agent contructor
    let asList (tools:AITool seq seq) =
        let finalSeq = tools |> Seq.fold (fun state tools -> Seq.append state tools) Seq.empty<AITool>
        System.Collections.Generic.List<AITool>(finalSeq)