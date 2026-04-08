namespace Agents.Wheater

open System.Threading
open Microsoft.Extensions.Logging
open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open Tools.OpenMeteoTools
open Tools.ToolsBase

type WeatherAgent (agent: AIAgent) =

    let session = agent.CreateSessionAsync().AsTask() |> Async.AwaitTask |> Async.RunSynchronously

    static member CreateChatClientUsingOpenAI(logger:ILogger, apiKey:string, model:string) =

        let instructions = """
            You are an expert metereologist.
        """
        // "You are an information agent. Answer questions cheerfully."

        let tools = [OpenMeteoTools(logger).GetTools()] |> asList

        let client = OpenAI.OpenAIClient(apiKey)
        let chatClient = client.GetChatClient(model)
        let agent = chatClient.AsIChatClient().AsAIAgent(instructions, "Weather Tool", "Retrieve info about the weather", tools)
        WeatherAgent(agent)

    member _.Ask (question:string, ct:CancellationToken) = async {
        let options:AgentRunOptions = AgentRunOptions()
        let! response = agent.RunAsync(question, session, options, ct) |> Async.AwaitTask

        return response.ToString()
    }