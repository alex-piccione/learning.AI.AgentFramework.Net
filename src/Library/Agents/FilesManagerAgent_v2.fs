namespace Agents

open Microsoft.Extensions.AI
open Microsoft.Agents.AI
open Tools.helper
open Tools
open Clients
open System

type FilesManagerAgent_v2 (agent:AIAgent, clientWrapper:ClientWrapper) =
    inherit ChatAgentBase(agent, clientWrapper)

    static let definition:AgentDefinition = {
        Name="Files Manager"
        //Description="Provides inventory, counts and management for file and directories of the specified root folder."
        Description = "Expert at inventorying, counting, and managing files/directories within a specific root folder."
        Instructions = $"""
            - CORE RULE: You only have access to the path returned by 'GetRootFolder' (the ROOT FOLDER).
            - RULES:
              - If a user asks about this specific path (or any sub-path), use the available tools.
              - IMPORTANT. Use ListDirectories' to get the sub-folders of a directory, when user ask for sub-folder search.
              - If not specified, assume the user wants to look into sub-folders.
        """
    }

    (*
    - PATHS: 
      - Always use forward slashes (/) for tool arguments.
    *)

    let mutable tools = Unchecked.defaultof<AITool list>

    interface IChatAgent

    static member Definition  with get () = definition

    static member CreateTools(logger, rootFolder) =

        // TODO convert Windows path (C:\\aaa) to Unix one (C:/aaa).
        //let unixRootFolder = Unchecked.

        let rootFolderTool : AITool =
            let getRootFolder = Func<string>(fun () -> rootFolder)  // Func to get the required Delegate

            AIFunctionFactory.Create(
                getRootFolder,
                "GetRootFolder",
                """Returns the root folder path used by the tools of this agent.
                Use this to check immediately if you can use the tools on some path.
                """,
                null
            ) :> AITool

        asList [
           DirectoryExplorerTools_v2(logger, rootFolder).GetTools()
           //FileManagerTools(logger, rootFolder).GetTools()
           [|rootFolderTool|]
        ]
