namespace Agents

open System.Threading
open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open Tools
open Helper
open Clients

type WeatherAgent (name, client:ClientWrapper, toolsProvider:ToolsProvider) =
    //inherit AgentBase(name, client)

    let instructions = """
        You are an expert metereologist.
    """

    let tools = toolsProvider.OpenMeteoTools
    let agent = client.ChatClient.AsAIAgent(instructions, name, "Retrieve info about the weather.", tools)
    let session = agent.CreateSessionAsync().AsTask() |> runTask

    member _.Ask (question:string, ct:CancellationToken) = task {
        let options:AgentRunOptions = AgentRunOptions()
        return! agent.RunAsync(question, session, options, ct)
    }

    interface IAgent with
        member _.Name = name
        member this.LlmModel = client.Info.LlmModel
        member this.LlmProvider = client.Info.LlmProvider

    interface IChatAgent with
        member _.Ask(question, ct) = task {
            let options:AgentRunOptions = AgentRunOptions()
            return! agent.RunAsync(question, session, options, ct)
        }