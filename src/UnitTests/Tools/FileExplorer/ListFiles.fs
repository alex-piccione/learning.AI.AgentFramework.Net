namespace UnitTests.Tools.DirectoryExplorer_v2

open System
open System.IO
open NUnit.Framework
open Swensen.Unquote

open RootFolderTestBase

type ListFiles () =
    inherit PathValidationTestBase()

    let mutable helper = Helper("")

    override this.GetOperation () =
        let tools = base.FileExplorerTools
        fun path -> tools.ListFiles(path) |> ignore

    [<SetUp>]
    member _.SetupAdditional() =
        helper <- Helper(base.TestDir)

    // ========== Specific ListFiles tests ==========

    [<Test>]
    member _.``ListFiles in root`` () =
        let file_a = helper.CreateFile("a.txt")
        let file_b = helper.CreateFile("b.txt")
        
        let files = base.FileExplorerTools.ListFiles(base.TestDir) |> Set.ofSeq

        test <@ files = Set.ofSeq [file_a; file_b] @>

    [<TestCase("dir")>]
    [<TestCase("sub dir")>]
    [<TestCase("sub DIR")>]
    member _.``ListFiles in subdirectory`` (dirName) =

        let sub_dir = Path.Combine(base.TestDir, dirName)
        let file_a = helper.CreateFile (Path.Combine(sub_dir, "a.txt"))
        let file_b = helper.CreateFile (Path.Combine(sub_dir, "b.txt"))

        let files = base.FileExplorerTools.ListFiles(sub_dir) |> Set.ofSeq

        test <@ files = Set.ofSeq [file_a; file_b] @>

    [<Test>]
    member _.``ListFiles handles file names with spaces`` () =
        let file_a = helper.CreateFile("my file.txt")
        let file_b = helper.CreateFile("a file with spaces.dat")

        let files = base.FileExplorerTools.ListFiles(base.TestDir) |> Set.ofSeq

        test <@ files = Set.ofSeq [file_a; file_b] @>

    [<Test>]
    member _.``ListFiles does not recurse into subdirectories`` () =
        let file_a = helper.CreateFile("a.txt")
        let _ = helper.CreateFile("sub/nested.txt")

        let files = base.FileExplorerTools.ListFiles(base.TestDir) |> Set.ofSeq

        test <@ files = Set.ofList [file_a] @>

    [<Test>]
    member _.``ListFiles on empty directory returns empty`` () =
        let files = base.FileExplorerTools.ListFiles(base.TestDir)

        test <@ Seq.isEmpty files @>

    [<Test>]
    member _.``ListFiles when directory does not exist`` () =

        let directory = Path.Combine(base.TestDir, "not_exist_dir")

        raisesWith<DirectoryNotFoundException>
            <@ base.FileExplorerTools.ListFiles directory @>
            (fun ex -> <@ ex.Message = $"Directory '{directory}' does not exist." @>)

