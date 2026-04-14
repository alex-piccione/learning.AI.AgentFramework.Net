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

        (*
        if (prohibitedWords.Contains (lastMessage.ToLower())) then
            logger.LogCritical("Terminate due to use of prohibited word")
            context.Terminate <- true
            let responseMessage = ChatMessage(ChatRole.Assistant, $"I can't process requests about prohibited content.")
            context.Messages.Add responseMessage
            //return 
            //context.result = AgentResponse()
            task { return obj() } |> ValueTask<obj>
            *)