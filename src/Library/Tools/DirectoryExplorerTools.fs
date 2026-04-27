namespace Tools

open System
open System.ComponentModel
open System.IO
open Microsoft.Extensions.Logging

type DirectoryExplorerTools (logger:ILogger, rootFolder:string) =
    inherit RootFolderToolsBase(logger, rootFolder)

    [<Description("Reads the content of a file as text. Provide the relative file path from the root folder.")>]
    member this.ReadFile(
        [<Description("Relative path to the file from the root folder, e.g. 'src/config.json'")>]
        filePath: string): string =

        this.ValidatePath filePath
        let fullPath = Path.Combine(rootFolder, filePath)
        if not (File.Exists fullPath) then
            raise (FileNotFoundException($"File '{filePath}' does not exist."))
        File.ReadAllText fullPath

    [<Description("List all files in a given directory. Returns relative file paths from the project root (e.g., 'Program.fs'). Use '.' for the project root.")>]
    member this.ListFiles(
        [<Description("Relative path to the directory from the root folder, e.g. 'src' or 'tests/data'")>]
        directoryPath: string) : string seq =
        
        this.ValidatePath directoryPath
        let fullPath = Path.Combine(rootFolder, directoryPath)
        if not (Directory.Exists fullPath) then
            raise (DirectoryNotFoundException($"Directory '{directoryPath}' does not exist."))
        Directory.EnumerateFiles fullPath

    //[<Description("Enumerate the directories of the given directory path")>]
    //member this.ListDirectories (directory:string) =
    //    Directory.EnumerateDirectories directory

    //[<Description("Enumerate the files and directories of the given directory path")>]
    //member this.GetDirectoryTree(directory:string) =
    //    Directory.EnumerateFileSystemEntries directory

    [<Description("Get a flat tree structure of all files and directories. Returns lines prefixed with 'd ' for directories (ending with '/') and 'f ' for files with their relative paths from project root.")>]
    member this.GetTree() : string seq =
        let getFileNameSafe (path: string | null) : string option =
            if isNull path then None
            else
                let name = Path.GetFileName path
                match name with
                | null -> None
                | some -> Some some

        let rec getAllEntries (dir: string) (relativeBase: string) =
            seq {
                for subDir in Directory.GetDirectories(dir) do
                    match getFileNameSafe subDir with
                    | None -> ()
                    | Some dirName ->
                        let relativePath = 
                            if relativeBase = "" then dirName
                            else relativeBase + "/" + dirName
                        // Directory names end with slash
                        yield $"d {relativePath}/"
                        yield! getAllEntries subDir relativePath

                for file in Directory.GetFiles(dir) do
                    match getFileNameSafe file with
                    | None -> ()
                    | Some fileName ->
                        let relativePath = 
                            if relativeBase = "" then fileName
                            else relativeBase + "/" + fileName
                        // File names do not end with slash
                        yield $"f {relativePath}"
            }

        getAllEntries rootFolder ""