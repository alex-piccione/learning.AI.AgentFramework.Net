namespace UnitTests.Tools.DirectoryExplorer_v2

open System
open System.IO
open NUnit.Framework
open Swensen.Unquote

open RootFolderTestBase_v2
open Utils
open PathNormalizer

type ListFiles () =
    inherit PathValidationTestBase()

    let mutable helper = Helper("")

    override this.GetOperation () =
        let tools = base.DirectoryExplorerTools
        fun path -> tools.ListFiles(path) |> ignore

    [<SetUp>]
    member _.SetupAdditional() =
        helper <- Helper(base.TestDir)

    // ========== Specific ListFiles tests ==========

    [<Test>]
    member _.``ListFiles in root`` () =
        let file_a = helper.CreateFile("a.txt")
        let file_b = helper.CreateFile("b.txt")
        
        let files = base.DirectoryExplorerTools.ListFiles(base.TestDir) |> List.ofSeq

        test <@ files = [file_a; file_b] @>

    [<TestCase("dir")>]
    [<TestCase("sub dir")>]
    [<TestCase("sub DIR")>]
    member _.``ListFiles in subdirectory`` (dirName) =

        let sub_dir = Path.Combine(base.TestDir, dirName)
        let file_a = Path.Combine(sub_dir, "a.txt")
        let file_b = Path.Combine(sub_dir, "b.txt")
        helper.CreateFile_bak(file_a,"")
        File.WriteAllText(file_b,"")

        let files = base.DirectoryExplorerTools.ListFiles(sub_dir)

        test <@ files = [(asWin file_a); (asWin file_b)] @>
        test <@ files |> Seq.contains (asWin file_a) @>
        test <@ files |> Seq.contains (asWin file_b) @>

    [<Test>]
    member _.``ListFiles does not recurse into subdirectories`` () =
        let file_a = helper.CreateFile("a.txt")
        let _ = helper.CreateFile("sub/nested.txt")

        let files = base.DirectoryExplorerTools.ListFiles(base.TestDir) |> Seq.toList

        test <@ files = [file_a] @>

    [<Test>]
    member _.``ListFiles on empty directory returns empty`` () =
        let files = base.DirectoryExplorerTools.ListFiles(base.TestDir)

        test <@ Seq.isEmpty files @>

    [<Test>]
    member _.``ListFiles when directory does not exist`` () =

        let directory = Path.Combine(base.TestDir, "not_exist_dir")

        raisesWith<DirectoryNotFoundException>
            <@ base.DirectoryExplorerTools.ListFiles directory @>
            (fun ex -> <@ ex.Message = $"Directory '{directory}' does not exist." @>)

