namespace UnitTests.Tools.DirectoryExplorer

open System
open System.IO
open NUnit.Framework
open Swensen.Unquote

open Tools.DirectoryExplorer.Models
open RootFolderTestBase
open Utils


type GetTree () =
    inherit TestBase()

    [<Test>]
    member _.``GetTree returns empty for empty directory`` () =
        let result = base.FileExplorerTools.GetTree()

        test <@ result.TooManyItems = false @>
        test <@ result.Items.Length = 0 @>

    [<Test>]
    member _.``GetTree returns files in root`` () =
        File.WriteAllText(Path.Combine(base.TestDir, "file1.txt"), "content1")
        File.WriteAllText(Path.Combine(base.TestDir, "file2.txt"), "content2")

        let result = base.FileExplorerTools.GetTree()

        test <@ result.TooManyItems = false @>
        test <@ result.Items.Length = 2 @>
        test <@ result.Items |> Seq.contains (TreeItem.File "file1.txt") @>
        test <@ result.Items |> Seq.contains (TreeItem.File "file2.txt") @>

    [<Test>]
    member _.``GetTree returns directories in root`` () =
        Directory.CreateDirectory(Path.Combine(base.TestDir, "subdir")) |> ignore

        let result = base.FileExplorerTools.GetTree()

        test <@ result.TooManyItems = false @>
        test <@ result.Items.Length = 1 @>
        test <@ result.Items[0] = TreeItem.Directory "subdir" @>

    [<Test>]
    member _.``GetTree returns nested structure`` () =
        Directory.CreateDirectory(Path.Combine(base.TestDir, "src/Core")) |> ignore
        File.WriteAllText(Path.Combine(base.TestDir, "src/Core/Library.fs"), "code")
        File.WriteAllText(Path.Combine(base.TestDir, "README.md"), "readme")

        let result = base.FileExplorerTools.GetTree()

        test <@ result.TooManyItems = false @>
        test <@ result.Items.Length = 4 @>
        test <@ result.Items |> Seq.contains (TreeItem.Directory "src") @>
        test <@ result.Items |> Seq.contains (TreeItem.Directory "src/Core") @>
        test <@ result.Items |> Seq.contains (TreeItem.File "src/Core/Library.fs") @>
        test <@ result.Items |> Seq.contains (TreeItem.File "README.md") @>

    [<Test>]
    member _.``GetTree uses relative paths from project root`` () =
        Directory.CreateDirectory(Path.Combine(base.TestDir, "aaa/bbb/ccc")) |> ignore
        File.WriteAllText(Path.Combine(base.TestDir, "aaa/bbb/ccc/deep.txt"), "deep")

        let result = base.FileExplorerTools.GetTree()

        test <@ result.TooManyItems = false @>
        test <@ result.Items |> Seq.contains (TreeItem.Directory "aaa") @>
        test <@ result.Items |> Seq.contains (TreeItem.Directory "aaa/bbb") @>
        test <@ result.Items |> Seq.contains (TreeItem.Directory "aaa/bbb/ccc") @>
        test <@ result.Items |> Seq.contains (TreeItem.File "aaa/bbb/ccc/deep.txt") @>

    [<Test>]
    member _.``GetTree returns paths with spaces correctly`` () =
        Directory.CreateDirectory(Path.Combine(base.TestDir, "music/Le Orme")) |> ignore
        File.WriteAllText(Path.Combine(base.TestDir, "music/Le Orme/Gioco di bimba.mp3"), "not a text file")

        let result = base.FileExplorerTools.GetTree()

        test <@ result.TooManyItems = false @>
        test <@ result.Items.Length = 3 @>
        test <@ result.Items |> Seq.contains (TreeItem.Directory "music") @>
        test <@ result.Items |> Seq.contains (TreeItem.Directory "music/Le Orme") @>
        test <@ result.Items |> Seq.contains (TreeItem.File "music/Le Orme/Gioco di bimba.mp3") @>