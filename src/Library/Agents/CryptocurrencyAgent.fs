namespace Agents.Cryptocurrency

open Microsoft.Extensions.Logging
open Microsoft.Agents.AI
open Microsoft.Extensions.AI

open Tools.Coingecko
open Tools.helper
open Tools.Wise
open Tools.Kraken

type CryptocurrencyAgent private (agent:AIAgent, session) =

    let options:AgentRunOptions = AgentRunOptions()

    static member Create(logger:ILogger,
        chatClient:IChatClient,
        krakenPublicKey,
        krakenPrivateKey,
        coingeckoApiKey,
        wiseApiKey) = task {

        let krakenTools = KrakenTools(logger, krakenPublicKey, krakenPrivateKey).GetTools()
        let coingeckoTools = CoingeckoTools(logger, coingeckoApiKey).GetTools()
        let wiseTools = WiseTools(logger, wiseApiKey).GetTools()

        let tools = asList [krakenTools; coingeckoTools; wiseTools]

        // select a settings
        let settings = Agents.Settings.Cryptocurrency.V3

        let agent:AIAgent = chatClient.AsAIAgent(settings.Instructions, settings.Name, settings.Description, tools)

        // add middleware
        let agent = agent.AsBuilder().Use(TokenConsumptionMiddleware.run, null).Build()

        let! session = agent.CreateSessionAsync().AsTask()

        return CryptocurrencyAgent(agent, session)
    }

    member _.Ask (question:string, ct) = task {
        return! agent.RunAsync(question, session, options, ct)
    }