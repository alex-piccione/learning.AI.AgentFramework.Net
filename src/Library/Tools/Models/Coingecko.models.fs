module Tools.Coingecko.Models

open System.ComponentModel
open Alex75.Cryptocurrencies

type CurrencyPairRate = {
    [<Description("The main currency being priced, e.g., XRD")>]
    Main: string
    
    [<Description("The quote currency used to determine the price, e.g., EUR")>]
    Quote: string
    
    [<Description("The current market exchange rate")>]
    Rate: decimal
}

module CurrencyPairRate =
    let fromApiResponse (response:Rates.CurrenciesRates) : CurrencyPairRate list = 
        let mutable result = []
        
        for (KeyValue(main, rates)) in response do
            let main = main.UpperCase
            for (KeyValue(quote, rate)) in rates do
                let quote = quote.UpperCase
                let record:CurrencyPairRate = { Main=main; Quote=quote; Rate=rate}
                result <- record :: result  // Prepend the record to the list
        result