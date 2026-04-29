namespace Agents

open System.Threading.Tasks
open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open Middlewares

type AgentDefinition = {
    Name: string
    Description: string
    Instructions: string
}

/// Helper to create an AIAgent with middlewares
type AIAgentHelper () =

    let mutable agentMiddlewares:IAgentMiddleware list = []
    let mutable functionMiddlewares:IFunctionMiddleware list = []
    let mutable chatClientMiddlewares:IChatClientMiddleware list = []

    member this.AddAgentMiddleware (middleware: IAgentMiddleware) : AIAgentHelper =
        agentMiddlewares <- middleware::agentMiddlewares
        this
    
    member this.AddFunctionMiddleware (middleware: IFunctionMiddleware) : AIAgentHelper =
        functionMiddlewares <- middleware::functionMiddlewares
        this
    
    member this.AddChatClientMiddleware (middleware: IChatClientMiddleware) : AIAgentHelper =
        chatClientMiddlewares <- middleware::chatClientMiddlewares
        this

    member this.Create(settings:AgentDefinition, chatClient:IChatClient, tools): Task<AIAgent> = task {
    
           let chatClientBuilder = chatClient.AsBuilder()
               
           let chatClientBuilder =
               chatClientMiddlewares
                   |> List.fold (fun (builder:ChatClientBuilder) middleware -> builder.Use(middleware.GetResponse, null)) chatClientBuilder
    
           let chatClient = chatClientBuilder.Build()
    
           let agentBuilder = 
               chatClient.AsAIAgent(settings.Instructions, settings.Name, settings.Description, tools)
                   .AsBuilder()
    
           let agentBuilder = 
               agentMiddlewares
               |> List.fold (fun (builder:Microsoft.Agents.AI.AIAgentBuilder) middleware -> builder.Use(middleware.Run, null))  agentBuilder
    
           let agentBuilder =
               functionMiddlewares
               |> List.fold (fun (builder:Microsoft.Agents.AI.AIAgentBuilder) middleware -> builder.Use(middleware.Next)) agentBuilder
    
           return agentBuilder.Build()
       }