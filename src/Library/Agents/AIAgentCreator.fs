namespace Agents

open System
open System.Threading.Tasks
open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open Middlewares
open OllamaSharp

open ChatClientFactory

type AgentDefinition = {
    Name: string
    Description: string
    Instructions: string
}

type LocalOllama = {URL:string}
type OpenAICompatibleData = {URL:string; apiKey:string}
type OpenAICompatibleKnownProvider = {knownProvider:LLMProvider; apiKey:string}


type LLMService =
    | LocalOllama of LocalOllama
    | OpenAICompatible of OpenAICompatibleData
    | KnownProvider of OpenAICompatibleKnownProvider

/// Helper to create AIAgent with middlewares
// TODO: set internal
type AIAgentCreator() =

    let mutable agentMiddlewares:IAgentMiddleware list = []
    let mutable functionMiddlewares:IFunctionMiddleware list = []
    let mutable chatClientMiddlewares:IChatClientMiddleware list = []

    member this.AddAgentMiddleware (middleware: IAgentMiddleware) : AIAgentCreator =
        agentMiddlewares <- middleware::agentMiddlewares
        this

    member this.AddFunctionMiddleware (middleware: IFunctionMiddleware) : AIAgentCreator =
        functionMiddlewares <- middleware::functionMiddlewares
        this

    member this.AddChatClientMiddleware (middleware: IChatClientMiddleware) : AIAgentCreator =
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

    // use OllamaSharp to create the AIAgent
    member this.Create(definition:AgentDefinition, llmService:LLMService, model:string, tools) : Task<AIAgent> = task {

        match llmService with 
        | LocalOllama ollama -> 
            let config = OllamaApiClient.Configuration()
            config.Uri <- Uri(ollama.URL)
            config.Model <- model
            let client = new OllamaApiClient(config)

            //let options = 
            let agent = client.AsAIAgent(definition.Instructions, definition.Name, definition.Description)
            return agent

        | OpenAICompatible openai ->

            let credentials = ClientModel.ApiKeyCredential openai.apiKey
            let options = OpenAI.OpenAIClientOptions()
            options.Endpoint <- Uri openai.URL

            let chatClient = OpenAI.Chat.ChatClient(model, credentials, options).AsIChatClient()
            let! agent = this.Create(definition, chatClient, tools)
            return agent
            
        | KnownProvider knownProvider ->
            let url = 
                match knownProvider.knownProvider with
                | LLMProvider.AliBaba -> Constants.LLMProviders.ALIBABA_URL
                | LLMProvider.AliBabaPlan -> Constants.LLMProviders.ALIBABA_PLAN_URL
                | LLMProvider.DeepSeek -> Constants.LLMProviders.DEEPSEEK_URL
                | LLMProvider.GitHub -> Constants.LLMProviders.GITHUB_URL
                | LLMProvider.Mistral -> Constants.LLMProviders.MISTRAL_URL
                | LLMProvider.Openrouter -> Constants.LLMProviders.OPENROUTER_URL
                | LLMProvider.Xiaomi -> Constants.LLMProviders.XIAOMI_URL
                | LLMProvider.Google -> Constants.LLMProviders.GOOGLE_URL

            let serviceData = OpenAICompatible({ URL=url; apiKey=knownProvider.apiKey})

            let! agent = this.Create(definition, serviceData, model, tools)
            return agent
    }