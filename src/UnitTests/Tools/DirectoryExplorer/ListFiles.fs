namespace UnitTests.Tools.DirectoryExplorer

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
        fun path -> tools.ListFiles(path) |> Seq.length |> ignore

    [<SetUp>]
    member _.SetupAdditional() =
        helper <- Helper(base.TestDir)

    // ========== Specific ListFiles tests ==========

    [<Test>]
    member _.``ListFiles in root`` () =
        helper.CreateFile("a.txt", "")
        helper.CreateFile("b.txt", "")

        let files = base.DirectoryExplorerTools.ListFiles(".") |> Seq.map Path.GetFileName |> Set.ofSeq

        test <@ files = Set.ofList ["a.txt"; "b.txt"] @>

    [<Test>]
    member _.``ListFiles in subdirectory`` () =
        helper.CreateFile("sub/a.txt", "")
        helper.CreateFile("sub/b.txt", "")

        let files = base.DirectoryExplorerTools.ListFiles("sub") |> Seq.map Path.GetFileName |> Set.ofSeq

        test <@ files = Set.ofList ["a.txt"; "b.txt"] @>

    [<Test>]
    member _.``ListFiles does not recurse into subdirectories`` () =
        helper.CreateFile("a.txt", "")
        helper.CreateFile("sub/nested.txt", "")

        let files = base.DirectoryExplorerTools.ListFiles(".") |> Seq.map Path.GetFileName |> Set.ofSeq

        test <@ files = Set.ofList ["a.txt"] @>

    [<Test>]
    member _.``ListFiles on empty directory returns empty`` () =
        let files = base.DirectoryExplorerTools.ListFiles(".") |> Seq.toList

        test <@ files = [] @>

    [<Test>]
    member _.``ListFiles when directory does not exist`` () =
        raisesWith<DirectoryNotFoundException>
            <@ base.DirectoryExplorerTools.ListFiles("not_exist_dir") @>
            (fun ex -> <@ ex.Message = "Directory 'not_exist_dir' does not exist." @>)

