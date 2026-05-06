namespace Tools

open System
open System.ComponentModel
open System.IO
open System.Threading.Tasks
open Microsoft.Extensions.Logging

type FileManagerTools (logger:ILogger, rootFolder:string) =
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

    [<Description("Create a new file or overwrite existing file with the given content. Creates parent directories if they don't exist.")>]
    member this.WriteTextFile(
        [<Description("Relative path to the file from the root folder, e.g. 'src/config.json' or 'tests/data/sample.txt'")>]
        filePath: string,
        [<Description("Content to write to the file")>]
        content: string): Task = task {

            this.ValidatePath filePath
            let fullPath = Path.Combine(rootFolder, filePath)
            let parentDir = Path.GetDirectoryName fullPath
            match parentDir with
            | null -> ()
            | dir when not (String.IsNullOrWhiteSpace dir) && not (Directory.Exists dir) ->
                Directory.CreateDirectory dir |> ignore
            | _ -> ()

            do! File.WriteAllTextAsync(fullPath, content)
        }