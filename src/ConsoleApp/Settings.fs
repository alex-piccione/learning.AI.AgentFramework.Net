module Settings

type AIService =
    | OpenAI
    | LocalOllama
    | AliBaba
    | AliBabaPlan
    | GitHub
    | Mistral
    | Openrouter
    | Xiaomi
    | Google

let service = AIService.AliBaba

let OllamaModel
    //= "dolphin3:8b"              // ❌ Does not support tools
    //= "dolphin-llama3:8b"        // ❌ Does not support tools
    //= "gemma4:e2b"               // ✅ Good
    //= "bjoernb/gemma4-e4b-fast"  // ❌TOO BIG but fast
    //= "llama3.2:3b"              // ✅ ok
    //= "llama3.1:8b"              // meh, bad bad bad
    //= "qwen3:8b"                 // slow and meh
    //= "qwen3.5:4b"               // ✅ Very Good
    //= "qwen3.5:9b"               // ❌TOO BIG, 8.8GB
    //= "nemotron-3-nano:4b"       // ok
    = "nemotron-3-nano:4b-q8_0"    // ok but expensive ?
    //= "llama3-groq-tool-use"
    //= "deepseek-r1:8b"           // ❌ Does not support tools
    //= "deepseek-r1:7b"           // ❌ Does not support tools
    //= "llama3-groq-tool-use:latest"
    //= "llama3-gradient:8b"       // ❌ Does not support tools
    //= "granite4:3b"              // ✅ ok, stupid sometimes?
    //= "granite4:3b-h"
    //= "functiongemma:270m"  dolphin-llama3:8b
    //= "olmo-3:7b"                // ❌ Does not support tools
    //= "olmo-3:7b-instruct"       // ✅ ok, but stupid
    //= "olmo-3:7b-think"          // ❌ Does not support tools
    //= "orca-mini:7b"             // ❌ Does not support tools"
    //= "orca-mini:7b-v3-q8_0"     // ❌ Does not support tools"
    //= "orca-mini:13b"            // ❌ Does not support tools and is Too big (11GB) but fast!
    //= "orca-mini:13b-v2-q3_K_L"  // ❌ Does not support tools"


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