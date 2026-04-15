namespace Agents

open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open Middlewares
open Tools
open Clients

type AgentSettings = {
    Name: string
    Description: string
    Instructions: string
}

type AgentBuilder(toolsProvider:ToolsProvider) =

    let mutable agentMiddlewares:IAgentMiddleware list = []
    let mutable functionMiddlewares:IFunctionMiddleware list = []
    let mutable chatClientMiddlewares:IChatClientMiddleware list = []

    member this.AddAgentMiddleware (middleware: IAgentMiddleware) : AgentBuilder =
        agentMiddlewares <- middleware::agentMiddlewares
        this
    
    member this.AddFunctionMiddleware (middleware: IFunctionMiddleware) : AgentBuilder =
        functionMiddlewares <- middleware::functionMiddlewares
        this
    
    member this.AddChatClientMiddleware (middleware: IChatClientMiddleware) : AgentBuilder =
        chatClientMiddlewares <- middleware::chatClientMiddlewares
        this

    member _.CreateWeatherAgent(chatClient) =
        WeatherAgent("Weather Agent", chatClient, toolsProvider)

    member _.CreateCryptocurrencyAgent(chatClient) =
        CryptocurrencyAgent(chatClient, toolsProvider)

    member _.CreateAgent(settings:AgentSettings, client:ClientWrapper, tools) = task {

        let chatClientBuilder = 
            client.ChatClient
                .AsBuilder()
            
        let chatClientBuilder =
            chatClientMiddlewares
                |> List.fold (fun (builder:ChatClientBuilder) middleware -> builder.Use(middleware.GetResponse, null)) chatClientBuilder

        let chatClient = chatClientBuilder.Build()

        let agentBuilder = 
            chatClient.AsAIAgent(settings.Instructions, settings.Name, settings.Description, tools)
                .AsBuilder()

        let agentBuilder = 
            agentMiddlewares
            |> List.fold (fun (builder:AIAgentBuilder) middleware -> builder.Use(middleware.Run, null))  agentBuilder

        let agentBuilder =
            functionMiddlewares
            |> List.fold (fun (builder:AIAgentBuilder) middleware -> builder.Use(middleware.Next)) agentBuilder

        return agentBuilder.Build()
    }
