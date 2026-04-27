namespace UnitTests.Tools.DirectoryExplorer

open System
open System.IO
open NUnit.Framework
open Swensen.Unquote

open RootFolderTestBase

type GetTree () =
    inherit TestBase()

    [<Test>]
    member _.``GetTree returns empty for empty directory`` () =
        let result = base.DirectoryExplorerTools.GetTree() |> Seq.toList
        
        test <@ result.IsEmpty @>

    [<Test>]
    member _.``GetTree returns files in root`` () =
        File.WriteAllText(Path.Combine(base.TestDir, "file1.txt"), "content1")
        File.WriteAllText(Path.Combine(base.TestDir, "file2.txt"), "content2")

        let result = base.DirectoryExplorerTools.GetTree() |> Seq.toList

        test <@ result.Length = 2 @>
        test <@ result |> Seq.contains "f file1.txt" @>
        test <@ result |> Seq.contains "f file2.txt" @>

    [<Test>]
    member _.``GetTree returns directories in root`` () =
        Directory.CreateDirectory(Path.Combine(base.TestDir, "subdir")) |> ignore

        let result = base.DirectoryExplorerTools.GetTree() |> Seq.toList

        test <@ result.Length = 1 @>
        test <@ result |> Seq.contains "d subdir/" @>

    [<Test>]
    member _.``GetTree returns nested structure`` () =
        Directory.CreateDirectory(Path.Combine(base.TestDir, "src")) |> ignore
        Directory.CreateDirectory(Path.Combine(base.TestDir, "src/Core")) |> ignore
        File.WriteAllText(Path.Combine(base.TestDir, "src/Core/Library.fs"), "code")
        File.WriteAllText(Path.Combine(base.TestDir, "README.md"), "readme")

        let result = base.DirectoryExplorerTools.GetTree() |> Seq.toList

        test <@ result.Length = 4 @>
        test <@ result |> Seq.contains "d src/" @>
        test <@ result |> Seq.contains "d src/Core/" @>
        test <@ result |> Seq.contains "f src/Core/Library.fs" @>
        test <@ result |> Seq.contains "f README.md" @>

    [<Test>]
    member _.``GetTree directories end with slash`` () =
        Directory.CreateDirectory(Path.Combine(base.TestDir, "dir1")) |> ignore
        Directory.CreateDirectory(Path.Combine(base.TestDir, "dir2")) |> ignore

        let result = base.DirectoryExplorerTools.GetTree() |> Seq.toList

        test <@ result |> Seq.forall (fun r -> not (r.StartsWith "d ") || r.EndsWith "/") @>

    [<Test>]
    member _.``GetTree files do not end with slash`` () =
        File.WriteAllText(Path.Combine(base.TestDir, "file1.txt"), "content")
        File.WriteAllText(Path.Combine(base.TestDir, "file2.fs"), "code")

        let result = base.DirectoryExplorerTools.GetTree() |> Seq.toList

        test <@ result |> Seq.forall (fun r -> not (r.StartsWith "f ") || not (r.EndsWith "/")) @>

    [<Test>]
    member _.``GetTree uses relative paths from project root`` () =
        Directory.CreateDirectory(Path.Combine(base.TestDir, "aaa/bbb/ccc")) |> ignore
        File.WriteAllText(Path.Combine(base.TestDir, "aaa/bbb/ccc/deep.txt"), "deep")

        let result = base.DirectoryExplorerTools.GetTree() |> Seq.toList

        test <@ result |> Seq.contains "d aaa/" @>
        test <@ result |> Seq.contains "d aaa/bbb/" @>
        test <@ result |> Seq.contains "d aaa/bbb/ccc/" @>
        test <@ result |> Seq.contains "f aaa/bbb/ccc/deep.txt" @>


