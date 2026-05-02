namespace Agents

open System.Threading.Tasks
open Tools
open Clients

// TODO: add ChatClientOptions to define how to auto-build the ChatClient

type AgentFactory(aiAgentCreator:AIAgentCreator, toolsProvider:ToolsProvider) =

    // TODO: move here the AiAgentCreator middleware utilities and hide AiAgentCreator
    //new (toolsProvider:ToolsProvider) =
    //    let aiAgentCreator = AIAgentCreator()
    //    AgentFactory(aiAgentCreator, toolsProvider)

    member this.CreateFilesManagerAgent(logger, clientWrapper:ClientWrapper, rootFolder): Task<FilesManagerAgent_v2> = task {
        let tools = FilesManagerAgent_v2.CreateTools(logger, rootFolder)
        let! aiAgent = aiAgentCreator.Create(FilesManagerAgent_v2.Definition, clientWrapper.ChatClient, tools)
        return FilesManagerAgent_v2(aiAgent, clientWrapper)
    }

    member this.CreateFilesManagerAgent_v1(logger, clientWrapper:ClientWrapper, rootFolder): Task<FilesManagerAgent> = task {
        let tools = FilesManagerAgent.CreateTools(logger, rootFolder)
        let! aiAgent = aiAgentCreator.Create(FilesManagerAgent.Definition, clientWrapper.ChatClient, tools)
        return FilesManagerAgent(aiAgent, clientWrapper)
    }

    member _.CreateWeatherAgent(clientWrapper) =
        WeatherAgent("Weather Agent", clientWrapper, toolsProvider)

    member _.CreateCryptocurrencyAgent(clientWrapper) =
        CryptocurrencyAgent(clientWrapper, toolsProvider)

