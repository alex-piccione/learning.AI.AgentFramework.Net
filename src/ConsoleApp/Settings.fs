module Settings

type AIService =
    | OpenAI
    | LocalOllama
    | AliBaba
    | AliBabaPlan
    | GitHub
    | Mistral

let service = AIService.LocalOllama

let OllamaModel    
    //= "dolphin-llama3:8b"        // ❌ Does not support tools
    //= "gemma4:e2b"               // ✅ Good
    //= "bjoernb/gemma4-e4b-fast"  // TOO BIG but fast
    //= "llama3.2:3b"                // ok
    //= "llama3.1:8b"              // meh
    //= "qwen3:8b"                 // slow and meh
    //= "qwen3.5:4b"               // ✅ Very Good
    //= "qwen3.5:9b"               // TOO BIG, 8.8GB 
    //= "nemotron-3-nano:4b-q8_0"
    //= "llama3-groq-tool-use"
    //= "deepseek-r1:8b"           // ❌ Does not support tools
    //= "llama3-groq-tool-use:latest"
    //= "llama3-gradient:8b"       // ❌ Does not support tools
    = "granite4:3b"              // ❌ stupid
    //= "granite4:3b-h"
    //= "functiongemma:270m"


(*

| Model                | Agent | Result | Note                                                            | 
|----------------------|-------|--------|-----------------------------------------------------------------|
| llama3.1:8b          | V1    | ❌     | No usage of Coingecko tool.                                     |
| llama3.2:3b          | V1    | ❌     | No usage of Coingecko tool.                                     |
| dolphin-llama3:8b    | V1    | ❌     | No usage of Coingecko tool. LIED about it ⚠️ (said it used it)  |
| llama3.1:8b          | V2    | ✅     | No usage of Coingecko tool. LIED about it ⚠️ (said it used it)  |
| qwen3.5:9b           | V2    | ❌     | No usage of Coingecko tool. No right output.                    |
| llama3.1:8b          | V3    | ❌     | 
| deepseek-r1:8b       | V3    | X      | Does not support tools                                           |
| llama3-groq-tool-use:latest  | ❌     | With 1 call to wise it pretended to have 2 exchange rates... non-sense. |
| granite4:3b          |
| granite4:3b-h        | V3    | 
| gemma4:e2b           | V3    |


✅ or ✔️
❌ or ⚠️
🤖
🤔 or 🧠
💡
🎯 Target
⚙️ Settings
🛠️ Tools
*)



//let expensesMcpServerUrl = "http://localhost:8000/mcp"
let expensesMcpServerUrl = "http://localhost:8000/sse"