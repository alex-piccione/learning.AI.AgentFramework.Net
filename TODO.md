# TODO

## In Progress


 


## Backlog

[ ] Try to optimize Files Manager v1.
[ ] Files Manager. Builder strategy to be able to add multiple Files Manager.
[ ] DeepSeek custom chat options to solve the error:
    System.ClientModel.ClientResultException: HTTP 400 (invalid_request_error: invalid_request_error)
    The `reasoning_content` in the thinking mode must be passed back to the API.

[ ] granite4.1:8b returns use the tools correctly, but its response is an "interpretation" of the command instead of the actual result.

[ ] Add Ollama option for context window size

[ ] Finish to see simple introductory: https://www.aihero.dev/what-is-the-context-window

[ ] Watch this video: https://www.youtube.com/watch?v=ubPOWYtXtQ4

[ ] TESTS: should we check a UNC path is not passed to FileSystem Agent?


## Issue with too many records for Files Manager

Found files must be passed in context, not in the prompt.  
Agents should use a shared context.

---

The agent, despite using corectly ListFiles and ListDirectories recursively cannot manage too many informations.
Maybe it has to store the obtained info somewhere while it has not ALL the data to compose the result.

```ai prompt
Microsoft Agent Framework

I'm creating a "tool" that use agents to search in a file system.  
The orchestrator agent has a sub-agen FilesManager that can list all the direcptries and files using ListDirectories(path) and ListFiles(path). 
So, also having provided agent instructions that says to use the LKistDirectory for a sub-folder scan, it has all the tools.
The problem is that often it stops at the root level only.
For example if I ask to give me all the .mp3 files in a folder and all the subfolders it only provides the files on the root folder.  
It  actually completely fail when hte folder containes thousands of files.
I'm using a 8B model with a lomited 4K context.

I don't know how it is working internally but I' imaginign that the model (the orchestrator agent) is struggling to maintain the temporary information (one folder list result) and to compose the result at the end.
Again, I can't imagine how it work to colelct the data and then generate a result, but maybe an agent that help to "temporary store" the data can help.
I'm sure that also an in-memory one can manage thousands of record that are just a path and a info like "file" or "directory". 
Also if that info is a json structure.
I can also provide a tool thta stoer key-values if it helps (or Tree structure but it can  start to be complicated).

Is the problem clear, or I need to explain better?

What do you think? 
Any idea of the possible issue and if my reasoning of the temoprary stoer is effectively a possible solution?


```




