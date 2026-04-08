namespace Agents.Cryptocurrency

open System.Threading
open Microsoft.Extensions.Logging
open Microsoft.Agents.AI
open Microsoft.Extensions.AI

open Tools.Kraken
open Tools.CoingeckoTools
open Tools.ToolsBase
open Tools.Wise

type CryptocurrencyAgent (
    logger:ILogger,
    chatClient:IChatClient,
    krakenPublicKey:string,
    krakenPrivateKey:string,
    coingeckoApiKey:string,
    wiseApiKey:string
    ) =

    let krakenTools = KrakenTools(logger, krakenPublicKey, krakenPrivateKey).GetTools()
    let coingeckoTools = CoingeckoTools(logger, coingeckoApiKey).GetTools()
    let wiseTools = WiseTools(logger, wiseApiKey).GetTools()

    let tools = asList [krakenTools; coingeckoTools; wiseTools]

    // select a settings
    let settings = Agents.Settings.Cryptocurrency.V3

    let agent = chatClient.AsAIAgent(settings.Instructions, settings.Name, settings.Description, tools)
    let session = agent.CreateSessionAsync().AsTask() |> Async.AwaitTask |> Async.RunSynchronously

    member _.Name = settings.Name
    member _.Instructions = settings.Instructions

    member _.Ask (question:string, ct:CancellationToken) = task {
        let options:AgentRunOptions = AgentRunOptions()
        let! response = agent.RunAsync(question, session, options, ct)
        return response
    }