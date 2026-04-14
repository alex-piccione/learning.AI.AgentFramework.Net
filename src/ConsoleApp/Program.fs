open System
open System.Reflection
open System.Threading
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Spectre.Console
open Helper
open OpenAIClientBuilder
open Agents.Wheater
open Agents.Cryptocurrency
open Agents.Expenses
open Agents.Musicist

open Middleware.AgentCallTelemetryMiddleware

let ct = CancellationToken()

let loggerFactory = LoggerFactory.Create(
    fun builder ->
        builder
            .AddConsole()
            .SetMinimumLevel(LogLevel.Debug) // Set minimum log level
        |> ignore
    )

let logger = loggerFactory.CreateLogger("ConsoleApp")

let config =
    ConfigurationBuilder()
        .AddUserSecrets(Assembly.GetExecutingAssembly()) // assembly is required to give the runtime the correct one where to find the UserSecretsId
        //.AddAzureKeyVault(Uri("https://your-vault.vault.azure.net/"), DefaultAzureCredential())
        .Build()

let openAIKey = config.Get "OpenAI api key"
let alibabaApiKey = config.Get "AliBaba api key"
let alibabaPlanApiKey = config.Get "AliBaba Plan api key"
let githubToken = config.Get "GitHub token"
let mistralApiKey = config.Get "Mistral api key"
let openrouterApiKey = config.Get "Openrouter api key"
let xiaomiApiKey = config.Get "Xiaomi api key"
let googleApiKey = config.Get "Google api key"
let googleApiKeyForLyria = config.Get "Google api key for Lyria"

let wheatherAgent = WeatherAgent.CreateChatClientUsingOpenAI(logger, openAIKey, LlmModels.OpenAI.GPT_5_mini)

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
    | Settings.AIService.OpenAI -> OpenAIClientBuilder.BuildOpenAIChatClient (openAIKey, LlmModels.OpenAI.GPT_5_2)
    | Settings.AIService.LocalOllama -> OpenAIClientBuilder.BuildLocalOllamaChatClient Settings.OllamaModel
    | Settings.AIService.AliBaba -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.AliBaba, alibabaApiKey, LlmModels.Alibaba.Qwen_3_5_plus)
    | Settings.AIService.AliBabaPlan -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.AliBabaPlan, alibabaPlanApiKey, LlmModels.AlibabaPlan.Zhipu)
    | Settings.AIService.GitHub -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.GitHub, githubToken, LlmModels.GitHub.Phi_4_mini_instruct)
    | Settings.AIService.Mistral -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.Mistral, mistralApiKey, LlmModels.Mistral.MINISTRAL_14b_2512)
    | Settings.AIService.Openrouter -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.Openrouter, openrouterApiKey, LlmModels.Openrouter.Minimax_2_5_Free)
    | Settings.AIService.Xiaomi -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.Xiaomi, xiaomiApiKey, LlmModels.Xiaomi.Mimo_V2_Pro)
    | Settings.AIService.Google -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.Google, googleApiKey, LlmModels.Google.Gemma_4_26B)

// create Middlewares
let agentCallTelemetryMiddleware = AgentCallTelemetryMiddleware(logger, LogType.Detailed)

let functionCallMiddleware = FunctionCallMiddleware.FunctionCallMiddleware(logger)

//let question = "What is my balance on Kraken, considering all the tokens? Calculate the balances in EUR and give me also the total. Give me a table in the answer."
//let question = "What is the exchange rates of GBP/EUR and USD/EUR?"
//let question = "What is the market ticker (bid and ask) of XRP/EUR and SOL/EUR ?"
let question = "List my open orders on Kraken."
AnsiConsole.MarkupLine($"[cyan]{question}[/]")

let tasc = task {
    try
        let! cryptocurrencyAgent = CryptocurrencyAgent.Create(
            logger,
            chatClient,
            config.Get "Kraken:public key",
            config.Get "Kraken:private key",
            config.Get "Coigecko:api key",
            config.Get "Wise:api key",
            [agentCallTelemetryMiddleware],
            [functionCallMiddleware]
            )

        AnsiConsole.MarkupLine $"🤖 :robot: Agent [blue]CryptocurrencyAgent[/] using model 🧠 [cyan]{model}[/] of [cyan]{Settings.service}[/]."

        let! response = cryptocurrencyAgent.Ask(question, ct)

        AnsiConsole.MarkupLine($"Total Agent calls: [yellow]{agentCallTelemetryMiddleware.CallsCount}[/]")
        AnsiConsole.MarkupLine($"Total used tokens: [yellow]{agentCallTelemetryMiddleware.UsedTokens}[/]")
        AnsiConsole.MarkupLine($"Total elapsed time: [yellow]{agentCallTelemetryMiddleware.CallsExecutionTime}[/]")
        

        //match response.Usage with
        //| null -> ()
        //| usage -> renderUsage usage

        try
            do! ConsoleMarkdownRenderer.Displayer.DisplayMarkdownAsync(response.Text.EscapeMarkup())
        with ex ->
            //logger.LogWarning "Failed to use DisplayMarkdownAsync"
            logger.LogInformation response.Text

    with ex ->
       AnsiConsole.MarkupLine $"[red]Failed to call Agent.[/]"
       AnsiConsole.WriteException(ex)
}
tasc.Spinner(Spinner.Known.Star) //|> Async.RunSynchronously
//|> Async.Awa
|> runTask



(* test local MCP
task {
    let! expensesAgent = ExpensesAgent.Create (logger, loggerFactory, chatClient, Settings.expensesMcpServerUrl)

    let! response = expensesAgent.Ask("Yesterday I bought pizza for dinner. paid cash. 5 EUR. Add the record to the expenses.", ct)

    match response.Usage with
    | null -> ()
    | usage -> renderUsage usage

    AnsiConsole.MarkupLine($"[cyan]{response.Text.EscapeMarkup()}[/]")
} |> runTask
*)

(* test Google Lyria 3
let musicistAgent = MusicistAgent(logger, chatClient, googleApiKeyForLyria, "lyria-3-clip-preview")

task {
    let song = """
        Create a prog rock song for Pablo who is riding his Caballero motorbike.
        He has fun while driving on mountain roads and in narrow paths in the wood.
         He has a lot of fun and is happy.
        15 seconds long.
        Save it in D:/AI-songs/caballero.mp3
        """

    AnsiConsole.MarkupLine($"[cyan]{song}[/]")
    let! response = musicistAgent.Ask(song, ct)

    match response.Usage with
    | null -> ()
    | usage -> renderUsage usage

    AnsiConsole.MarkupLine($"[cyan]{response.Text.EscapeMarkup()}[/]")
} |> runTask
*)


loggerFactory.Dispose()

()