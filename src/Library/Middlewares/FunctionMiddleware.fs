namespace Middlewares

open System
open Microsoft.Extensions.Logging
open Middlewares


type FunctionMiddleware (logger:ILogger) =

    interface IFunctionMiddleware with
        member _.NextAsFunc =
            Func<_, _, _, _, _>(fun agent context next ct ->
                    let result = next.Invoke(context, ct) 
                    result
            )

        member _.Next agent context next ct = 
            logger.LogDebug($"Call function: {context.Function.Name} Iteration:{context.Iteration}")
            let result = next.Invoke(context, ct)
            result
