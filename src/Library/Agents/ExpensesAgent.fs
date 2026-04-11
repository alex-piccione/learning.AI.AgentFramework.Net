namespace Agents.Expenses

open System.Threading
open Microsoft.Extensions.AI
open Microsoft.Extensions.Logging
open Tools.helper
open Tools.Expenses
open Microsoft.Agents.AI


type ExpensesAgent (logger:ILogger, loggerFactory, chatClient:IChatClient, expensesMcpServerUrl) =

    let expensesTools = ExpensesTools.CreateAsync(logger, expensesMcpServerUrl, loggerFactory) |> Async.AwaitTask |> Async.RunSynchronously
    let tools = asList [expensesTools.GetTools()]

    let name = "Expenses"
    let description = "Agent for managing the Expenses registry of hte user."
    let instructions = """
    You use the available tools for:
    - Add expenses records
    - List existing expenses
    - Edit expenses records
    - Delete expenses records

    If there is a doubt about an istruction or an info, you ask the user for clarification.
    """

    let agent = chatClient.AsAIAgent(instructions, name, description, tools)
    let session = agent.CreateSessionAsync().AsTask() |> Async.AwaitTask |> Async.RunSynchronously

    member _.Ask (question:string, ct:CancellationToken) = task {
        let options:AgentRunOptions = AgentRunOptions()
        let! response = agent.RunAsync(question, session, options, ct)
        return response
    }

