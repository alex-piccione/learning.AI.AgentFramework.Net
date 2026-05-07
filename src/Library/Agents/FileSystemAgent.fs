namespace Agents

open Microsoft.Extensions.AI
open Microsoft.Agents.AI
open Tools.helper
open Tools
open Clients
open System
open System.IO

type FilesManagerAgent_v2 (agent:AIAgent, clientWrapper:ClientWrapper) =
    inherit ChatAgentBase(agent, clientWrapper)

    static let definition:AgentDefinition = {
        Name="File System"
        Description = "Expert at inventorying, counting, and managing files/directories within a specific root folder."
        Instructions = $"""
            - CORE RULE: You only have access to the path returned by 'GetRootFolder' (the ROOT FOLDER).
            - RULES:
              - If a user asks about this specific path (or any sub-path), use the available tools.
              - IMPORTANT. Use ListDirectories' recursively to get the sub-folders of a directory.
              - When search in sub-folders the search don't stop at first level but go trough all the sub-folders until "ListDirectories" returns an empty list.
              - If not specified, assume the user wants to look into sub-folders.
        """
    }


    let mutable tools = Unchecked.defaultof<AITool list>

    interface IChatAgent

    static member Definition  with get () = definition

    static member CreateTools(logger, rootFolder) =

        // convert Windows path style (C:\\aaa) to Unix style (C:/aaa).
        let unixRootFolder = Path.GetFullPath(rootFolder).Replace('\\', '/')

        let rootFolderTool : AITool =
            let getRootFolder = Func<string>(fun () -> unixRootFolder)  // Func to get the required Delegate

            AIFunctionFactory.Create(
                getRootFolder,
                "GetRootFolder",
                """Returns the root folder path used by the tools of this agent.
                Use this to check immediately if you can use the other tools on some path.
                """,
                null
            ) :> AITool

        asList [
           FileExplorerTools(logger, unixRootFolder).GetTools()
           //FileManagerTools(logger, rootFolder).GetTools()
           [|rootFolderTool|]
        ]