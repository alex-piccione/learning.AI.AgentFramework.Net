module Utils

open System.IO
open Tools.DirectoryExplorer.Models

let dirSeparator = Path.DirectorySeparatorChar.ToString()

let if_null (str: string|null) (defaultValue: string) =
    match str with
    | null -> defaultValue
    | value -> value

// NOTE. Path.GetDirectoryName returns the parent directory path, NOT the last directory of a path
let getFilePath (path:string) = if_null (Path.GetFileName path) "not set"
let getDirPath (path:string) =  if_null (Path.GetFileName path) "not set"
    //let dirName = (path.Split "/")[..1]

    //Path.GetDirectoryName
    //if_null dirName "not set"

type TreeItem with
    static member File (path) = {Type="file"; Name=getFilePath path; Path=path}
    static member Directory (path) = {Type="directory"; Name=getDirPath path; Path=path}