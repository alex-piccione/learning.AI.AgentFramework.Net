namespace Tools

open System
open System.ComponentModel
open System.IO
open Microsoft.Extensions.Logging
open Tools.DirectoryExplorer.Models

type DirectoryExplorerTools_v2 (logger:ILogger, rootFolder:string) =
    inherit RootFolderToolsBase(logger, rootFolder)

    [<Description("List all the files in the given directory.")>]
    member this.ListFiles(
        [<Description("""Path of the directory to look at.""")>]
        directoryPath: string) : 
        [<Description("List of the files found in the provided directory.")>]string seq =

        this.ValidatePath_v2 directoryPath

        if not (Directory.Exists directoryPath) then
            raise (DirectoryNotFoundException($"Directory '{directoryPath}' does not exist."))

        Directory.EnumerateFiles directoryPath

    [<Description("List all the directories in the given directory. Use this to navigate sub-folders.")>]
    member this.ListDirectories(
        [<Description("""Path of the directory to look at.""")>]
        directoryPath: string) : 
        [<Description("List of the directories found in the requested directory.")>]string seq =

        this.ValidatePath_v2 directoryPath

        if not (Directory.Exists directoryPath) then
            raise (DirectoryNotFoundException($"Directory '{directoryPath}' does not exist."))

        Directory.EnumerateDirectories directoryPath
