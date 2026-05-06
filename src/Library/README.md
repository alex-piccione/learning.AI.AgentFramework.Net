# AI Agent 

This library provides some helper for use Microsoft Agent Framework.  


## ChatClientBuilder

Helper to create an _IChatClient_ .  
It can be used ot customeize Agents with the preferred client.  


## AgentBuilder (to be renamed AgentFactory)

Helper to create specific Agents customized for specific roles.


## Predefined Agents

Agents:
- Files Manager: to manage files and folder from a defined root folder. It cannot see or act in a defferent path 
- Crypto (to be renamed Finance Agent): exchanges rates, crypto values, Kraken account balance and orders.
- Weather: retrieves weather information
- Musicist: create short jingles
- Expenses: manage expenses record of the user. Requires a third party MCP server.
- Orchestrator: multipurpose agent that manage all the other agents