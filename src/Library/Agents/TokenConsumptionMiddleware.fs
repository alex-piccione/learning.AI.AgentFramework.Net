module TokenConsumptionMiddleware

open System
open System.Threading
open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open Helper

(*
Agent Run middleware for count used tokens
*)

let mutable tokens:int64 = 0L

let internal run (messages:ChatMessage seq) (session:AgentSession|null) (options:AgentRunOptions|null) (agent:AIAgent) ct = task {

    // Call to Agent execution
    let! response = agent.RunAsync(messages, session, options, ct)

    let usedTokens = 
        match response.Usage with
        | null -> 0L
        | usage -> getValueOrDefault (usage.TotalTokenCount) 0L

    Interlocked.Add(&tokens, usedTokens) |> ignore

    Console.WriteLine($"Used tokens: {usedTokens}. Total tokens: {tokens}.")

    return response

    (*
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
    *)
}

