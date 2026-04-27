namespace Agents

open System.Threading.Tasks
open System.ComponentModel
open Microsoft.Agents.AI
open Tools
open Agents
open Clients


type OrchestratorTools (
    logger, 
    weatherAgent:IChatAgent, 
    cryptoAgent:IChatAgent,
    ?filesAgent: FilesManagerAgent)
    = 
    inherit ToolsBase(logger)

    [<Description("Weather tool. Answer a question about the weather.")>]
    member __.AskWeather (question, ct) = 
        weatherAgent.Ask (question, ct)

    [<Description("Crypto tool. Answer a question about cryptocurrencies. It has the tools to access realtime exchanges rates, Kraken, Wise and other financial stuff.")>]
    member __.AskCrypto (question, ct) = 
        cryptoAgent.Ask (question, ct)


    [<Description("Files and Directories tool. It has the tools to access files and directories of the provided root folder.")>]
    member _.AskFiles(question, ct) =
        match filesAgent with
        | Some agent -> (agent :> IChatAgent).Ask(question, ct)
        | None -> invalidOp "Files tool is not configured."


type OrchestratorAgent private (agent:AIAgent, clientWrapper) =
    inherit ChatAgentBase(agent, clientWrapper)

    static member Create (logger, agentBuilder:AgentBuilder, clientWrapper:ClientWrapper, rootFolder:string option, ?ct): Task<IChatAgent> = task {

        let agentSettings:AgentSettings = {
            Name = "Orchestrator"
            Description = "Manages user request and sub-agents."
            Instructions = """
                You are a chereful agent.
                You use your tools to answer the user questions.
                For questions about currency exchange rates, Kraken exchange and financial info, use the AskCrypto tool.

                If you think you don't have the right tool for the task, list your tools to the user, format the list in JSON
            """
        }

        let fileAgentOpt =
            match rootFolder with 
            | None -> None
            | Some folder -> Some(agentBuilder.CreateFilesManagerAgent(logger, clientWrapper, folder))

        let tools = 
            OrchestratorTools(
                logger,
                agentBuilder.CreateWeatherAgent(clientWrapper),
                agentBuilder.CreateCryptocurrencyAgent(clientWrapper),
                ?filesAgent=fileAgentOpt
            ).GetTools()

        let! agent = agentBuilder.CreateAgent(agentSettings, clientWrapper, tools)

        return OrchestratorAgent (agent, clientWrapper) :> IChatAgent
    }

    // TODO: implement a Builder strategy
    //member _.AddFileManager(rootFolder) =
    //    let fileManager = FileManagerAgent(logger, clientWrapper, rootFolder)
    //    let file_Tools = fileManager.

    //    let new_tools = tools;

    //    let! agent = agentBuilder.CreateAgent(agentSettings, client, tools)

    ///    return OrchestratorAgent (agent, client, logger) :> IChatAgent
