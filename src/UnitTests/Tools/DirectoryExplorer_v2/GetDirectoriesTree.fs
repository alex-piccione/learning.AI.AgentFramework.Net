namespace UnitTests.Tools.DirectoryExplorer_v2

open System
open System.IO
open NUnit.Framework
open Swensen.Unquote

open RootFolderTestBase_v2
open Utils

type GetTreeOfDirectories () =
    inherit TestBase()

    [<Test>]
    member _.``returns empty for empty directory`` () =
        let result = base.DirectoryExplorerTools.GetDirectoriesTree base.TestDir

        test <@ result.Length = 0 @>

    [<TestCase("subdir")>]
    [<TestCase("sub dir")>]
    [<TestCase("sub DIR")>]
    member _.``returns directories in root`` (subdir) =
        let dirPath = Path.Combine(base.TestDir, subdir)
        Directory.CreateDirectory dirPath |> ignore
        File.WriteAllText(Path.Combine(dirPath, "README.md"), "readme")

        let result = base.DirectoryExplorerTools.GetDirectoriesTree base.TestDir

        test <@ result.Length = 1 @>
        test <@ result[0] = asWin dirPath @>

    [<Test>]
    member _.``returns nested structure`` () =
        let dir_aaa = Path.Combine(base.TestDir, "aaa")
        let dir_bbb = Path.Combine(base.TestDir, "aaa/bbb")
        Directory.CreateDirectory(dir_bbb) |> ignore
        File.WriteAllText(Path.Combine(base.TestDir, "aaa/bbb/File.ext"), "text")
        File.WriteAllText(Path.Combine(base.TestDir, "README.md"), "readme")

        let result = base.DirectoryExplorerTools.GetDirectoriesTree base.TestDir

        test <@ result.Length = 2 @>
        test <@ result |> Seq.contains (asWin dir_aaa) @>
        test <@ result |> Seq.contains (asWin dir_bbb) @>

    [<TestCase("sub-dir")>]
    [<TestCase("sub dir")>]
    [<TestCase("sub DIR")>]
    member _.``is callable on sub-directories`` (subdir) =
        let dirPath = Path.Combine(base.TestDir, subdir)
        Directory.CreateDirectory dirPath |> ignore
        File.WriteAllText(Path.Combine(dirPath, "README.md"), "readme")
        File.WriteAllText(Path.Combine(base.TestDir, "README.md"), "readme")  // noise

        let result = base.DirectoryExplorerTools.GetDirectoriesTree dirPath

        test <@ result.Length = 1 @>
        test <@ result[0] = asWin dirPath @>