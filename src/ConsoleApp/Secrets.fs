module Secrets

open Microsoft.Extensions.Configuration
open System.Reflection
open Helper

let config =
    ConfigurationBuilder()
        .AddUserSecrets(Assembly.GetExecutingAssembly()) // assembly is required to give the runtime the correct one where to find the UserSecretsId
        //.AddAzureKeyVault(Uri("https://your-vault.vault.azure.net/"), DefaultAzureCredential())
        .Build()

let openAIKey = config.Get "OpenAI api key"
let alibabaApiKey = config.Get "AliBaba api key"
let alibabaPlanApiKey = config.Get "AliBaba Plan api key"
let deepseekApiKey = config.Get "DeepSeek api key"
let githubToken = config.Get "GitHub token"
let mistralApiKey = config.Get "Mistral api key"
let openrouterApiKey = config.Get "Openrouter api key"
let xiaomiApiKey = config.Get "Xiaomi api key"
let googleApiKey = config.Get "Google api key"
let googleApiKeyForLyria = config.Get "Google api key for Lyria"

let krakenPublicKey = config.Get "Kraken:public key"
let krakenPrivateKey = config.Get "Kraken:private key"
let coingeckoApiKey = config.Get "Coigecko:api key"
let wiseApiKey = config.Get "Wise:api key"

