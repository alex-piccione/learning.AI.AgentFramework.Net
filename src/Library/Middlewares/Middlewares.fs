namespace Middlewares

open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open System.Threading
open System.Threading.Tasks
open System

type IAgentMiddleware =
    // runFunc: Func<ChatMessage seq, (AgentSession | null), (AgentRunOptions | null), AIAgent, CancellationToken, Task<AgentResponse>>
    abstract member Run:
        messages: ChatMessage seq ->
        session: AgentSession|null ->
        options: AgentRunOptions|null ->
        agent: AIAgent ->
        ct: CancellationToken ->
        Task<AgentResponse>

    // runStreamingFunc: Func<ChatMessage seq, (AgentSession | null), (AgentRunOptions | null), AIAgent, CancellationToken, IAsyncEnumerable<AgentResponseUpdate>>
    //abstract member RunStreaming


/// Middleware for Tools call. ONLY available for ChatClientAgent.
type IFunctionMiddleware =
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


/// Middleware for the IChatClient
type IChatClientMiddleware =
    // getResponseFunc: Func<ChatMessage seq,(ChatOptions | null), IChatClient, CancellationToken, Task<ChatResponse>>
    abstract member GetResponse : 
        messages: ChatMessage seq  ->
        options: ChatOptions | null  ->
        chatClient: IChatClient ->
        ct: CancellationToken ->
        Task<ChatResponse>

    // getStreamingResponseFunc: Func<ChatMessage seq,(ChatOptions | null), IChatClient, CancellationToken, IAsyncEnumerable<ChatResponseUpdate>>