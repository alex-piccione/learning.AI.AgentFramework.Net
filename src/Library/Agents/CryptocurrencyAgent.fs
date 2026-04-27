namespace Agents

open Microsoft.Agents.AI
open Microsoft.Extensions.AI

open Tools
open Clients

type CryptocurrencyAgent private (agent:AIAgent, clientWrapper) =
    inherit ChatAgentBase(agent, clientWrapper)

    new(clientWrapper:ClientWrapper, toolsProvider:ToolsProvider) =
        let settings = Agents.Settings.Cryptocurrency.V3
        let tools = Array.concat [
            toolsProvider.CoingeckoTools
            toolsProvider.KrakenTools
            toolsProvider.WiseTools // for exchange rates with Fiat
        ]
        let agent:AIAgent = clientWrapper.ChatClient.AsAIAgent(settings.Instructions, settings.Name, settings.Description, tools)

        CryptocurrencyAgent(agent, clientWrapper)
