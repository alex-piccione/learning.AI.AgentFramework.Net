# Microsoft Agents Framework 

Playground for Microsoft Agent Framework.  
  
Official docs: 
GitHub: https://github.com/microsoft/agent-framework  

This project uses OpenAI compatible API hosts.  

## OpenAI clients

Think of the **OpenAIClient** as a factory — you call .GetXxxClient() on it to get a specialized client for each API surface:
| Client | Purpose |
| --- | --- |
| Chat.ChatClient | Multi-turn conversations with models (GPT-4o, etc.) — the most common one |
| Assistants.AssistantClient | Stateful assistants with threads, memory, tool use — OpenAI's older agent abstraction |
| Responses.ResponsesClient | OpenAI's newer stateless response API (successor to chat completions) |
| Conversations.ConversationClient | Multi-party conversation management |
| Audio.AudioClient | Speech-to-text (Whisper) and text-to-speech |
| Embeddings.EmbeddingClient | Convert text to vector embeddings (for semantic search, RAG) |
| Images.ImageClient | Generate/edit images (DALL·E)Files.OpenAIFileClient | Upload files to OpenAI (for fine-tuning, assistants, batch) |
| FineTuning.FineTuningClient | Fine-tune models on your data |
| Batch.BatchClient | Submit large batches of requests asynchronously (cheaper) |
| Moderations.ModerationClient | Check if content violates usage policies |
| Models.OpenAIModelClient | List/inspect available models |
| VectorStores.VectorStoreClient | Manage vector stores for file search in |
| AssistantsRealtime.RealtimeClient | Low-latency real-time audio/text streaming (GPT-4o Realtime) |
| Graders.GraderClient | Evaluate model outputs (evals pipeline) |
| Evals.EvaluationClient | Run evaluation datasets against models |
| Containers.ContainerClient | Manage sandboxed code execution containers | 
| Videos.VideoClientVideo generation (Sora) |


## Agent Tools

- Open-Meteo - Free API for weather forecasts, historical data, and climate information. No API key required.
- weatherapi.com - not tried yet, but seems to have a free tier with 1 million calls/month. Requires API key.
- KrakenAPIClient - Client for Kraken exchange API 
- Cingecko API Client - Client for Coingecko API
- Wise aPI Client - Client for Wise API (to get fiat exchange rates, not available from Coingecko)

## OCR

https://ollama.com/library/glm-ocr

## Tools/Libraries

- Microsoft Agent Framework
- Npgsql.FSharp
- NUnit
- Unquote
- Foq