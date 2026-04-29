namespace Tools

open System
open System.ComponentModel
open System.IO
open Microsoft.Extensions.Logging
open Tools.DirectoryExplorer.Models

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

    [<Description("List all the files in a given directory.")>]
    member this.ListFiles(
        [<Description("""Directory relative path. IT MUST BE RELATIVE TO THE ROOT FOLDER,
        if root folder is 'C:/Aaa', 'tests/data' will result in 'C:/Aaa/tests/data'.
        Use '.' for the root folder.""")>]
        directoryPath: string) : 
        [<Description("List of the files found in the requested directory.")>]string seq =

        this.ValidatePath directoryPath
        let fullPath = Path.Combine(rootFolder, directoryPath)
        if not (Directory.Exists fullPath) then
            raise (DirectoryNotFoundException($"Directory '{directoryPath}' does not exist."))
        Directory.EnumerateFiles fullPath

    [<Description("List all the directories in a given directory.")>]
    member this.ListDirectories(
        [<Description("""
        Relative path to the directory from the root folder, e.g. if root folder is 'C:/Aaa', 'tests/data' will result in 'C:/Aaa/tests/data'.
        Use '.' for the root folder.""")>]
        directoryPath: string) : 
        [<Description("List of the directories found in the requested directory.")>]string seq =

        this.ValidatePath directoryPath
        let fullPath = Path.Combine(rootFolder, directoryPath)
        if not (Directory.Exists fullPath) then
            raise (DirectoryNotFoundException($"Directory '{directoryPath}' does not exist."))
        Directory.EnumerateDirectories fullPath

    //[<Description("Enumerate the directories of the given directory path")>]
    //member this.ListDirectories (directory:string) =
    //    Directory.EnumerateDirectories directory

    //[<Description("Enumerate the files and directories of the given directory path")>]
    //member this.GetDirectoryTree(directory:string) =
    //    Directory.EnumerateFileSystemEntries directory

    //[<Description("""
    //    Get a flat tree structure of all files and directories, at every level, of the root folder.
    //    Returns lines prefixed with 'd ' for directories (ending with '/') and 'f ' for files with their relative paths from root folder.
    //    """)>]
    member this.GetTree_back() : string seq =

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
                        // use "/" as dir separator (Unix style)
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
                        // use "/" as dir separator (Unix style)
                        let relativePath =
                            if relativeBase = "" then fileName
                            else relativeBase + "/" + fileName
                        // File names do not end with slash
                        yield $"f {relativePath}"
            }

        getAllEntries rootFolder ""

    [<Description("Returns a flat list of every file and directory within the root folder.
    Useful for answering questions about counts, totals, or to find specific files deep in the hierarchy.")>]
    member this.GetTree() : GetTreeResult =
        let MAX = 1000
        let items = ResizeArray<TreeItem>()
        let mutable limitReached = false

        let getFileNameSafe (path: string | null) : string option =
            if isNull path then None
            else
                let name = Path.GetFileName path
                match name with
                | null -> None
                | some -> Some some
    
        let rec traverse dir relativeBase =
            if not limitReached then
                // Process Directories
                for subDir in Directory.GetDirectories(dir) do
                    if not limitReached then
                        match getFileNameSafe subDir with
                        | Some dirName ->
                            let relativePath = if relativeBase = "" then dirName else $"{relativeBase}/{dirName}"
                            if items.Count >= MAX then limitReached <- true
                            else 
                                items.Add({ Type="directory"; Name=dirName; Path=relativePath })
                                traverse subDir relativePath
                        | None -> ()
    
                // Process Files
                for file in Directory.GetFiles(dir) do
                    if not limitReached then
                        match getFileNameSafe file with
                        | Some fileName ->
                            let relativePath = if relativeBase = "" then fileName else $"{relativeBase}/{fileName}"
                            if items.Count >= MAX then limitReached <- true
                            else items.Add({ Type="file"; Name=fileName; Path=relativePath })
                        | None -> ()
    
        traverse rootFolder ""
        { TooManyItems = limitReached; Items = items.ToArray()  }

        (*
    [<Description("Returns a flat list of every file and directory within the root folder.
    REQUIRED for answering questions about counts, totals, or to find specific files deep in the hierarchy.")>]
    member this.GetTree() :
        [<Description("The result of this operation. If there are more than 1000 items the operation will not be completed.")>]
        GetTreeResult =

        // TODO: if there are too many items, AI Agent cannot manage it
        // raise a specific error
        let MAX = 1000
        let mutable counter = 0

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
                        // use "/" as dir separator (Unix style)
                        let relativePath =
                            if relativeBase = "" then dirName
                            else relativeBase + "/" + dirName
                        counter <- counter + 1
                        yield {Type="directory"; Name=dirName; Path=relativePath}
                        yield! getAllEntries subDir relativePath

                for file in Directory.GetFiles(dir) do
                    match getFileNameSafe file with
                    | None -> ()
                    | Some fileName ->
                        counter <- counter + 1
                        if counter > MAX then 
                        // use "/" as dir separator (Unix style)
                        let relativePath =
                            if relativeBase = "" then fileName
                            else relativeBase + "/" + fileName
                        counter <- counter + 1
                        yield {Type="file"; Name=fileName; Path=relativePath}
            }

        getAllEntries rootFolder ""

        *)