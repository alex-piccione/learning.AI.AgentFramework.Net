module Tools.Kraken.Models

open System
open System.ComponentModel

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

[<Description("Order in status \"open\" in the Kraken market")>]
type OpenOrder = {
    [<Description("When the order was created")>]
    CreatedOn: DateTime

    [<Description("Main currency")>]
    MainCurrency: string
    
    [<Description("Quote currency")>]
    QuoteCurrency: string

    [<Description("Quantity of main currency")>]    
    Quantity: decimal

    [<Description("Order Type, e.g. Market, Limit, etc...")>]
    OrderType: string
    
    [<Description("Limit price. Value is zero if is a order type that does not support a limit price.")>]
    LimitPrice: decimal

    [<Description("Side of the order (Buy or Sell)")>]
    Side: string
}

type OpenOrder with
    static member FromApiOpenOrder (order: Alex75.Cryptocurrencies.OpenOrder): OpenOrder =
        {
            CreatedOn = order.OpenTime
            MainCurrency = order.Pair.Main.UpperCase
            QuoteCurrency = order.Pair.Quote.UpperCase
            Quantity = order.BuyOrSellQuantity
            OrderType = order.Type.ToString()
            LimitPrice = order.LimitPrice
            Side =
                match order.Side with
                | Alex75.Cryptocurrencies.OrderSide.Buy -> "Buy"
                | Alex75.Cryptocurrencies.OrderSide.Sell -> "Sell"
        }