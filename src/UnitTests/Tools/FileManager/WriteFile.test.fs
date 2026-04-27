namespace UnitTests.Tools.FileExplorer

open System
open System.IO
open System.Threading.Tasks
open NUnit.Framework
open Swensen.Unquote

open RootFolderTestBase

type WriteTextFile () =
    inherit PathValidationTestBase()

    //let runTask (t:Task) = t |> Async.AwaitTask |> Async.RunSynchronously  // --> raises a AggregateException
    let runTask (t:Task) = t.GetAwaiter().GetResult()                        // --> raises the original exception

    override this.GetOperation () =
        let tools = base.FileManagerTools
        fun path -> tools.WriteTextFile(path, "content") |> runTask

    // ========== Specific CreateFile tests ==========

    // TODO: dues to FS0405
    // (a protected member is called or 'base' is being used. This is only allowed in the direct implementation of members
    // since they could escape their object scope.)
    // cannot use "task {...}"

    [<Test>]
    member this.``WriteTextFile creates new file in root`` () =
        let filePath = "new_file.txt"
        let content = "This is new content."

        base.FileManagerTools.WriteTextFile(filePath, content) |> runTask

        let fullPath = Path.Combine(base.TestDir, filePath)
        test <@ File.Exists fullPath @>
        let actualContent = File.ReadAllText fullPath
        test <@ actualContent = content @>

    [<Test>]
    member _.``WriteTextFile creates new file in nested directory`` () =
        let filePath = "subdir/nested/new_file.txt"
        let content = "Nested content."

        base.FileManagerTools.WriteTextFile(filePath, content) |> runTask

        let fullPath = Path.Combine(base.TestDir, filePath)
        test <@ File.Exists fullPath @>
        let actualContent = File.ReadAllText fullPath
        test <@ actualContent = content @>

    [<Test>]
    member _.``WriteTextFile overwrites existing file`` () =
        let filePath = "existing.txt"
        let initialContent = "Initial content."
        let newContent = "Overwritten content."

        // Create initial file
        let fullPath = Path.Combine(base.TestDir, filePath)
        File.WriteAllText(fullPath, initialContent)

        // Overwrite with CreateFile
        base.FileManagerTools.WriteTextFile(filePath, newContent) |> runTask

        let actualContent = File.ReadAllText fullPath
        test <@ actualContent = newContent @>