# TODO


[ ] Try to optimize Files Manager v1.
[ ] Files Manager. Add tests for spaces in file and directory name 
[ ] Files Manager. Builder strategy to be able to add multiple Files Manager.
[ ] DeepSeek custom chat options to solve the error:
    System.ClientModel.ClientResultException: HTTP 400 (invalid_request_error: invalid_request_error)
    The `reasoning_content` in the thinking mode must be passed back to the API.

[ ] granite4.1:8b returns use the tools correctly, but its response is an "interpretation" of the command instead of the actual result.

[ ] Add Ollama option for context window size

var options = new OllamaOptions
{
    ContextWindowSize = 32768,  // Or 65536 if VRAM allows
    // Other settings...
};
var client = new OllamaChatClient("granite4.1:8b", options, httpClient);

[ ] Finish to see simple introductory: https://www.aihero.dev/what-is-the-context-window

[ ] Watch this vidkeo: https://www.youtube.com/watch?v=ubPOWYtXtQ4
