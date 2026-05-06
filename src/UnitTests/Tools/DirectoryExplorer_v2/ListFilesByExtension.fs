namespace UnitTests.Tools.DirectoryExplorer_v2

open System
open System.IO
open NUnit.Framework
open Swensen.Unquote

open RootFolderTestBase_v2

type ListFilesByExtension () =
    inherit PathValidationTestBase()

    let mutable helper = Helper("")

    override this.GetOperation () =
        let tools = base.DirectoryExplorerTools
        fun path -> tools.ListFilesByExtension(path, ".txt") |> Seq.length |> ignore

    [<SetUp>]
    member _.SetupAdditional() =
        helper <- Helper(base.TestDir)

    // ========== Specific ListFilesByExtension tests ==========

    [<Test>]
    member _.``ListFilesByExtension in root`` () =
        helper.CreateFile_bak("a.txt", "")
        helper.CreateFile_bak("b.csv", "")
        helper.CreateFile_bak("c.txt", "")

        let files = base.DirectoryExplorerTools.ListFilesByExtension(".", ".txt") |> Seq.map Path.GetFileName |> Set.ofSeq

        test <@ files = Set.ofList ["a.txt"; "c.txt"] @>

    [<Test>]
    member _.``ListFilesByExtension in subdirectory`` () =
        helper.CreateFile_bak("sub/a.txt", "")
        helper.CreateFile_bak("sub/b.csv", "")
        helper.CreateFile_bak("sub/c.txt", "")

        let files = base.DirectoryExplorerTools.ListFilesByExtension("sub", ".txt") |> Seq.map Path.GetFileName |> Set.ofSeq

        test <@ files = Set.ofList ["a.txt"; "c.txt"] @>

    [<Test>]
    member _.``ListFilesByExtension does not recurse into subdirectories`` () =
        helper.CreateFile_bak("a.txt", "")
        helper.CreateFile_bak("sub/nested.txt", "")

        let files = base.DirectoryExplorerTools.ListFilesByExtension(".", ".txt") |> Seq.map Path.GetFileName |> Set.ofSeq

        test <@ files = Set.ofList ["a.txt"] @>

    [<Test>]
    member _.``ListFilesByExtension on empty directory returns empty`` () =
        let files = base.DirectoryExplorerTools.ListFilesByExtension(".", ".txt") |> Seq.toList

        test <@ files = [] @>

    [<Test>]
    member _.``ListFilesByExtension when directory does not exist`` () =
        raisesWith<DirectoryNotFoundException>
            <@ base.DirectoryExplorerTools.ListFilesByExtension("not_exist_dir", ".txt") @>
            (fun ex -> <@ ex.Message = "Directory 'not_exist_dir' does not exist." @>)

    [<Test>]
    member _.``ListFilesByExtension with no matching files returns empty`` () =
        helper.CreateFile_bak("a.csv", "")
        helper.CreateFile_bak("b.json", "")

        let files = base.DirectoryExplorerTools.ListFilesByExtension(".", ".txt") |> Seq.toList

        test <@ files = [] @>

    [<Test>]
    member _.``ListFilesByExtension with different extension`` () =
        helper.CreateFile_bak("a.csv", "")
        helper.CreateFile_bak("b.csv", "")
        helper.CreateFile_bak("c.txt", "")

        let files = base.DirectoryExplorerTools.ListFilesByExtension(".", ".csv") |> Seq.map Path.GetFileName |> Set.ofSeq

        test <@ files = Set.ofList ["a.csv"; "b.csv"] @>