namespace Agents.Cryptocurrency

open Microsoft.Extensions.Logging
open Microsoft.Agents.AI
open Microsoft.Extensions.AI

open Tools.Coingecko
open Tools.helper
open Tools.Wise
open Tools.Kraken

type CryptocurrencyAgent private (agent, session) =

    let options:AgentRunOptions = AgentRunOptions()

    static member Create(logger:ILogger,
        chatClient:IChatClient,
        krakenPublicKey:string,
        krakenPrivateKey:string,
        coingeckoApiKey:string,
        wiseApiKey:string) = task {

        let krakenTools = KrakenTools(logger, krakenPublicKey, krakenPrivateKey).GetTools()
        let coingeckoTools = CoingeckoTools(logger, coingeckoApiKey).GetTools()
        let wiseTools = WiseTools(logger, wiseApiKey).GetTools()

        let tools = asList [krakenTools; coingeckoTools; wiseTools]

        // select a settings
        let settings = Agents.Settings.Cryptocurrency.V3

        let agent = chatClient.AsAIAgent(settings.Instructions, settings.Name, settings.Description, tools)
        let! session = agent.CreateSessionAsync().AsTask()

        return CryptocurrencyAgent(agent, session)
    }

    member _.Ask (question:string, ct) = task {
        return! agent.RunAsync(question, session, options, ct)
    }