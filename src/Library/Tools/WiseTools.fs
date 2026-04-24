namespace Tools

open Microsoft.Extensions.Logging
open System.Threading.Tasks
open System.ComponentModel
open Alex75.Cryptocurrencies
open Alex75.Cryptocurrencies.Services.Wise
open Tools

type WiseTools (logger:ILogger, apiKey) =
    inherit ToolsBase(logger)

    let client = WiseApiClient(apiKey)

    [<Description("Retrieve the exchange rate. Returns how many units of 'quote' are needed for 1 unit of 'base'.")>]
    member this.GetExchangeRate (
        [<Description("The Base currency (the '1' in the pair, e.g., GBP in GBP/EUR)")>]
        baseCurrency:string, 
        [<Description("The Quote currency (the target value, e.g., EUR in GBP/EUR)")>]
        quoteCurrency:string): Task<decimal> = task {

        let pair = CurrencyPair(baseCurrency, quoteCurrency)
        this.LogCall "GetExchangeRate" (Some $"{baseCurrency}/{quoteCurrency}")
        try 
            let! rate = client.GetExchangeRate(pair)
            logger.LogDebug($"{baseCurrency}/{quoteCurrency} = {rate}")
            return rate
        with ex -> 
            this.LogError "GetExchangeRate" ex
            return failwith $"Failed to call Wise API. {ex}"
    }