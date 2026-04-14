module Middlewares

open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open System.Threading
open System.Threading.Tasks
open System

type IAgentRunMiddleware =
    abstract member RunAsync: ChatMessage seq -> AgentSession|null -> AgentRunOptions|null -> AIAgent -> CancellationToken -> Task<AgentResponse>
    