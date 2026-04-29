module Tools.DirectoryExplorer.Models

open System.ComponentModel


[<Description("Result of the GetTree call.")>]
type GetTreeResult = {
    [<Description("If this is true, there wehre too many items and retrieving all them is not possible.")>]
    TooManyItems:bool
    [<Description("If TooManyItems is false, this list will be populated with all files and directories in the root folder (at any level), otherwise will be empty.")>]
    //[<Description("List of all the directories and files in the root folder, at any level. All the paths are relative to the root folder.")>]
    Items: TreeItem array
} 
and TreeItem = {
    [<Description("Defines the item type: 'directory' or 'file'.")>]
    Type: string
    [<Description("The name of the file or directory.")>]
    Name: string
    [<Description("Relative path from root. Use this to determine nesting levels.")>]
    Path: string
}
