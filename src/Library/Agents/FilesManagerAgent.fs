namespace Agents

open Microsoft.Extensions.AI
open Microsoft.Agents.AI
open Tools.helper
open Tools
open Clients
open System

type FilesManagerAgent (agent:AIAgent, clientWrapper:ClientWrapper) =
    inherit ChatAgentBase(agent, clientWrapper)

    static let definition:AgentDefinition = {
        Name="Files Manager"
        //Description="Provides inventory, counts and management for file and directories of the specified root folder."
        Description = "Expert at inventorying, counting, and managing files/directories within a specific root folder."
        Instructions = """
            - CORE RULE: You only have access to the path returned by 'GetRootFolder' (the ROOT FOLDER).
            - RULES:
              - You ALWAYS get the relative path to ROOT FOLDER required by the tools, calling "GetReleativePath".
              - If a user asks about this specific path (or any sub-path), use the available tools.              
              - To get the full inventory for counting or searching, try to use 'GetTree'.
              - Ig 'GetTree' returns tha there atre too many items, you have to use use 'ListFiles' and 'ListDirectories' multiple times. 
              - If a user looks for something in a folder, by default search also in the sub-folders, unless specifically asked to not do it.
              - If there is a doubt about an istruction or an info, you ask to the user for clarification.
        """
        // When the user asks for an absolute path, you have to check if it is IN the root path of this agent first (use GetRootFolder),
        //then, if it is valid, you need to obtain the relative path and use that to call the tools.


        (*

        - PATHS: 
          - Path Handling: ALWAYS convert paths (absolute and relative) to relative paths based on the ROOT FOLDER,
            e.g. if the path is "C:\aaa\bbb" or "C/aaa/bbb" and the ROOT FOLDER is "C:\aaa", you have to use just "bbb".
          - Always use forward slashes (/) for tool arguments.
          - Remember that the tools expect a relative path from the root folder.
            For example, if ROOT FOLDER is C:/aaa/bbb and you need to look at C:/aaa/bbb/ccc just us "ccc" as relative path for the tools argument.

        static let definition: AgentDefinition = {
        Name = "Files Manager"
        Description = "Expert at inventorying, counting, and managing files/directories within a specific root folder."
        Instructions = """
            - CORE RULE: You only have access to the path returned by 'GetRootFolder'.
            - WORKFLOW:
              1. Call 'GetRootFolder' to identify your boundary.
              2. Compare the user's requested path with the Root Folder.
              3. If the paths match or the user asks for "this folder", use "." as the relative path.
              4. If the path is valid, convert it to a relative path and call 'GetTree'.
            - COUNTING: To answer "how many" or "list all," always call 'GetTree' and process the returned list.
            - PATHS: Always use forward slashes (/) for tool arguments.
        """
}
        *)

        (*
        static let definition: AgentDefinition = {
            Name = "Files Manager"
            // Optimization: Add "Count" and "Inventory" to the description
            Description = "Provides inventory, counts, and management for files and directories in the root folder."
            
            Instructions = """
                Your root folder is "T:\Torrent\Completed".
                - To get the full inventory for counting or searching, use 'GetTree'.
                - Important: You are responsible for counting the items returned by the tools to answer "how many" questions.
            """
        }
        *)
    }

    let mutable tools = Unchecked.defaultof<AITool list>

    interface IChatAgent

    static member Definition  with get () = definition

    static member CreateTools(logger, rootFolder) =
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

        let getReleativePathTool : AITool =
            let getReleativePath = Func<string, string>(fun path ->  // Func to get the required Delegate
                path.Replace(rootFolder, "") )

            AIFunctionFactory.Create(
                getReleativePath,
                "GetReleativePath",
                """Returns the relative path to the ROOT FOLDER.
                Use this to get the relative path to use in agent tools.
                """,
                null
            ) :> AITool

        asList [
           FileExplorerTools(logger, rootFolder).GetTools()
           FileManagerTools(logger, rootFolder).GetTools()
           [|rootFolderTool; getReleativePathTool|]
        ]
