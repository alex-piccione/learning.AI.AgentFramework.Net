module Tools.Wise

open Microsoft.Extensions.Logging
open System.Threading.Tasks
open System.ComponentModel
open Alex75.Cryptocurrencies
open Alex75.Cryptocurrencies.Services.Wise
open ToolsBase

type WiseTools (logger:ILogger, apiKey) =
    inherit ToolsBase(logger)

    let client = WiseApiClient(apiKey)

    [<Description("Retrieve the exchange rate (fiat over fiat) from Wise API")>]
    member this.GetExchangeRate (
        [<Description("The Main currency in the main/quote pair.")>]
        main:string, 
        [<Description("The Quote currency in the main/quote pair.")>]
        quote:string): Task<decimal> = task {

        let pair = CurrencyPair(main, quote)
        this.LogCall "GetExchangeRate" (Some $"{main}/{quote}")
        try 
            let! rate = client.GetExchangeRate(pair)
            logger.LogDebug($"{main}/{quote} = {rate}")
            return rate
        with ex -> 
            this.LogError "GetExchangeRate" ex
            return failwith $"Failed to call Wise API. {ex}"
    }