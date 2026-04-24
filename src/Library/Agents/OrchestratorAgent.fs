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
    cryptoAgent:IChatAgent) 
    = 
    inherit ToolsBase(logger)

    [<Description("Answer a question about the weather.")>]
    member __.AskWeather (question, ct) = 
        weatherAgent.Ask (question, ct)

    [<Description("Answer a question about cryptocurrencies. It has the tools to access realtime exchanges rates, Kraken, Wise and other financial stuff.")>]
    member __.AskCrypto (question, ct) = 
        cryptoAgent.Ask (question, ct)


type OrchestratorAgent private (agent:AIAgent, clientWrapper) =
    inherit ChatAgentBase(agent, clientWrapper)

    static member Create (logger, agentBuilder:AgentBuilder, client:ClientWrapper, ct): Task<IChatAgent> = task {

        let agentSettings:AgentSettings = {
            Name = "Orchestrator"
            Description = "Manages user request and sunb-agents."
            Instructions = """
                You are a chereful agent.
                You use your tool to answer user questions.
                For questions about currency exchange rates, Kraken exchange and financial info, use the AskCrypto tool.

                If you think you don't have the tool for the task, list your tools to the user in JSON.
            """
        }

        let tools = 
            OrchestratorTools(
            logger,
            agentBuilder.CreateWeatherAgent(client),
            agentBuilder.CreateCryptocurrencyAgent(client) 
                ).GetTools()

        let! agent = agentBuilder.CreateAgent(agentSettings, client, tools)

        return OrchestratorAgent (agent, client) :> IChatAgent
    }
