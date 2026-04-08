open System.Reflection
open System.Threading
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging

open Helper
open Agents.Wheater
open Spectre.Console
open Agents.Cryptocurrency
open OpenAIClientBuilder

let ct = CancellationToken()

let logger =
    LoggerFactory.Create( // not disposed
        fun builder ->
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug) // Set minimum log level to Debug
            |> ignore
        ).CreateLogger("ConsoleApp")

let config =
    ConfigurationBuilder()
        .AddUserSecrets(Assembly.GetExecutingAssembly()) // assembly is required to give the runtime the correct one where to find the UserSecretsId
        //.AddAzureKeyVault(Uri("https://your-vault.vault.azure.net/"), DefaultAzureCredential())
        .Build()

let weather_model = Models.OpenAI.GPT_5_mini
let cryptocurrencies_model = Models.OpenAI.GPT_5_2

let openAIKey = config.Get "OpenAI api key"
let alibabaApiKey = config.Get "AliBaba api key"
let githubToken = config.Get "GitHub token"
let mistralApiKey = config.Get "Mistral api key"

let wheatherAgent = WeatherAgent.CreateChatClientUsingOpenAI(logger, openAIKey, weather_model)

(*
//let question = AnsiConsole.Ask<string>("[bold green]Ask the agent about the weather:[/]")
//let question = "Dammi le coordinate geografiche di Pesaro"
let question = "Come è il tempo a Pesaro?"
AnsiConsole.MarkupLine($"[cyan]{question}[/]")
let response = wheatherAgent.Ask(question, CancellationToken.None) |> Async.RunSynchronously
AnsiConsole.MarkupLine($"[yellow]{response}[/]")
*)

let chatClient, model =
    match Settings.service with
    | Settings.AIService.OpenAI -> OpenAIClientBuilder.BuildOpenAIChatClient (openAIKey, cryptocurrencies_model)
    | Settings.AIService.LocalOllama -> OpenAIClientBuilder.BuildLocalOllamaChatClient Settings.OllamaModel
    | Settings.AIService.AliBaba -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.AliBaba, alibabaApiKey, Models.Alibaba.Zhipu)
    | Settings.AIService.GitHub -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.GitHub, githubToken, Models.GitHub.Phi_4_mini_instruct)
    | Settings.AIService.Mistral -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.Mistral, mistralApiKey, Models.Mistral.MINISTRAL_14b_2512)

let cryptocurrencyAgent = CryptocurrencyAgent(
        logger,
        chatClient,
        config.Get "Kraken:public key",
        config.Get "Kraken:private key",
        config.Get "Coigecko:api key",
        config.Get "Wise:api key"
        )

AnsiConsole.MarkupLine $"🤖 Cryptocurrencies Agent [blue]{cryptocurrencyAgent.Name}[/] using model 🧠 [red]{model}[/] ({Settings.service})."

//let question = "What is my balance on Kraken, considering all the tokens? Calculate the balances in EUR and give me also the total. Give me a table in the answer."
//let question = "What is the exchange rates of GBP/EUR and USD/EUR?"
let question = "What is the market ticker (bid and ask) of XRP/EUR and SOL/EUR ?"
AnsiConsole.MarkupLine($"[cyan]{question}[/]")

task {
    let! response = cryptocurrencyAgent.Ask(question, ct)

    match response.Usage with
    | null -> ()
    | usage -> renderUsage usage

    //logger.LogInformation response.Text

    do! ConsoleMarkdownRenderer.Displayer.DisplayMarkdownAsync(response.Text)
}
|> Async.AwaitTask
|> Async.RunSynchronously

()