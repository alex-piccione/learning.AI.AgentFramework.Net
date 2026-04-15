namespace Tools 


open Microsoft.Extensions.Logging
open Tools

type ThirdPartySecrets = {
    krakenPublicKey:string
    krakenPrivateKey:string
    coingeckoApiKey:string
    wiseApiKey:string
    googleApiKey:string
    googleLyriaApiKey:string
}

type ToolsProvider(logger:ILogger, secrets:ThirdPartySecrets) =

    let openMeteoTools = OpenMeteoTools(logger).GetTools()

    let krakenTools = KrakenTools(logger, secrets.krakenPublicKey, secrets.krakenPrivateKey).GetTools()
    let coingeckoTools = CoingeckoTools(logger, secrets.coingeckoApiKey).GetTools()
    let wiseTools = WiseTools(logger, secrets.wiseApiKey).GetTools()

    let googleLyriaTools = GoogleLyriaTools(logger, secrets.googleLyriaApiKey).GetTools()

    member _.OpenMeteoTools = openMeteoTools
    member _.KrakenTools = krakenTools
    member _.CoingeckoTools = coingeckoTools
    member _.WiseTools = wiseTools
    member _.GoogleLyriaTools = googleLyriaTools
