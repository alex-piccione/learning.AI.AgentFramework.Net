namespace Tools.Expenses

open System
open ModelContextProtocol.Client;
open Microsoft.Extensions.AI

type ExpensesTools private (logger, mcpTools:seq<McpClientTool>) =
    inherit Tools.ToolsBase(logger)

    static member CreateAsync(logger, mcpUrl, loggerFactory) = task {
        let options = HttpClientTransportOptions(Endpoint=Uri(mcpUrl))
        options.Name <- "Expenses"

        // ModelContextProtocol (v1.2, 11/04/2026) does not manage JSON response properly (in streaable-http mode)
        // so we need to use "old" SSE mode

        options.TransportMode <- HttpTransportMode.Sse
        //options.KnownSessionId <- Guid.NewGuid().ToString()

        let clientTransport = HttpClientTransport(options, loggerFactory)
        let! mcpClient = McpClient.CreateAsync clientTransport

        match mcpClient.SessionId with
        | null | "" -> ()
            //logger.Log(LogLevel. "No Session ID returned from server. Stateless mode assumed.")
        | sid -> 
            //logger.LogInformation("Captured Session ID: {sid}", sid)
            // INJECT the header for all future requests in this transport
            //mcpClient.
            ()
            //options.AdditionalHeaders.Add (map ["mcp-session-id", sid])


        let! mcpTools = mcpClient.ListToolsAsync()
        return ExpensesTools (logger, mcpTools)
    } 

    override this.GetTools() = 
        mcpTools |> Seq.map (fun t -> t :> AITool)
