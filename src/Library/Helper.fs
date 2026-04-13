module Helper

open System

/// AwaitTask + RunSynchronously
let runTask task = task |> Async.AwaitTask |> Async.RunSynchronously

let getValueOrDefault (nullable: Nullable<'T>) (defaultValue: 'T) =
    if nullable.HasValue then nullable.Value
    else defaultValue