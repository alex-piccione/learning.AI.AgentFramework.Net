namespace Middleware.AgentCallTelemetryMiddleware

open System
open System.Threading
open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open Microsoft.Extensions.Logging
open Helper
open Middlewares

(*
Agent Run middleware for count used tokens and execution time
*)

type LogType =
    | None
    | Simple
    | Detailed

type AgentCallTelemetryMiddleware (logger:ILogger, logType:LogType) =

    let mutable tokens:int64 = 0L
    let mutable agentCallsCount:int64 = 0L
    let mutable agentCallsExecutionMilliseconds:int64 = 0L

    member _.UsedTokens = tokens
    member _.CallsCount = agentCallsCount
    member _.CallsExecutionTime = TimeSpan.FromMilliseconds agentCallsExecutionMilliseconds

    interface IAgentRunMiddleware with

        member this.RunAsync (messages:ChatMessage seq) (session:AgentSession|null) (options:AgentRunOptions|null) (agent:AIAgent) ct = task {

            let crono = System.Diagnostics.Stopwatch.StartNew()

            // Execute the Call to Agent
            let! response = agent.RunAsync(messages, session, options, ct)

            let elapsedMiliseconds = crono.ElapsedMilliseconds
            Interlocked.Add(&agentCallsExecutionMilliseconds, elapsedMiliseconds) |> ignore
            Interlocked.Add(&agentCallsCount, 1) |> ignore

            match response.Usage with
            | null -> ()
            | usage ->
                let usedTokens = getValueOrDefault usage.TotalTokenCount 0L

                if usedTokens > 0 then Interlocked.Add(&tokens, usedTokens) |> ignore

                match logType with
                | None -> ()
                | Simple -> 
                    logger.LogDebug($"Agent call used tokens: {usedTokens}. Total used tokens: {tokens}.")
                    logger.LogDebug($"Agent call execution time: {elapsedMiliseconds}ms. Total execution time: {this.CallsExecutionTime}.")
                | _ -> 
                    let number (nullable:Nullable<int64>) =
                        match nullable.HasValue with
                        | false -> "   N/A"
                        | true -> nullable.Value.ToString().PadLeft 6

                    let data = $"""
| Token Type | Count  |
|------------|--------|
| Input      | {number usage.InputTokenCount} |
| Output     | {number usage.OutputTokenCount} |
| Reasoning  | {number usage.ReasoningTokenCount} |
| Total      | {number usage.TotalTokenCount} |

Additional Tokens: {match usage.AdditionalCounts with
                    | null -> "None"
                    | c when c.Count = 0  -> "None"
                    | counts -> String.concat "\n" [for kvp in counts -> $"- {kvp.Key}: {kvp.Value}"]}
"""

                    logger.LogDebug data
                    logger.LogDebug($"Agent call execution time: {elapsedMiliseconds}ms. Total execution time: {this.CallsExecutionTime}.")

            return response
         }