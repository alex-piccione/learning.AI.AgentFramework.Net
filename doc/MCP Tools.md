# MCP Tools

Add a MCP server as tools.
You need the _ModelContextProtocol_ package.  
  
```fsharp
open ModelContextProtocol.Client;

type ExampleTools private (logger, mcpTools:seq<McpClientTool>) =
    inherit Tools.ToolsBase(logger)

    member this.GetTools() = mcpTools

    static member CreateAsync(logger, mcpUrl, loggingFactory) = task {
        let options = HttpClientTransportOptions(Endpoint=mcpUrl)
        options.Name <- "Example Tools"
        //options.OAuth <-

        let clientTransport = HttpClientTransport(options, loggingFactory)
        let! mcpClient = McpClient.CreateAsync clientTransport
        let! mcpTools = mcpClient.ListToolsAsync()
        return ExampleTools (logger, mcpTools)
    } 

    override this.GetTools() =
        mcpTools |> Seq.map (fun t -> t :> AITool)
```

## Free MCP server

https://toolradar.com/blog/free-mcp-servers

```
"filesystem": {
  "command": "npx",
  "args": ["-y", "@modelcontextprotocol/server-filesystem", "/Users/you/projects"]
}
```

