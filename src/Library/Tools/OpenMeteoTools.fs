namespace Tools.OpenMeteo

open System.Threading.Tasks
open System.ComponentModel
open System.Net.Http
open Microsoft.Extensions.Logging

open Tools.OpenMeteo.Models
open Tools

[<Description("Coordinates of a city")>]
type CityLocation = {
    [<Description("The name of the city")>]
    City: string

    [<Description("The latitude coordinate")>]
    Latitude: float

    [<Description("The longitude coordinate")>]
    Longitude: float
}

type OpenMeteoTools (logger:ILogger) =
    inherit ToolsBase(logger)

    let client = new HttpClient()
    static let apiUrlBase = "https://api.open-meteo.com/v1"
    static let geolocationApiBaseUrl = "https://geocoding-api.open-meteo.com/v1"

    member private __.ProcessResponse<'T> (response:HttpResponseMessage) : Task<Result<'T, string>> = task {
        try
            let! content = response.Content.ReadAsStringAsync()
            if response.IsSuccessStatusCode then
                return Ok(deserialize<'T> content)
            else
                return Error($"{response.StatusCode} - {response.ReasonPhrase}. Content: {content}")
        with exn -> return Error($"Error processing response: {exn.Message}")
    }

    [<Description("Get the city geolocation: latitude and longitude")>]
    member this.GetCityGeolocation([<Description("The city name")>]city: string)
        : Task<CityLocation> = task {
        this.LogCall "GetCityGeolocation" (Some city)
        
        let url = $"{geolocationApiBaseUrl}/search?name={city}&count=1&language=en"
        try
            let! response = client.GetAsync(url)
            match! this.ProcessResponse<Geolocation.SearchResult>(response) with
            | Error err -> return failwith $"API error. {err}"
            | Ok searchResult ->
                if searchResult.results.Length > 0 then
                    let result = searchResult.results[0]
                    return {
                        City = result.name
                        Latitude = result.latitude
                        Longitude = result.longitude
                    }
                else
                    return failwith $"No geolocation found for city: {city}"

        with ex ->
            this.LogError "GetCityGeolocation" ex
            return failwith $"Error calling geolocation API: {ex.Message}"
        }

    [<Description("Get current weather forecast")>]
    member this.GetCurrentWeatherValues([<Description("The latitude")>]latitude: float, [<Description("The longitude")>]longitude: float)
        : Task<string> = task {
            this.LogCall "GetCurrentWeatherValues" (Some $"Lat:{latitude} Long:{longitude}")
            let lat = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)
            let lon = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)
            let url = $"{apiUrlBase}/forecast?latitude={lat}&longitude={lon}&current=temperature_2m,apparent_temperature,relative_humidity_2m,rain,weather_code"
            try
                let! response = client.GetAsync(url)
                match! this.ProcessResponse<Forecast.Response>(response) with
                | Error err -> return failwith $"API error. {err}"
                | Ok result ->
                    return $"{result}"
                    //return {
                    //    City = result.hourly
                    //}

            with ex ->
                this.LogError "GetCurrentWeatherValues" ex
                return failwith $"Error calling current weather forecast: {ex.Message}"
        }