namespace UnitTests.Tools.DirectoryExplorer

open System
open System.IO
open NUnit.Framework
open Swensen.Unquote

open RootFolderTestBase

type ReadFile () =
    inherit PathValidationTestBase()

    let mutable helper = Helper("")

    override this.GetOperation () =
        let tools = base.FileManagerTools
        fun path -> tools.ReadFile (path) |> ignore

    [<SetUp>]
    member _.SetupAdditional() =
        helper <- Helper(base.TestDir)

    // ========== Specific ReadFile tests ==========

    [<Test>]
    member _.``ReadFile in root`` () =
        let expectedContent = "This is the content of the file."
        let file = helper.CreateFile("aaa.txt", expectedContent)

        // Execute
        let content = base.FileManagerTools.ReadFile(file)

        test <@ content = expectedContent @>

    [<Test>]
    member _.``ReadFile in subdirectory`` () =
        let expectedContent = "This is the content of the file."
        let file = helper.CreateFile("aaa/bbb.txt", expectedContent)

        // Execute
        let content = base.FileManagerTools.ReadFile(file)

        test <@ content = expectedContent @>

    [<Test>]
    member _.``ReadFile when file does not exist`` () =
        let fullPath = Path.Combine(base.TestDir, "not_exist.txt")
        raisesWith<FileNotFoundException>
            <@ base.FileManagerTools.ReadFile(fullPath) @>
            (fun ex -> <@ ex.Message = $"File '{fullPath}' does not exist." @>)
