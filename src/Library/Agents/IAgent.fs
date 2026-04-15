namespace Agents 

open System.Threading
open System.Threading.Tasks
open Microsoft.Agents.AI
open Clients
open Helper

type IAgent =
    abstract member Name:string
    abstract member LlmModel:string
    abstract member LlmProvider:string

type IChatAgent =
    inherit IAgent
    abstract member Ask: string * CancellationToken -> Task<AgentResponse>

[<AbstractClass>]
type AgentBase(agent:AIAgent, clientWrapper:ClientWrapper) =

    interface IAgent with
        member _.Name = 
            match agent.Name with
            | null -> agent.Id
            | name -> name
        member _.LlmModel = clientWrapper.Info.LlmModel
        member _.LlmProvider = clientWrapper.Info.LlmProvider

[<AbstractClass>]
type ChatAgentBase(agent:AIAgent, clientWrapper:ClientWrapper) =

    let session = agent.CreateSessionAsync().AsTask() |> runTask
    let options = AgentRunOptions()

    // customization
    //do 
    //    options.ResponseFormat <- Microsoft.Extensions.AI.ChatResponseFormat.Text

    interface IAgent with
        member _.Name = 
            match agent.Name with
            | null -> agent.Id
            | name -> name
        member _.LlmModel = clientWrapper.Info.LlmModel
        member _.LlmProvider = clientWrapper.Info.LlmProvider

    interface IChatAgent with
        member _.Ask(question, ct) = task {
            return! agent.RunAsync(question, session, options, ct)
        }