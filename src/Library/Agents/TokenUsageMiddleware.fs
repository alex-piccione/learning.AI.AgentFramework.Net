namespace Middleware.TokenUsageMiddleware

open System
open System.Threading
open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open Microsoft.Extensions.Logging
open Helper
open Middlewares

(*
Agent Run middleware for count used tokens
*)

type TokenUsageMiddleware (logger:ILogger, logDetails:bool) =

    let mutable tokens:int64 = 0L

    member _.UsedTokens = tokens

    interface IAgentRunMiddleware with

        member this.RunAsync (messages:ChatMessage seq) (session:AgentSession|null) (options:AgentRunOptions|null) (agent:AIAgent) ct = task {

            // Execute the Call to Agent
            let! response = agent.RunAsync(messages, session, options, ct)

            match response.Usage with
            | null -> ()
            | usage ->
                let usedTokens = getValueOrDefault usage.TotalTokenCount 0L

                if usedTokens > 0 then Interlocked.Add(&tokens, usedTokens) |> ignore

                if logDetails = false then
                    logger.LogDebug($"Agent call used tokens: {usedTokens}. Total used tokens: {tokens}.")
                else
                    let number (nullable:Nullable<int64>) =
                        match nullable.HasValue with
                        | false -> "   N/A"
                        | true -> nullable.Value.ToString().PadLeft 6

                    let data =
                        $"""
| Token Type | Count  |
|------------|--------|
| Input      | {number usage.InputTokenCount} |
| Output     | {number usage.OutputTokenCount} |
| Reasoning  | {number usage.ReasoningTokenCount} |
| Total      | {number usage.TotalTokenCount} |

Additional Tokens:
{match usage.AdditionalCounts with
 | null -> "None"
 | c when c.Count = 0  -> "None"
 | counts -> String.concat "\n" [for kvp in counts -> $"- {kvp.Key}: {kvp.Value}"]}
"""

                    logger.LogDebug data

            return response
         }