namespace Middlewares

open System.Linq
open Microsoft.Extensions.Logging
open Microsoft.Extensions.AI
open Microsoft.Agents.AI
open Helper

type AgentProhibitedWordsMiddleware(logger:ILogger) =

    let prohibitedWords = ["Mussolini"; "Fragole"]

    interface IAgentMiddleware with
        member this.Run messages session options agent ct =

            let lastMessage = messages.Last().Text
            let foundWords = prohibitedWords |> List.filter (fun word -> lastMessage.Contains word)
            if foundWords.Count() > 0 then
                let responseMessage = ChatMessage(ChatRole.Assistant, $"I can't process requests about prohibited content ({foundWords}).")
                let response = AgentResponse(responseMessage)
                //response.FinishReason <- ChatFinishReason.Stop

                // TODO: this raises an excepion... why?
                System.Threading.Tasks.Task.FromResult response

            else
                agent.RunAsync(messages, session, options, ct)