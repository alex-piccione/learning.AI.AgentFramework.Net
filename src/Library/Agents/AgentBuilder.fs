namespace Agents

open System.Threading.Tasks
open Tools
open Clients

type AgentBuilder(aiAgentHelper:AIAgentHelper, toolsProvider:ToolsProvider) =

    member this.CreateFilesManagerAgent(logger, clientWrapper:ClientWrapper, rootFolder): Task<FilesManagerAgent_v2> = task {
        let tools = FilesManagerAgent_v2.CreateTools(logger, rootFolder)
        let! aiAgent = aiAgentHelper.Create(FilesManagerAgent_v2.Definition, clientWrapper.ChatClient, tools)
        return FilesManagerAgent_v2(aiAgent, clientWrapper)
    }

    member this.CreateFilesManagerAgent_v1(logger, clientWrapper:ClientWrapper, rootFolder): Task<FilesManagerAgent> = task {
        let tools = FilesManagerAgent.CreateTools(logger, rootFolder)
        let! aiAgent = aiAgentHelper.Create(FilesManagerAgent.Definition, clientWrapper.ChatClient, tools)
        return FilesManagerAgent(aiAgent, clientWrapper)
    }

    member _.CreateWeatherAgent(chatClient) =
        WeatherAgent("Weather Agent", chatClient, toolsProvider)

    member _.CreateCryptocurrencyAgent(clientWrapper) =
        CryptocurrencyAgent(clientWrapper, toolsProvider)