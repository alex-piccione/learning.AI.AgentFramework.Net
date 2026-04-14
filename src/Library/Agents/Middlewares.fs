module Middlewares

open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open System.Threading
open System.Threading.Tasks
open System

type IAgentRunMiddleware =
    // member Use: runFunc: System.Func<Microsoft.Extensions.AI.ChatMessage seq,(AgentSession | null),(AgentRunOptions | null),AIAgent,System.Threading.CancellationToken,System.Threading.Tasks.Task<AgentResponse>> | null * runStreamingFunc: System.Func<Microsoft.Extensions.AI.ChatMessage seq,(AgentSession | null),(AgentRunOptions | null),AIAgent,System.Threading.CancellationToken,System.Collections.Generic.IAsyncEnumerable<AgentResponseUpdate>> | null -> AIAgentBuilder
    abstract member Run:
        messages: ChatMessage seq ->
        session: AgentSession|null ->
        options: AgentRunOptions|null ->
        agent: AIAgent ->
        ct: CancellationToken ->
        Task<AgentResponse>

    //abstract member RunStream

type IFunctionCallMiddleware =
    abstract member NextAsFunc : 
        Func<
            AIAgent,
            FunctionInvocationContext,
            Func<FunctionInvocationContext, CancellationToken, ValueTask<obj>>,
            CancellationToken,
            ValueTask<obj>
        >

    abstract member Next:
        agent: AIAgent ->
        context: FunctionInvocationContext ->
        next: Func<FunctionInvocationContext, CancellationToken, ValueTask<obj | null>> ->
        ct: CancellationToken ->
        ValueTask<obj | null>
