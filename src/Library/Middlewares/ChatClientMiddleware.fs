namespace Middlewares

open System.Threading
open Microsoft.Extensions.AI
open Microsoft.Extensions.Logging
open Middlewares

type ChatClientCallMiddleware(logger:ILogger) =
    interface IChatClientMiddleware with
        member this.GetResponse messages options chatClient ct = task {
            let! response = chatClient.GetResponseAsync (messages, options, ct)
            logger.LogDebug($"FinishReason: {response.FinishReason}")
            return response
        }