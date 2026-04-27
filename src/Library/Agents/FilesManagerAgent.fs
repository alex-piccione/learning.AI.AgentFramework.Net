namespace Agents

open Microsoft.Extensions.AI
open Microsoft.Agents.AI
open Tools.helper
open Tools
open Clients
open System

type FilesManagerAgent private (agent:AIAgent, clientWrapper:ClientWrapper) =
    inherit ChatAgentBase(agent, clientWrapper)

    interface IChatAgent

    new(logger, clientWrapper:ClientWrapper, rootFolder) =

        let rootFolderTool : AITool =
            let getRootFolder = Func<string>(fun () -> rootFolder)  // Func to get the required Delegate

            AIFunctionFactory.Create(
                getRootFolder,
                "GetRootFolder",
                "Returns the configured root folder path used by the Files Manager agent.",
                null
            ) :> AITool

        let tools = asList [
           DirectoryExplorerTools(logger, rootFolder).GetTools()
           FileManagerTools(logger, rootFolder).GetTools()
           [|rootFolderTool|]
        ]   

        let name = "Files Manager"
        let description = "Manages the files and directories of the specified root foilder."
        let instructions = """
            You can only manage files and directories from the provided root folder.
            You use the available tools for:
            - List files and directories
            - Write text files

            If user asks for something specific in a folder, look also in the sub-folders, unless specifically asked to not do.

            If there is a doubt about an istruction or an info, you ask to the user for clarification.
        """

        let agent:AIAgent = clientWrapper.ChatClient.AsAIAgent(instructions, name, description, tools)

        let! session = agent.CreateSessionAsync().AsTask()

        FilesManagerAgent(agent, clientWrapper)

