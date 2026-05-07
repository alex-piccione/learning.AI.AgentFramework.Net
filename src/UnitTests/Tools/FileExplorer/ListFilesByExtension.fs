namespace UnitTests.Tools.DirectoryExplorer_v2

open System
open System.IO
open NUnit.Framework
open Swensen.Unquote

open RootFolderTestBase

type ListFilesByExtension () =
    inherit PathValidationTestBase()

    let mutable helper = Helper("")

    let sameSequence s1 s2 = 
        Set.ofSeq s1 = Set.ofSeq s2

    override this.GetOperation () =
        let tools = base.FileExplorerTools
        fun path -> tools.ListFilesByExtension(path, ".txt") |> Seq.length |> ignore

    [<SetUp>]
    member _.SetupAdditional() =
        helper <- Helper(base.TestDir)

    // ========== Specific ListFilesByExtension tests ==========

    [<Test>]
    member _.``ListFilesByExtension in root`` () =
        // note, not in alphabetic order
        let file_z = helper.CreateFile "Z.txt"
        let file_a = helper.CreateFile "a.txt"
        let file_b = helper.CreateFile "b.csv"
        let file_c = helper.CreateFile "c.txt"

        let files = base.FileExplorerTools.ListFilesByExtension(base.TestDir, ".txt")

        test <@ Set.ofSeq files = Set.ofList [file_a; file_c; file_z] @>

    [<TestCase("subdir")>]
    [<TestCase("sub dir")>]
    [<TestCase("sub DIR")>]
    member _.``ListFilesByExtension in subdirectory`` (sub_dir) =

        let subDirPath = Path.Combine(base.TestDir, sub_dir)

        // note, not in alphabetic order
        let file_z = helper.CreateFile $"{sub_dir}/z.txt"
        let file_c = helper.CreateFile $"{sub_dir}/c.csv"
        let file_a = helper.CreateFile $"{sub_dir}/a.txt"

        let files = base.FileExplorerTools.ListFilesByExtension(subDirPath, ".txt")

        test <@ sameSequence files [file_a; file_z] @>

    [<Test>]
    member _.``ListFilesByExtension handles file names with spaces`` () =
        let file_a = helper.CreateFile "my file.txt"
        let file_b = helper.CreateFile "another file.txt"
        let file_c = helper.CreateFile "no match.csv"

        let files = base.FileExplorerTools.ListFilesByExtension(base.TestDir, ".txt")

        test <@ sameSequence files [file_a; file_b] @>

    [<Test>]
    member _.``ListFilesByExtension does not recurse into subdirectories`` () =
        let file_a = helper.CreateFile "a.txt"
        let _ = helper.CreateFile "sub/nested.txt"

        let files = base.FileExplorerTools.ListFilesByExtension(base.TestDir, ".txt")

        test <@ sameSequence files [file_a] @>

    [<Test>]
    member _.``ListFilesByExtension on empty directory returns empty`` () =
        let files = base.FileExplorerTools.ListFilesByExtension(base.TestDir, ".txt")

        test <@ Seq.isEmpty files @>

    [<Test>]
    member _.``ListFilesByExtension when directory does not exist`` () =

        let directory = Path.Combine(base.TestDir, "not_exist_dir")

        raisesWith<DirectoryNotFoundException>
            <@ base.FileExplorerTools.ListFilesByExtension(directory, ".txt") @>
            (fun ex -> <@ ex.Message = $"Directory '{directory}' does not exist." @>)

    [<Test>]
    member _.``ListFilesByExtension with no matching files returns empty`` () =
        let _ = helper.CreateFile "a.csv"
        let _ = helper.CreateFile "b.json"

        let files = base.FileExplorerTools.ListFilesByExtension(base.TestDir, ".txt")

        test <@ Seq.isEmpty files @>