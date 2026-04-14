namespace Middlewares

open System.Linq
open Microsoft.Extensions.Logging
open Microsoft.Extensions.AI
open Microsoft.Agents.AI

type AgentProhibitedWordsMiddleware(logger:ILogger) =

    let prohibitedWords = ["Mussolini"; "Fragole"]

    interface IAgentMiddleware with
        member this.Run messages session options agent ct =

            let lastMessage = messages.Last().Text
            let foundWords = prohibitedWords |> List.filter (fun word -> lastMessage.Contains word)
            if foundWords.Count() > 0 then
                let responseMessage = ChatMessage(ChatRole.Assistant, $"I can't process requests about prohibited content ({foundWords}).")
                let response = AgentResponse(responseMessage)
                System.Threading.Tasks.Task.FromResult response
    
                // lastmessage can be of the assistant, not the user
                //let lastmessage = context.Messages.Last().Text
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
            else
                agent.RunAsync(messages, session, options, ct)
