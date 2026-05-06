namespace Tools

open System
open System.ComponentModel
open System.IO
open Microsoft.Extensions.Logging
open PathNormalizer

type FileExplorerTools (logger:ILogger, rootFolder:string) =
    inherit RootFolderToolsBase_v2(logger, rootFolder)

    [<Description("List all the files in the given directory.")>]
    member this.ListFiles(
        [<Description("Path of the directory to look at.")>]
        directoryPath: string) : 
        [<Description("List of the files found in the provided directory.")>]string seq =

        this.ValidatePath directoryPath

        if not (Directory.Exists directoryPath) then
            raise (DirectoryNotFoundException($"Directory '{directoryPath}' does not exist."))

        Directory.EnumerateFiles directoryPath |> Seq.map normalizePath

    [<Description("List all the directories in the given directory. Use this to navigate sub-folders.")>]
    member this.ListDirectories(
        [<Description("Path of the directory to look at.")>]
        directoryPath: string) : 
        [<Description("List of the directories found in the requested directory.")>]string seq =

        this.ValidatePath directoryPath

        if not (Directory.Exists directoryPath) then
            raise (DirectoryNotFoundException($"Directory '{directoryPath}' does not exist."))

        Directory.EnumerateDirectories directoryPath |> Seq.map normalizePath

    [<Description("List all files with a specific extension in the given directory.")>]
    member this.ListFilesByExtension(
        [<Description("Path of the directory to look at.")>]
        directoryPath: string,
        [<Description("File extension to filter by (e.g., '.txt', '.csv').")>]
        extension: string) :
        [<Description("List of files with the specified extension in the provided directory.")>]string seq =

        this.ValidatePath directoryPath

        if not (Directory.Exists directoryPath) then
            raise (DirectoryNotFoundException($"Directory '{directoryPath}' does not exist."))

        Directory.EnumerateFiles(directoryPath, $"*{extension}") |> Seq.map normalizePath

    
    [<Description("Returns a flat list of every directory within the given directory.
    Useful for answering questions about counts, totals, or to find specific files deep in the hierarchy.")>]
    member this.GetDirectoriesTree(
        [<Description("Path of the directory to look at.")>]
        directoryPath: string) =

        this.ValidatePath directoryPath

        let mutable items = []
   
        let rec traverse dir =
            for subDir in Directory.GetDirectories(dir) do
                items <- normalizePath(subDir)::items
                traverse subDir
    
        traverse rootFolder 
        items |> List.sort