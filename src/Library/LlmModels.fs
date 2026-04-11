module LlmModels

    module public OpenAI =
        let GPT_5_2 = "gpt-5.2"
        let GPT_5_mini = "gpt-5-mini"
        let GPT_5_nano = "gpt-5-nano"

    module public Alibaba =
        let Qwen_3_6_plus = "qwen3.6-plus"
        let Qwen_3_5_plus = "qwen3.5-plus"
        //let Qwen3_max_2026_01_23 = "qwen3-max-2026-01-23"
        //let Qwen_Coder_Next = "qwen3-coder-next"
        //let Qwen_Coder_Plus = "qwen3-coder-plus"
        //let Zhipu = "glm-5"
        //let Kimi = "kimi-k2.5"
        //let MiniMax = "MiniMax-M2.5"

    module public AlibabaPlan =
        let Qwen_3_6_plus = "qwen3.6-plus"
        let Qwen_3_5_plus = "qwen3.5-plus"
        let Qwen3_max_2026_01_23 = "qwen3-max-2026-01-23"
        let Qwen_Coder_Next = "qwen3-coder-next"
        let Qwen_Coder_Plus = "qwen3-coder-plus"
        let Zhipu = "glm-5"
        let Kimi = "kimi-k2.5"
        let MiniMax = "MiniMax-M2.5"


    module public GitHub =
        let Phi_4_mini_instruct = "microsoft/Phi-4-mini-instruct"   // it returns the tool to call, not hte response
        let Phi_4 = "microsoft/phi-4"
        let Grok_3_mini = "xai/grok-3-mini"    // too small context, slow, not capable of complete
        let Llama_4_Maverick_17B_FP8 = "meta/llama-4-maverick-17b-128e-instruct-fp8"   // STUPID
        let Ministral_3_1_small = "mistral-ai/mistral-small-2503"    // WOW
        let Grok_3 = "xai/grok-3"   // Slow, Good for Plan
        //let DEEPSEEK_R1 = "deepseek/DeepSeek-R1"  // does not support tools
        //let DeepSeek_R1_0528 = "deepseek/DeepSeek-R1-0528"  // erro calling tool
        //let DeepSeek_V3_0324 = "deepseek/DeepSeek-V3-0324" 


    module public Mistral =
        let MINISTRAL_14b_2512 = "ministral-14b-2512"
        let MISTRAL_MEDIUM_2505 = "mistral-medium-2505"
        let magistral_medium_2509 = "magistral-medium-2509"


    module public Openrouter =
        let Gemma_4_26B_A4B_Free = "google/gemma-4-26b-a4b-it:free"
        let Gemma_4_26B_A4B = "google/gemma-4-26b-a4b-it"
        let Gemma_4_31B_Free = "google/gemma-4-31b-it:free"
        let Gemma_4_31B = "google/gemma-4-31b-it"
        let Qwen_3_6_Plus = "qwen/qwen3.6-plus"
        let Google_Lyria_3_Pro_Preview = "google/lyria-3-pro-preview" // generate songs
        let Alibaba_Wan_2_6_experimental = "alibaba/wan-2.6" // generate videos
        let Nemotron_3_Super_Free = "nvidia/nemotron-3-super-120b-a12b:free"
        let Nemotron_3_Super = "nvidia/nemotron-3-super-120b-a12b"