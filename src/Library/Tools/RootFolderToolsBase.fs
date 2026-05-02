namespace Tools

open System
open System.IO

(*
This is a base type for tools that can operato only in a specified root folder.
It validate the given path to avoid it goes on parent r other not allowed directories.

It prevent traversal using ".." or escaped characters in the path.
It prevent also the use of null byte character, that potentially allows malicious code execution.

What is a Null Byte Injection Attack?
- A null byte (\0) is a special character in many programming languages and systems that signifies the end of a string.
- In some cases, attackers can inject a null byte into input data (e.g., file paths) to manipulate how the program processes strings.
- For example:
  - If a file path is constructed dynamically from user input, an attacker could inject a null byte to truncate the path prematurely.
  - This can lead to unintended behavior, such as accessing or modifying files outside the intended directory.
*)

//[<Abstract>]
type RootFolderToolsBase(logger, rootFolder:string) =
    inherit ToolsBase(logger)

    do
        if not (Directory.Exists rootFolder) then
            failwith $"Directory \"{rootFolder}\" does not exist."

    member __.GetRelativePath (path: string) =

        if String.IsNullOrWhiteSpace(path) then
            raise (ArgumentException("Path cannot be empty or whitespace."))

        if not (path.StartsWith(rootFolder)) then
            raise (ArgumentException($"Path '{path}' is not allowed, it MUST be in the root folder '{rootFolder}'."))

        // Reject URL-encoded characters (e.g. %2e%2e → ..)
        if path.Contains("%") then
            raise (UnauthorizedAccessException($"Path '{path}' contains URL-encoded characters and is not allowed."))

        // Check for path traversal
        if path.Contains("..") then
            raise (UnauthorizedAccessException($"Path '{path}' is not allowed."))

        if path.Contains("\0") then // null byte
            raise (UnauthorizedAccessException($"Path '{path}' contains a null byte."))

        path.Replace(rootFolder, "")


    // convert Windows path style (C:\\aaa) to Unix style (C:/aaa).
    member __.GetUnixPathStyle path = Path.GetFullPath(path).Replace('\\', '/')

    member __.ValidatePath_v2 (path: string) =

        let path = __.GetUnixPathStyle path

        if String.IsNullOrWhiteSpace(path) then
            raise (ArgumentException("Path cannot be empty or whitespace."))

        if not (path.StartsWith(rootFolder)) then
            raise (ArgumentException($"Path '{path}' is not allowed, it MUST be in the root folder '{rootFolder}'."))

        // Reject URL-encoded characters (e.g. %2e%2e → ..)
        if path.Contains("%") then
            raise (UnauthorizedAccessException($"Path '{path}' contains URL-encoded characters and is not allowed."))

        // Check for path traversal
        if path.Contains("..") then
            raise (UnauthorizedAccessException($"Path '{path}' is not allowed."))

        if path.Contains("\0") then // null byte
            raise (UnauthorizedAccessException($"Path '{path}' contains a null byte."))


    member __.ValidatePath (path: string) =

        if String.IsNullOrWhiteSpace(path) then
            raise (ArgumentException("Path cannot be empty or whitespace."))

        if Path.IsPathRooted(path) then
            raise (ArgumentException($"Path '{path}' is absolute and not allowed. Please use relative paths."))

        // Reject URL-encoded characters (e.g. %2e%2e → ..)
        if path.Contains("%") then
            raise (UnauthorizedAccessException($"Path '{path}' contains URL-encoded characters and is not allowed."))

        // Check for path traversal
        if path.Contains("..") then
            raise (UnauthorizedAccessException($"Path '{path}' is not allowed."))

        if path.Contains("\0") then // null byte
            raise (UnauthorizedAccessException($"Path '{path}' contains a null byte."))