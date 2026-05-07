namespace UnitTests.Tools.DirectoryExplorer_v2

open System
open System.IO
open NUnit.Framework
open Swensen.Unquote

open RootFolderTestBase

type GetTreeOfDirectories () =
    inherit TestBase()

    let mutable helper = Helper("")

    [<SetUp>]
    member _.SetupAdditional() =
        helper <- Helper(base.TestDir)

    [<Test>]
    member _.``returns empty for empty directory`` () =
        let result = base.FileExplorerTools.GetDirectoriesTree base.TestDir

        test <@ result.Length = 0 @>

    [<TestCase("subdir")>]
    [<TestCase("sub dir")>]
    [<TestCase("sub DIR")>]
    member _.``returns directories in root`` (subdir) =
        let dirPath = helper.CreateDir (Path.Combine(base.TestDir, subdir))
        
        let _ = helper.CreateFile (Path.Combine(dirPath, "README.md"))

        let result = base.FileExplorerTools.GetDirectoriesTree base.TestDir

        test <@ result = [dirPath] @>

    [<Test>]
    member _.``returns nested structure`` () =
        let dir_aaa = helper.CreateDir "aaa"
        let dir_bbb = helper.CreateDir "aaa/bbb"

        let _ = helper.CreateFile "aaa/bbb/File.ext"
        let _ = helper.CreateFile "README.md"

        let result = base.FileExplorerTools.GetDirectoriesTree base.TestDir

        test <@ result = [dir_aaa; dir_bbb] @>

    [<TestCase("sub-dir")>]
    [<TestCase("sub dir")>]
    [<TestCase("sub DIR")>]
    member _.``is callable on sub-directories`` (subdir) =
        let dirPath = helper.CreateDir (Path.Combine(base.TestDir, subdir))

        let _ = helper.CreateFile (Path.Combine(dirPath, "README.md"))
        let _ = helper.CreateFile (Path.Combine(base.TestDir, "README.md"))

        let result = base.FileExplorerTools.GetDirectoriesTree dirPath

        test <@ result = [dirPath] @>