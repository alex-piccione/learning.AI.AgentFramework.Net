module resource_helper

open System.Reflection
open System.IO

let assembly = Assembly.GetExecutingAssembly()

let readResource resourceName =
    let resourceFullName = $"{assembly.GetName().Name}.data.{resourceName}"
    let names = assembly.GetManifestResourceNames()
    if not(Array.contains resourceFullName names) then failwith $@"Cannot find ""{resourceName}"" in the embedded resources"

    match assembly.GetManifestResourceStream(resourceFullName) with
    | null -> failwith $@"Cannot load ""{resourceName}"" from the embedded resources"
    | stream ->
        use reader = new StreamReader(stream)
        reader.ReadToEnd()