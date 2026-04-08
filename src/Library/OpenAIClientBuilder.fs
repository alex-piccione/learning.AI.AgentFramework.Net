module OpenAIClientBuilder

open System
open OpenAI
open Microsoft.Extensions.AI

type LLMProvider =
    | AliBaba
    | GitHub
    | Mistral

/// Helper for create OpenAI Client
type OpenAIClientBuilder () =

    /// Create a OpenAI Chat Client
    static member BuildOpenAIChatClient(apiKey:string, model:string):IChatClient * string =
        //OpenAI.Chat.ChatClient(model, apiKey).AsIChatClient(), model
        OpenAIClient(apiKey).GetChatClient(model).AsIChatClient(), model

    static member BuildLocalOllamaChatClient(model:string):IChatClient * string =
        let credentials = ClientModel.ApiKeyCredential "not required"
        let options = OpenAI.OpenAIClientOptions()
        options.Endpoint <- Uri "http://localhost:11434/v1"
        //OpenAI.Chat.ChatClient(model, credentials, options).AsIChatClient(), model
        OpenAIClient(credentials, options).GetChatClient(model).AsIChatClient(), model

    static member BuildOpenAICompatibleChatClient(provider:LLMProvider, apiKey:string, model:string):IChatClient * string =

        let url = 
            match provider with
            | LLMProvider.AliBaba -> Constants.LLMProviders.ALIBABA_AI_URL
            | LLMProvider.GitHub -> Constants.LLMProviders.GITHUB_AI_URL
            | LLMProvider.Mistral -> Constants.LLMProviders.MISTRAL_AI_URL

        let credentials = ClientModel.ApiKeyCredential apiKey
        let options = OpenAI.OpenAIClientOptions()
        options.Endpoint <- Uri url
        //OpenAI.Chat.ChatClient(model, credentials, options).AsIChatClient(), model
        OpenAIClient(credentials, options).GetChatClient(model).AsIChatClient(), model

    (*
    using Microsoft.Extensions.AI;

    IChatClient client =
        new OpenAI.Chat.ChatClient("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
        .AsIChatClient();

    Console.WriteLine(await client.GetResponseAsync(
    [
        new ChatMessage(ChatRole.System, "You are a helpful AI assistant"),
        new ChatMessage(ChatRole.User, "What is AI?"),
    ]));

    *)