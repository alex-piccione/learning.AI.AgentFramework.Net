namespace Tools.Kraken

open Microsoft.Extensions.Logging
open System.ComponentModel
open System.Threading.Tasks
open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies
open Tools
open Tools.Kraken.Models


type KrakenTools (logger:ILogger, krakenPublicKey, kakenSecretKey) =
    inherit ToolsBase(logger)

    let client = Client(krakenPublicKey, kakenSecretKey) :> IClient

    [<Description("Retrieve the balances of the owned currencies (crypto and fiat) in the Kraken exchange")>]
    member this.GetBalance () = task {
        this.LogCall "GetBalance" None
        try 
            let! balance = client.GetBalance()
            return balance

        with ex -> 
            this.LogError "GetBalance" ex
            return failwith $"Failed to call Kraken API. {ex}"
    }

    [<Description("Retrieve the market ticker of the given currency pair in the Kraken exchange")>]
    member this.GetTicker (main:string, quote:string): Task<Ticker> = task {
        this.LogCall "GetTicker" (Some $"{main}/{quote}")
        try 
            let! ticker = client.GetTicker(CurrencyPair(main, quote))
            return Ticker.FromApiTicker ticker

        with ex -> 
            this.LogError "GetTicker" ex
            return failwith $"Failed to call Kraken API. {ex}"
    }

    [<Description("Retrieve the open orders in the Kraken exchange")>]
    member this.GetOpenOrders (): Task<OpenOrder array> = task {
        this.LogCall "GetOpenOrders" None
        try 
            let! orders = client.ListOpenOrders()
            return Array.map (fun o -> OpenOrder.FromApiOpenOrder o) <| orders

        with ex -> 
            this.LogError "GetTicker" ex
            return failwith $"Failed to call Kraken API. {ex}"
    }