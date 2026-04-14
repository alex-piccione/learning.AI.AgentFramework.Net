module FunctionCallMiddleware

open System
open Microsoft.Extensions.Logging
open Middlewares


type FunctionCallMiddleware (logger:ILogger) =

    interface IFunctionCallMiddleware with
        member _.NextAsFunc =
            Func<_, _, _, _, _>(fun agent context next ct ->
                    let result = next.Invoke(context, ct) 
                    result
            )

        member _.Next agent context next ct = 
            logger.LogInformation("Callback")
            let result = next.Invoke(context, ct)
            result
