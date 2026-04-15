namespace Agents

open Microsoft.Extensions.Logging
open Microsoft.Agents.AI
open Microsoft.Extensions.AI
open Tools
open Tools.helper
open Helper

type MusicistAgent (logger:ILogger, chatClient:IChatClient, googleApiKey, model) =
    let googleLyriaTools = GoogleLyriaTools(logger, googleApiKey)
    let tools = asList [googleLyriaTools.GetTools()]

    let name = "Musicist"
    let description = "Agent for create music."
    let instructions = """
    You use the available tools for create music and lyrics.
    If user ask to create a song, use the GenerateMusicClip tool.
    """

    let agent = chatClient.AsAIAgent(instructions, name, description, tools)
    let session = agent.CreateSessionAsync().AsTask() |> runTask

    member _.Ask (question:string, ct) = task {
        let options:AgentRunOptions = AgentRunOptions()
        return! agent.RunAsync(question, session, options, ct)
    }