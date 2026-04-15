namespace Tools

open System
open System.IO
open Microsoft.Extensions.Logging
open Google.GenAI
open Google.GenAI.Types

open Tools
open System.ComponentModel

type GoogleLyriaTools (logger, googleLyriaApiKey:string) =
    inherit ToolsBase(logger)

    [<Description("Create a song with the provided description and given length.")>]
    member this.GenerateMusicClip (
        [<Description("Style, mood, some indication about the story or argument.")>]
        instructions:string, 
        [<Description("Lenght of the song, in seconds")>]
        lenghtInSeconds:int, 
        [<Description("The LLM model to use.")>]
        llmModel:string,
        [<Description("Where to save the generated MP3. The file path.")>]
        outputFile:string, ct) = task {

        this.LogCall "GenerateMusicClip" None

        try 
            match Path.GetDirectoryName outputFile with
            | null -> failwith $"Directory of {outputFile} is empty"
            | directory -> 
                if Directory.Exists outputFile = false then
                    Directory.CreateDirectory directory |> ignore

            let client = new Client(apiKey=googleLyriaApiKey)

            let config = GenerateContentConfig()
            //config. ResponseMimeType = "audio/wav"  // default is mp3 ?

            let! response = client.Models.GenerateContentAsync(
              model = llmModel,
              contents =  $"{instructions} It has to be {lenghtInSeconds} seconds lenght maximum."
            )

            (*
            let candidate =
                match response.Candidates with
                | null -> failwith "No Candidates"
                | c when c.Count = 0 -> failwith "Empty Candidates"
                | c -> c[0]

            let content =
                match candidate.Content with
                | null -> failwith "No Content"
                | c -> c

                *)

            logger.LogDebug $"Code execution: {response.CodeExecutionResult}"
            logger.LogDebug $"Response: {response.Text}"

            match response.PromptFeedback with
            | null -> ()
            | feedback -> 
                match feedback.BlockReason.HasValue with
                | true  -> failwith feedback.BlockReason.Value.Value
                | _ -> ()

                match feedback.BlockReasonMessage with
                | null -> ()
                | message -> failwith message

            match response.Candidates with
            | null -> failwith $"No Candidates in the response"
            | candidates ->
                match candidates[0].Content with
                | null -> failwith $"No Content in the response Candidate"
                | content ->
                    match content.Parts with
                    | null -> failwith $"No Parts in response Candidate Parts"
                    | parts ->
                        //parts.ForEach(fun part ->
                        for part in parts do
                            if not (isNull part.Text) then System.Console.WriteLine(part.Text)
                            match part.InlineData with
                            | null -> ()
                            | blob ->
                                match blob.Data with
                                | null -> ()
                                | data ->
                                    do! File.WriteAllBytesAsync(outputFile, data, ct)
                                    System.Console.WriteLine("Done!")

        with ex -> 
            this.LogError "GenerateMusicClip" ex
            return failwith $"Failed to call GenerateMusicClip. {ex}"
    }