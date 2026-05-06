namespace Tools

open System
open System.IO
open PathNormalizer

(*
This is a base type for tools that are limited to operate only in a specified root folder.
It validates the given path, to avoid entering parent folder or other not allowed directories.

It prevents traversal using ".." or escaped characters in the path.
It prevents also the use of null byte character, that potentially allows malicious code execution.

What is a Null Byte Injection Attack?
- A null byte (\0) is a special character in many programming languages and systems that signifies the end of a string.
- In some cases, attackers can inject a null byte into input data (e.g., file paths) to manipulate how the program processes strings.
- For example:
  - If a file path is constructed dynamically from user input, an attacker could inject a null byte to truncate the path prematurely.
  - This can lead to unintended behavior, such as accessing or modifying files outside the intended directory.
*)

type RootFolderToolsBase_v2 (logger, rootFolderPath:string) =
    inherit ToolsBase(logger)

    let rootFolder = normalizePath(rootFolderPath)

    do
        if not (Directory.Exists rootFolder) then
            failwith $"Directory \"{rootFolder}\" does not exist."

    member __.ValidatePath (path: string) =

        if String.IsNullOrWhiteSpace(path) then
            raise (ArgumentException("Path cannot be empty or whitespace."))

        // Reject URL-encoded characters (e.g. %2e%2e → ..)
        if path.Contains("%") then
            raise (UnauthorizedAccessException($"Path '{path}' contains URL-encoded characters and is not allowed."))

        // Check for path traversal
        if path.Contains("..") then
            raise (UnauthorizedAccessException($"Path '{path}' is not allowed."))

        // Check for null byte
        if path.Contains("\0") then
            raise (UnauthorizedAccessException($"Path '{path}' contains a null byte."))

        let path = normalizePath path

        if not (path.StartsWith(rootFolder)) then
            raise (ArgumentException($"Path '{path}' is not allowed, it MUST be in the root folder '{rootFolder}'."))


