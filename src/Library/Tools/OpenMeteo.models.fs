module Tools.OpenMeteo.Models

open System.Text.Json
open System.ComponentModel


let jsonOptions = JsonSerializerOptions()
jsonOptions.PropertyNameCaseInsensitive <- true

let deserialize<'T> (json: string) : 'T =
    try 
        match JsonSerializer.Deserialize<'T>(json, jsonOptions) with
        | null -> failwithf "Failed to deserialize response. Result was null. JSON: %s ." json
        | item -> item
    with ex -> failwithf "Failed to deserialize response. Error: %s. JSON: %s" json ex.Message


module Geolocation =

    type Result = {
        name: string
        latitude: float
        longitude: float
        timezone: string
    }

    type SearchResult = { 
        results: Result list
    }

    type WeatherData = {
        Temperature: float
        Humidity: float
        WindSpeed: float
        Precipitation: float
        Time: System.DateTime
    }


module Forecast = 

    //type CurrentData = "" | ""
    //type HourlyData = "temperature_2m" | "rain"

    type Request = {
        latitude: float
        longitude: float
        current: string array option
        hourly: string list option
    }

    (*
    "hourly": {
        "time": ["2022-07-01T00:00", "2022-07-01T01:00", "2022-07-01T02:00", ...],
        "temperature_2m": [13, 12.7, 12.7, 12.5, 12.5, 12.8, 13, 12.9, 13.3, ...]
    },
    "hourly_units": {
        "temperature_2m": "°C"
    }
    *)
    type HourlyData = {
        time: System.DateTime array
        [<Description("Air temperature at 2 meters above groun. °C (Celsius).")>]
        temperature_2m: float array
        [<Description("Relative humidity at 2 meters above ground. °C (Celsius).")>]
        relative_humidity_2m: float
        [<Description("Apparent temperature is the perceived feels-like temperature combining wind chill factor, relative humidity and solar radiation")>]
        apparent_temperature: float
        [<Description("Total precipitation (rain, showers, snow) sum of the preceding hour. mm (millimeters).")>]
        precipitation: float
        [<Description("Rain from large scale weather systems of the preceding hour in millimeter.")>]
        rain: float
        [<Description("Probability of precipitation with more than 0.1 mm of the preceding hour. Probability is based on ensemble weather models with 0.25° (~27 km) resolution.")>]
        precipitation_probability: float
        [<Description("Wind speed (Km/h) at 10 meters above ground.")>]
        wind_speed_10m:float
        [<Description("Total cloud cover as an area fraction.")>]
        cloud_cover:float
    }

    type CurrentData = {
        time: System.DateTime (* array *)
        [<Description("Air temperature at 2 meters above ground. °C (Celsius).")>]
        temperature_2m: float (* array *)
        [<Description("Relative humidity at 2 meters above ground. °C (Celsius).")>]
        relative_humidity_2m: float
        [<Description("Apparent temperature is the perceived feels-like temperature combining wind chill factor, relative humidity and solar radiation")>]
        apparent_temperature: float
        [<Description("Total precipitation (rain, showers, snow) sum of the preceding hour. mm (millimeters).")>]
        precipitation: float
        [<Description("Rain from large scale weather systems of the preceding hour in millimeter.")>]
        rain: float
        [<Description("Probability of precipitation with more than 0.1 mm of the preceding hour. Probability is based on ensemble weather models with 0.25° (~27 km) resolution.")>]
        precipitation_probability: float
        [<Description("Wind speed (Km/h) at 10 meters above ground.")>]
        wind_speed_10m:float
        [<Description("Total cloud cover as an area fraction.")>]
        cloud_cover:float
    }

    type Response = {
        [<Description("A list of weather variables to get current conditions.")>]
        current: CurrentData
        [<Description("Hourly values")>]
        hourly: HourlyData
    }