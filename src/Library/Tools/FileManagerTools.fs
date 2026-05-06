namespace Tools

open System
open System.ComponentModel
open System.IO
open System.Threading.Tasks
open Microsoft.Extensions.Logging

type FileManagerTools (logger:ILogger, rootFolder:string) =
    inherit RootFolderToolsBase_v2(logger, rootFolder)

    [<Description("Reads the content of a file as text. Provide the absolute file path.")>]
    member this.ReadFile(
        [<Description("Absolute path to the file")>]
        filePath: string): string =

        this.ValidatePath filePath
        if not (File.Exists filePath) then
            raise (FileNotFoundException($"File '{filePath}' does not exist."))
        File.ReadAllText filePath

    [<Description("Create a new file or overwrite existing file with the given content. Creates parent directories if they don't exist.")>]
    member this.WriteTextFile(
        [<Description("Absolute path to the file")>]
        filePath: string,
        [<Description("Content to write to the file")>]
        content: string): Task = task {

            this.ValidatePath filePath
            let parentDir = Path.GetDirectoryName filePath
            match parentDir with
            | null -> ()
            | dir when not (String.IsNullOrWhiteSpace dir) && not (Directory.Exists dir) ->
                Directory.CreateDirectory dir |> ignore
            | _ -> ()

            do! File.WriteAllTextAsync(filePath, content)
        }