# Project Context

## Build
- `cd src && dotnet build *.slnx` - Build the solution
- **Preferred**: `dotnet build src/` - Simpler build form (user preference)

## Project Structure
- F# project using NUnit + Unquote for testing
- Solution file (.slnx) is in the `src/` folder
- ConsoleApp/ (entry point, helpers, settings, secrets)
- Library/ (agents, tools, utilities)
- UnitTests/ (test assemblies)

## Key Info
- Microsoft Agents Framework with OpenAI-compatible API
- Local Ollama support for running models locally

## TODO.md

Check TODO.md for pending tasks and improvements:
- Files Manager: Add tests for spaces in file and directory names
- Files Manager: Add builder strategy for multiple File Managers
- Handle special characters in file names
- Fix DeepSeek custom chat options for thinking mode
- Address Granite 4.1:8b response interpretation issues
- Add Ollama context window size options
