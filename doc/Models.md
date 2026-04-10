# Models

## MCP server

To add a MCP server as tools:


## GitHub models

https://docs.github.com/en/billing/reference/costs-for-github-models

``curl -H "Authorization: Bearer $GITHUB_TOKEN" https://models.github.ai/catalog/models``

Updated on 10/04/2026  

| Model Name                | Model ID                                    | Cost (1M tokens in/out) | Capabilities     | Notes                                               |
|---------------------------|---------------------------------------------|--------------|-----------------------------|-----------------------------------------------------|
| OpenAI GPT-4.1            | openai/gpt-4.1                              | 2.00 / 8.00  | tools, streaming, agents    | Strong coding, instruction following, long context  |
| OpenAI GPT-4.1 mini       | openai/gpt-4.1-mini                         | 0.40 / 1.60  | tools, streaming, agents    | Faster/cheaper GPT-4.1                              |
| OpenAI GPT-4.1 nano       | openai/gpt-4.1-nano                         |              | tools, streaming, agents    | Lowest latency/cost GPT-4.1                         |
| OpenAI GPT-4o             | openai/gpt-4o                               | 2.50 / 10.00 | tools, streaming, agents    | Multimodal: text + image + audio                    |
| OpenAI GPT-4o mini        | openai/gpt-4o-mini                          | 0.15 / 0.60  | tools, streaming, agents    | Cheap, multimodal                                   |
| OpenAI gpt-5              | openai/gpt-5                                |              | tools, streaming, reasoning | Logic-heavy, multi-step tasks                       |
| OpenAI gpt-5-chat         | openai/gpt-5-chat                           |              | tools, streaming, reasoning | Multimodal, conversational                          |
| OpenAI gpt-5-mini         | openai/gpt-5-mini                           |              | tools, streaming, reasoning | Cost-sensitive apps                                 |
| OpenAI gpt-5-nano         | openai/gpt-5-nano                           |              | tools, streaming, reasoning | Optimized for speed/latency                         |
| OpenAI o1                 | openai/o1                                   |              | tools, reasoning            | Advanced reasoning, math, science                   |
| OpenAI o1-mini            | openai/o1-mini                              |              | reasoning, streaming        | Cheaper o1, good at coding                          |
| OpenAI o1-preview         | openai/o1-preview                           |              | reasoning                   | Advanced reasoning, no tools                        |
| OpenAI o3                 | openai/o3                                   |              | tools, streaming, reasoning | Improved o1, multimodal                             |
| OpenAI o3-mini            | openai/o3-mini                              |              | tools, streaming, reasoning | Cost-efficient high performance                     |
| OpenAI o4-mini            | openai/o4-mini                              |              | tools, streaming, reasoning | Improved o3-mini, multimodal                        |
| OpenAI Embedding 3 large  | openai/text-embedding-3-large               |              | embeddings                  | RAG, semantic search                                |
| OpenAI Embedding 3 small  | openai/text-embedding-3-small               |              | embeddings                  | RAG, semantic search                                |
| AI21 Jamba 1.5 Large      | ai21-labs/ai21-jamba-1.5-large              |              | tools, streaming            | 256K context, multilingual, RAG                     |
| Cohere Command A          | cohere/cohere-command-a                     |              |                             | Agentic, multilingual                               |
| Cohere Command R 08-2024  | cohere/cohere-command-r-08-2024             |              | streaming                   | RAG, multilingual                                   |
| Cohere Command R+ 08-2024 | cohere/cohere-command-r-plus-08-2024        |              | tools, streaming            | RAG-optimized, enterprise                           |
| DeepSeek-R1               | deepseek/deepseek-r1                        | 1.35 / 5.40  | tools, streaming, reasoning | Thinking/reasoning model                            |
| DeepSeek-R1-0528          | deepseek/deepseek-r1-0528                   | 1.35 / 5.40  | tools, streaming, reasoning | Updated R1, better tools + coding                   |
| DeepSeek-V3-0324          | deepseek/deepseek-v3-0324                   | 1.14 / 4.56  | tools, streaming            | Strong general-purpose, non-thinking                |
| Llama 3.2 11B Vision      | meta/llama-3.2-11b-vision-instruct          |              | streaming                   | Multimodal: text + image + audio                    |
| Llama 3.2 90B Vision      | meta/llama-3.2-90b-vision-instruct          |              | streaming                   | Multimodal: text + image + audio                    |
| Llama 3.3 70B Instruct    | meta/llama-3.3-70b-instruct                 | 0.71 / 0.71  | streaming                   | Strong open-weight general model                    |
| Llama 4 Maverick 17B FP8  | meta/llama-4-maverick-17b-128e-instruct-fp8 | 0.25 / 1.00  | tools, streaming, agents    | Multimodal, 1M context                              |
| Llama 4 Scout 17B 16E     | meta/llama-4-scout-17b-16e-instruct         |              | tools, streaming, agents    | Multimodal, 10M context                             |
| Llama 3.1 405B Instruct   | meta/meta-llama-3.1-405b-instruct           |              | agents                      | Multilingual dialogue                               |
| Llama 3.1 8B Instruct     | meta/meta-llama-3.1-8b-instruct             |              | streaming                   | Lightweight, multilingual                           |
| Codestral 25.01           | mistral-ai/codestral-2501                   |              | streaming                   | 80+ languages, code completion                      |
| Ministral 3B              | mistral-ai/ministral-3b                     |              | tools, streaming            | Edge/on-device, low latency                         |
| Mistral Medium 3 (25.05)  | mistral-ai/mistral-medium-2505              |              | tools, streaming            | Reasoning + vision                                  |
| Mistral Small 3.1         | mistral-ai/mistral-small-2503               |              | tools, streaming, agents    | Multimodal, 128K context                            |
| Grok 3                    | xai/grok-3                                  | 3.00 / 15.00 | agents                      | Finance, healthcare, law                            |
| Grok 3 Mini               | xai/grok-3-mini                             | 0.25 / 1.27  | agents, reasoning           | Thinking model, math/logic                          |
| MAI-DS-R1                 | microsoft/mai-ds-r1                         | 1.35 / 5.40  | streaming, reasoning        | Microsoft fine-tune of DeepSeek-R1                  |
| Phi-4                     | microsoft/phi-4                             | 0.13 / 0.50  |                             | 14B, low latency, reasoning                         |
| Phi-4 mini instruct       | microsoft/phi-4-mini-instruct               | 0.08 / 0.30  |                             | 3.8B, math, coding, tools                           |
| Phi-4 mini reasoning      | microsoft/phi-4-mini-reasoning              |              | reasoning                   | Lightweight math/multi-step reasoning               |
| Phi-4 multimodal instruct | microsoft/phi-4-multimodal-instruct         | 0.08 / 0.32  | streaming                   | Text + audio + image inputs                         |
| Phi-4 reasoning           | microsoft/phi-4-reasoning                   |              | reasoning, streaming        | Open-weight reasoning model                         |