namespace Clients

open Microsoft.Extensions.AI

type ClientInfo (model:string, provider:string) =
    member _.LlmModel = model
    member _.LlmProvider = provider


type ClientWrapper(chatClient:IChatClient, info:ClientInfo) =
    member _.ChatClient = chatClient
    member _.Info = info
