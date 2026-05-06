namespace ChatClientFactory

open System
open OpenAI
open Microsoft.Extensions.AI
open Clients
open OllamaSharp

type LLMProvider =
    | AliBaba
    | AliBabaPlan
    | DeepSeek
    | GitHub
    | Mistral
    | Openrouter
    | Xiaomi
    | Google

type ChatClientFactory () =

    static member BuildOpenAIChatClient(apiKey:string, model:string):IChatClient * ClientInfo =
        //OpenAI.Chat.ChatClient(model, apiKey).AsIChatClient(), model
        OpenAIClient(apiKey).GetChatClient(model).AsIChatClient(), ClientInfo(model, "OpenAI")

    static member BuildLocalOllamaChatClient (url:string, model:string):IChatClient * ClientInfo =

        let config = OllamaApiClient.Configuration()
        config.Uri <- Uri(url)
        config.Model <- model
        new OllamaApiClient(config) :> IChatClient, ClientInfo(model, "Local Ollama")

    static member BuildLocalOllamaChatClient_old(model:string):IChatClient * ClientInfo =

        let credentials = ClientModel.ApiKeyCredential "not required"
        let options = OpenAI.OpenAIClientOptions()
        options.Endpoint <- Uri "http://localhost:11434/v1"

        OpenAIClient(credentials, options).GetChatClient(model).AsIChatClient(), ClientInfo(model, "Local Ollama")

    static member BuildOpenAICompatibleChatClient(provider:LLMProvider, apiKey:string, model:string):IChatClient * ClientInfo =

        let url = 
            match provider with
            | LLMProvider.AliBaba -> Constants.LLMProviders.ALIBABA_URL
            | LLMProvider.AliBabaPlan -> Constants.LLMProviders.ALIBABA_PLAN_URL
            | LLMProvider.DeepSeek -> Constants.LLMProviders.DEEPSEEK_URL
            | LLMProvider.GitHub -> Constants.LLMProviders.GITHUB_URL
            | LLMProvider.Mistral -> Constants.LLMProviders.MISTRAL_URL
            | LLMProvider.Openrouter -> Constants.LLMProviders.OPENROUTER_URL
            | LLMProvider.Xiaomi -> Constants.LLMProviders.XIAOMI_URL
            | LLMProvider.Google -> Constants.LLMProviders.GOOGLE_URL

        let credentials = ClientModel.ApiKeyCredential apiKey
        let options = OpenAI.OpenAIClientOptions()
        options.Endpoint <- Uri url

        //OpenAI.Chat.ChatClient(model, credentials, options).AsIChatClient(), model
        OpenAIClient(credentials, options).GetChatClient(model).AsIChatClient(), ClientInfo(model, provider.ToString())
