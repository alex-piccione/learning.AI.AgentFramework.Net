namespace Tools.Coingecko

open System.ComponentModel
open System.Threading.Tasks
open Microsoft.Extensions.Logging
open Alex75.Cryptocurrencies.Services.Coingecko
open Alex75.Cryptocurrencies

open Tools
open Tools.Coingecko.Models

type CoingeckoTools (logger: ILogger, apiKey: string) =
    inherit ToolsBase(logger)

    let client = CoingeckoApiClientDemo(apiKey)

    [<Description("Retrieve the price (exchange rate) of the currency pair main/quote. It uses Coingecko Demo API. Main currency cannot be a fiat currency.")>]
    member this.GetRate(main:string, quote:string) : Task<CurrencyPairRate> = task {
        this.LogCall "GetRate" (Some $"{main}/{quote})") //($"{this.GetType().Name} | Call to  | Start  ({main}/{quote})")

        let! rate =  client.GetSinglePairRateAsync(CurrencyPair(main, quote))
        return { Main=main; Quote=quote; Rate=rate }
    }

    [<Description("Retrieve the price (exchange rate) of the currency pairs provided. It uses Coingecko Demo API.")>]
    member this.GetRates(mains:string seq, quotes:string seq) : Task<CurrencyPairRate list>  = task {
        this.LogCall "GetRates" (Some $"""{(String.concat ", " mains)}""")

        let! apiResult = (client.GetPairsRateAsync(
            mains   |> Seq.map (fun m -> Currency(m)),
            quotes  |> Seq.map (fun q -> Currency(q))
        ))

        return CurrencyPairRate.fromApiResponse apiResult
    }