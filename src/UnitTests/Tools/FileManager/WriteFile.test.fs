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
        let filePath = Path.Combine(base.TestDir, "new_file.txt")
        let content = "This is new content."

        base.FileManagerTools.WriteTextFile(filePath, content) |> runTask

        test <@ File.Exists filePath @>
        let actualContent = File.ReadAllText filePath
        test <@ actualContent = content @>

    [<Test>]
    member _.``WriteTextFile creates new file in nested directory`` () =
        let filePath = Path.Combine(base.TestDir, "subdir/nested/new_file.txt")
        let content = "Nested content."

        base.FileManagerTools.WriteTextFile(filePath, content) |> runTask

        test <@ File.Exists filePath @>
        let actualContent = File.ReadAllText filePath
        test <@ actualContent = content @>

    [<Test>]
    member this.``WriteTextFile creates new file with spaces in name`` () =
        let filePath = Path.Combine(base.TestDir, "my new file.txt")
        let content = "Content with spaces in filename."

        base.FileManagerTools.WriteTextFile(filePath, content) |> runTask

        test <@ File.Exists filePath @>
        let actualContent = File.ReadAllText filePath
        test <@ actualContent = content @>

    [<Test>]
    member this.``WriteTextFile creates new file in directory with spaces`` () =
        let filePath = Path.Combine(base.TestDir, "sub dir/nested/my file.txt")
        let content = "Nested content with spaces."

        base.FileManagerTools.WriteTextFile(filePath, content) |> runTask

        test <@ File.Exists filePath @>
        let actualContent = File.ReadAllText filePath
        test <@ actualContent = content @>

    [<Test>]
    member _.``WriteTextFile overwrites existing file`` () =
        let filePath = Path.Combine(base.TestDir, "existing.txt")
        let initialContent = "Initial content."
        let newContent = "Overwritten content."

        // Create initial file
        File.WriteAllText(filePath, initialContent)

        // Overwrite with CreateFile
        base.FileManagerTools.WriteTextFile(filePath, newContent) |> runTask

        let actualContent = File.ReadAllText filePath
        test <@ actualContent = newContent @>