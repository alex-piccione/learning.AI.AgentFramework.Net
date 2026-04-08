module Settings

type AIService =
    | OpenAI
    | LocalOllama
    | AliBaba
    | AliBabaPlan
    | GitHub
    | Mistral

let service = AIService.AliBaba

let OllamaModel
    //= "llama3.2:3b"
    //= "llama3.1:8b"
    //= "qwen3.5:9b"     8.8GB   *** TOO BIG ***
    //= "nemotron-3-nano:4b-q8_0"
    //= "llama3-groq-tool-use"
    //= "deepseek-r1:8b"    ** does not support tools ***
    //= "llama3-groq-tool-use:latest"
    = "granite4:3b"
    //= "granite4:3b-h"
    //= "functiongemma:270m"

(*
llama3.2:3b (2GB, excellent tool use, multilingual/agentic) – Best starter.
dolphin-llama3:8b (4.7GB, strong coding/tools, uncensored).
qwen3.5:4b or qwen3.5:2b (3.4GB/2.7GB, good multilingual/tools, but finicky—test first).
nemotron-3-nano:4b-q8_0 (4.2GB, solid tools).
*)

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
| llama3-groq-tool-use:latest   | ❌ | With 1 call to wise it pretended to have 2 exchange rates... non-sense. |
| granite4:3b          |
| granite4:3b-h        | V3    | 


Test Agent tools correct use



✅ or ✔️
❌ or ⚠️
🤖
🤔 or 🧠
💡
🎯 Target
⚙️ Settings
🛠️ Tools
*)
