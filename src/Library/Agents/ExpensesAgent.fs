namespace Agents

open Microsoft.Extensions.AI
open Microsoft.Extensions.Logging
open Microsoft.Agents.AI
open Tools.helper
open Tools.Expenses

type ExpensesAgent private (agent, session) =
   
    static member Create (logger:ILogger, loggerFactory, chatClient:IChatClient, expensesMcpServerUrl) = task {
        let! expensesTools = ExpensesTools.CreateAsync(logger, expensesMcpServerUrl, loggerFactory)
        let tools = asList [expensesTools.GetTools()]

        let name = "Expenses"
        let description = "Manages the Registry of the user's Expenses."
        let instructions = """
        You use the available tools for:
        - Add expenses records
        - List existing expenses
        - Edit expenses records
        - Delete expenses records

        If there is a doubt about an istruction or an info, you ask to the user for clarification.
        """

        let agent = chatClient.AsAIAgent(instructions, name, description, tools)
        let! session = agent.CreateSessionAsync().AsTask()

        return ExpensesAgent(agent, session)
    }

    member _.Ask (question:string, ct) = task {
        let options:AgentRunOptions = AgentRunOptions()
        return! agent.RunAsync(question, session, options, ct)
    }

