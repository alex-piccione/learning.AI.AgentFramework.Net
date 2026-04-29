open System
open System.Threading
open Microsoft.Extensions.Logging
open Spectre.Console
open Helper
open OpenAIClientBuilder
open Middlewares
open Agents
open Secrets

let ct = CancellationToken()

let loggerFactory = LoggerFactory.Create(
    fun builder ->
        builder
            .AddConsole()
            .SetMinimumLevel(LogLevel.Debug) // Set minimum log level
        |> ignore
    )

let logger = loggerFactory.CreateLogger("ConsoleApp")

let secrets:Tools.ThirdPartySecrets = {
    krakenPublicKey = krakenPublicKey
    krakenPrivateKey = krakenPrivateKey
    coingeckoApiKey = coingeckoApiKey
    wiseApiKey = wiseApiKey
    googleApiKey = googleApiKey
    googleLyriaApiKey = googleApiKeyForLyria
}

let toolsProvider = Tools.ToolsProvider(logger, secrets)

// create Middlewares
let agentTelemetryMiddleware = AgentTelemetryMiddleware(logger, LogType.Simple)
let prohibitedWordsMiddleware = AgentProhibitedWordsMiddleware(logger)

let functionMiddleware = FunctionMiddleware(logger)
let chatClientMiddleware = ChatClientCallMiddleware(logger)

let aiAgentHelper =
   AIAgentHelper()
        .AddAgentMiddleware(agentTelemetryMiddleware)
        .AddAgentMiddleware(prohibitedWordsMiddleware)
        .AddFunctionMiddleware(functionMiddleware)
        .AddChatClientMiddleware(chatClientMiddleware)

let agentBuilder = AgentBuilder(aiAgentHelper, toolsProvider)

let chatClient, clientInfo =
    match Settings.service with
    | Settings.AIService.OpenAI -> OpenAIClientBuilder.BuildOpenAIChatClient (openAIKey, LlmModels.OpenAI.GPT_5_2)
    | Settings.AIService.LocalOllama -> OpenAIClientBuilder.BuildLocalOllamaChatClient Settings.OllamaModel
    | Settings.AIService.AliBaba -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.AliBaba, alibabaApiKey, LlmModels.Alibaba.Qwen_3_5_122b_a10b)
    | Settings.AIService.AliBabaPlan -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.AliBabaPlan, alibabaPlanApiKey, LlmModels.AlibabaPlan.Zhipu)
    | Settings.AIService.GitHub -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.GitHub, githubToken, LlmModels.GitHub.Phi_4_mini_instruct)
    | Settings.AIService.Mistral -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.Mistral, mistralApiKey, LlmModels.Mistral.MINISTRAL_14b_2512)
    | Settings.AIService.Openrouter -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.Openrouter, openrouterApiKey, LlmModels.Openrouter.Minimax_2_5_Free)
    | Settings.AIService.Xiaomi -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.Xiaomi, xiaomiApiKey, LlmModels.Xiaomi.Mimo_V2_Pro)
    | Settings.AIService.Google -> OpenAIClientBuilder.BuildOpenAICompatibleChatClient (LLMProvider.Google, googleApiKey, LlmModels.Google.Gemma_4_26B)

let clientWrapper = Clients.ClientWrapper(chatClient, clientInfo)

// Test Agent //

(task {
    try
        (*
        // Weather Agent //
        let agent = agentBuilder.CreateWeatherAgent(clientWrapper)

        //let question = AnsiConsole.Ask<string>("[bold green]Ask the agent about the weather:[/]")
        //let question = "Dammi le coordinate geografiche di Pesaro"
        let question = "Come è il tempo a Pesaro? Answer in Spanish."
        *)

        //(*
        // cryptocurrency //
        //let agent = agentBuilder.CreateCryptocurrencyAgent(clientWrapper)

        //let question = "What is my balance on Kraken, considering all the tokens? Calculate the balances in EUR and give me also the total. Give me a table in the answer."
        //let question = "What is the exchange rates of GBP/EUR and USD/EUR?"
        //let question = "What is the market ticker (bid and ask) of XRP/EUR and SOL/EUR ?"
        //let question = "List my open orders on Kraken."
        //*)

        // test prohibited word
        //let question = "Who is Mussolini ?"  // test prohibited words

        // Agent call //
        //let question = "Quanti EUR sono 60 GBP? Quanti GBP ci vogliono per 200 EUR ?"
        //let torrentDir = "T:\Torrent\Completed\Angine de Poitrine"
        let torrentDir = "T:/Torrent/Completed/Angine de Poitrine"
        //let question = $"What is the root folder you can look at?"
        //let question = $"Can you see the files in {torrentDir} (or its subfolders)? There are .mp3 files?"
        //let question = $"How many files and directories are in {torrentDir} ? Use the GetTree tool and count items of type 'file' and 'directory'?"
        //let question = $"How many directories are in {torrentDir} ?"
        //let question = $"What tools do you have? What tools do your sub-agent have? What them do?"

        let question = $"List the files in {torrentDir}. Highlight the .mp3 ones (if there are). Also search in sub-folders."
        //let question = $"List the sub-folders in {torrentDir}. List the files there, highlight the .mp3 ones (if there are)."

        // create Orchestrator Agent
        //let! filesManageAgent = agentBuilder.CreateFilesManagerAgent(logger, clientWrapper, torrentDir)
        //let agent = filesManageAgent :> IChatAgent
        let! agent = OrchestratorAgent.Create(logger, aiAgentHelper, agentBuilder, clientWrapper, Some torrentDir, ct)

        AnsiConsole.MarkupLine $"🤖 [blue]{agent.Name}[/] using 🧠 [cyan]{agent.LlmModel}[/] on [cyan]{agent.LlmProvider}[/].\n"

        AnsiConsole.WriteLine()
        AnsiConsole.MarkupLine($"[yellow](🙋 User question)[/]\n\n[cyan]{question}[/]")
        AnsiConsole.WriteLine()

        AnsiConsole.WriteLine("------------------------------------------------------------------------")

        let! response = agent.Ask(question, ct)

        AnsiConsole.WriteLine()
        AnsiConsole.WriteLine()
        AnsiConsole.MarkupLine($"[gray]Total Agent calls:[/]  [white]{agentTelemetryMiddleware.CallsCount}[/]")
        AnsiConsole.MarkupLine($"[gray]Total used tokens:[/]  [white]{agentTelemetryMiddleware.UsedTokens}[/]")
        AnsiConsole.MarkupLine($"[gray]Total elapsed time:[/] [white]{agentTelemetryMiddleware.CallsExecutionTime}[/]")
        AnsiConsole.WriteLine("------------------------------------------------------------------------")
        AnsiConsole.WriteLine()

        //match response.Usage with
        //| null -> ()
        //| usage -> renderUsage usage

        AnsiConsole.WriteLine()
        AnsiConsole.MarkupLine($"[yellow](🤖 Agent answer)[/]")
        AnsiConsole.WriteLine()

        try
            do! ConsoleMarkdownRenderer.Displayer.DisplayMarkdownAsync(response.Text.EscapeMarkup())
        with ex ->
            logger.LogWarning "Failed to use DisplayMarkdownAsync"
            logger.LogInformation response.Text

    with ex ->
       AnsiConsole.MarkupLine $"[red]Failed to call Agent.[/]"
       AnsiConsole.WriteException(ex)

}).Spinner(Spinner.Known.Aesthetic)
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