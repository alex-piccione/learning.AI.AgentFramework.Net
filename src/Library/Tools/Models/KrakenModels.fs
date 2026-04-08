module KrakenModels

open System
open System.ComponentModel
open Alex75.Cryptocurrencies

type Ticker = {
    [<Description("Highest price offered for buy in the market")>]
    Bid: decimal
    [<Description("Lowest price offered for sell in the market")>]
    Ask: decimal
}

type Ticker with
    static member FromApiTicker (ticker:Alex75.Cryptocurrencies.Ticker): Ticker =
        {
            Bid = ticker.Bid
            Ask = ticker.Ask
        }

type OrderSide =
    | [<Description("The order is for BUY the main token, paying within the quote token")>] Buy
    | [<Description("The order is for SELL the main token, paying within the quote token")>] Sell

type OpenOrder = {
    [<Description("When the order was created")>]
    CreatedOn: DateTime
    [<Description("The main currency")>]
    MainCurrency: string
    [<Description("The quote currency")>]
    QuoteCurrency: string
    [<Description("The quote currency")>]
    Side: OrderSide
}

type OpenOrder with
    static member FromApiOpenOrder (order: Alex75.Cryptocurrencies.OpenOrder): OpenOrder =
        {
            CreatedOn = order.OpenTime
            MainCurrency = order.Pair.Main.UpperCase
            QuoteCurrency = order.Pair.Quote.UpperCase
            Side = 
                match order.Side with 
                | Alex75.Cryptocurrencies.OrderSide.Buy -> Buy
                | Alex75.Cryptocurrencies.OrderSide.Sell -> Sell

        }