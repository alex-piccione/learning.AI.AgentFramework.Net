namespace Agents

open Microsoft.Agents.AI
open Microsoft.Extensions.AI

open Tools
open Clients

type CryptocurrencyAgent private (agent:AIAgent, client) =
    inherit ChatAgentBase(agent, client)

    new(client:ClientWrapper, toolsProvider:ToolsProvider) =
        let settings = Agents.Settings.Cryptocurrency.V3
        let tools = Array.concat [
            toolsProvider.CoingeckoTools
            toolsProvider.KrakenTools
            toolsProvider.WiseTools // for exchange rates with Fiat
        ]
        let agent:AIAgent = client.ChatClient.AsAIAgent(settings.Instructions, settings.Name, settings.Description, tools)

        CryptocurrencyAgent(agent, client)
