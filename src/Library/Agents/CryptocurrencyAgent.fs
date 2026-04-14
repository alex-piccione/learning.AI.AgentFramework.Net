namespace Agents.Cryptocurrency

open Microsoft.Extensions.Logging
open Microsoft.Agents.AI
open Microsoft.Extensions.AI

open Tools.Coingecko
open Tools.helper
open Tools.Wise
open Tools.Kraken
open Middlewares

type CryptocurrencyAgent private (agent:AIAgent, session) =

    let options:AgentRunOptions = AgentRunOptions()

    static member Create(logger:ILogger,
        chatClient:IChatClient,
        krakenPublicKey,
        krakenPrivateKey,
        coingeckoApiKey,
        wiseApiKey,
        agentRunMiddlewares:IAgentRunMiddleware seq,
        callFunctionMiddlewares: IFunctionCallMiddleware seq
    ) = task {

        let krakenTools = KrakenTools(logger, krakenPublicKey, krakenPrivateKey).GetTools()
        let coingeckoTools = CoingeckoTools(logger, coingeckoApiKey).GetTools()
        let wiseTools = WiseTools(logger, wiseApiKey).GetTools()

        let tools = asList [krakenTools; coingeckoTools; wiseTools]

        // select a settings
        let settings = Agents.Settings.Cryptocurrency.V3

        let agent:AIAgent = chatClient.AsAIAgent(settings.Instructions, settings.Name, settings.Description, tools)

        let agentBuilder =
            agent.AsBuilder()
            |> Seq.fold (fun (state:AIAgentBuilder) (middleware:IAgentRunMiddleware) -> state.Use(middleware.Run, null))
            <| agentRunMiddlewares
        
        let agentBuilder =
            callFunctionMiddlewares 
            |> Seq.fold (fun (state:AIAgentBuilder) (middleware:IFunctionCallMiddleware) ->
                state.Use(callback=middleware.Next)
            ) agentBuilder

        let agent = agentBuilder.Build()

        // add middleware
        //let agentBuilder = agent.AsBuilder()
        //let agent =
            //(middlewares |> Seq.fold (fun state middleware -> state.Use(middleware.RunAsync, null)) (agent.AsBuilder()) )
          //  agent.AsBuilder()
          //      .Use(tokenUsage.RunAsync, null)
                //.UseLogging()
          //      .Build()

        let! session = agent.CreateSessionAsync().AsTask()

        return CryptocurrencyAgent(agent, session)
    }

    member _.Ask (question:string, ct) = task {
        return! agent.RunAsync(question, session, options, ct)
    }