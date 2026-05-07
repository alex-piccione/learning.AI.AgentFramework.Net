namespace RootFolderTestBase

open System
open System.IO
open NUnit.Framework
open Microsoft.Extensions.Logging.Testing;
open Swensen.Unquote
open Tools

/// Helper  for creating test files
type Helper (rootPath:string) =

    let normalizePath (path:string) = path.Replace("\\", "/")

    /// filePath can be absolute or relative to the root folder
    /// It returns a normalized path
    member _.CreateFile(filePath:string, ?content:string) =

        let filePath =
            if Path.IsPathRooted filePath then filePath
            else Path.Combine(rootPath, filePath)

        let directory = Path.GetDirectoryName (Path.Combine(rootPath, filePath))
        match directory with
        | null -> ()
        | dir when not (String.IsNullOrWhiteSpace dir) && not (Directory.Exists dir) ->
            Directory.CreateDirectory dir |> ignore
        | _ -> ()
        use stream = File.Create (filePath)
        let bytes = Text.Encoding.UTF8.GetBytes(content |> Option.defaultValue "")
        stream.Write(bytes, 0, bytes.Length)

        normalizePath filePath

    /// dirPath can be absolute or relative to the root folder
    /// It returns a normalized path
    member _.CreateDir(dirPath:string) =

        let dirPath =
            if Path.IsPathRooted dirPath then dirPath
            else Path.Combine(rootPath, dirPath)

        Directory.CreateDirectory dirPath |> ignore

        normalizePath dirPath

/// Helper module providing common test infrastructure
module TestHelpers =

    let logger = FakeLogger()

    let createTestEnvironment () =
        let testDir = Path.Combine(Path.GetTempPath(), $"Test_DirectoryExplorerTools_{Guid.NewGuid().ToString()[..6]}")
        Console.WriteLine($"Test dir: {testDir}")
        Directory.CreateDirectory(testDir) |> ignore
        let directoryExplorerTools = FileExplorerTools(logger, testDir)
        let fileManagerTools = FileManagerTools(logger, testDir)
        (testDir, directoryExplorerTools, fileManagerTools)

    let cleanupTestEnvironment (testDir: string) =
        if Directory.Exists(testDir) then
            try Directory.Delete(testDir, true)
            with _ -> ()

/// Base class providing common test infrastructure for all FileExplorerTools tests
/// Provides SetUp, TearDown, and Tools instance
[<AbstractClass>]
type TestBase() =

    let mutable testDir = ""
    let mutable fileExplorerTools = Unchecked.defaultof<FileExplorerTools>
    let mutable fileManagerTools = Unchecked.defaultof<FileManagerTools>

    member _.TestDir = testDir
    member _.FileExplorerTools = fileExplorerTools
    member _.FileManagerTools = fileManagerTools

    [<SetUp>]
    member this.Setup() =
        let (dir, fileExplorerTools_, fileManagerTools_) = TestHelpers.createTestEnvironment ()
        testDir <- dir
        fileExplorerTools <- fileExplorerTools_
        fileManagerTools <- fileManagerTools_

    [<TearDown>]
    member this.Teardown() =
        TestHelpers.cleanupTestEnvironment testDir

/// Abstract base class providing common path validation test cases for FileExplorerTools methods
/// Inherits from TestBase to get SetUp/TearDown and Tools
/// Derived classes must implement GetOperation to specify which method to test
[<AbstractClass>]
type PathValidationTestBase() =
    inherit TestBase()

    /// Override this to return the operation to test (e.g., fun path -> tools.ReadFile(path))
    abstract member GetOperation : unit -> (string -> unit)

    // ========== Common path validation test cases ==========

    [<TestCase("")>]
    [<TestCase("   ")>]
    member this.``should reject empty or whitespace paths`` (path: string) =
        let operation = this.GetOperation ()
        raisesWith<ArgumentException>
            <@ operation path @>
            (fun ex -> <@ ex.Message = "Path cannot be empty or whitespace." @>)

    [<TestCase("C:\\Windows\\file")>]
    [<TestCase("C:/Windows/file")>]
    [<TestCase("\\folder\\file")>]
    [<TestCase("C:file")>]
    [<Platform("Win")>]
    member this.``should reject paths outside the root folder (Windows)`` (path: string) =
        let operation = this.GetOperation ()
        raisesWith<ArgumentException>
            <@ operation path @>
            //(fun ex -> <@ ex.Message = $"Path '{path}' is not allowed, it MUST be in the root folder '{base.TestDir}'." @>)
            (fun ex -> <@ ex.Message.StartsWith $"Path '{path}' is not allowed, it MUST be in the root folder " @>) // can't access base !!!

    [<TestCase("/etc/file")>]
    [<TestCase("/home/user/file")>]
    [<Platform("Unix,Linux,MacOsx")>]
    member this.``should reject paths outside the root folder (Unix)`` (path: string) =
        let operation = this.GetOperation ()
        raisesWith<ArgumentException>
            <@ operation path @>
            (fun ex -> <@ ex.Message.StartsWith $"Path '{path}' is not allowed, it MUST be in the root folder " @>) // can't access base !!!

    [<TestCase("../file")>]
    [<TestCase("aaa/../../file")>]
    member this.``should reject path traversal with '..'`` (path: string) =
        let operation = this.GetOperation ()
        raisesWith<UnauthorizedAccessException>
            <@ operation path @>
            (fun ex -> <@ ex.Message = $"Path '{path}' is not allowed." @>)

    [<Test>]
    member this.``should reject paths with null bytes`` () =
        let operation = this.GetOperation ()
        let invalidPath = "path\0file"
        raisesWith<UnauthorizedAccessException>
            <@ operation invalidPath @>
            (fun ex -> <@ ex.Message = $"Path '{invalidPath}' contains a null byte." @>)

    [<TestCase("%2e%2e/file")>]
    [<TestCase("..%2f/file")>]
    [<TestCase("%2e%2e%2f%2e%2etc")>]
    [<TestCase("aaa%2f..%2f..%2fetc")>]
    [<TestCase("%2e%2e\\file")>]
    [<TestCase("%2e%2e%5cfile")>]
    member this.``should reject URL-encoded path traversal`` (path: string) =
        let operation = this.GetOperation ()
        raisesWith<UnauthorizedAccessException>
            <@ operation path @>
            (fun ex -> <@ ex.Message = $"Path '{path}' contains URL-encoded characters and is not allowed." @>)