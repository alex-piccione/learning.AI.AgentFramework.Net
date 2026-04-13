module Helper

/// AwaitTask + RunSynchronously
let RunTask task = task |> Async.AwaitTask |> Async.RunSynchronously

