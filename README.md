# Forgelingo V2 (C#)

Refactor of Forgelingo V2 into C# (net8) with Avalonia UI + Core library.

## Getting started

1. Set DEEPSEEK_API_KEY environment variable if using DeepSeek provider.

Linux/macOS:
```bash
export DEEPSEEK_API_KEY="your_key_here"
```

Windows PowerShell:
```powershell
$env:DEEPSEEK_API_KEY = "your_key_here"
```

2. Build and run (CLI or UI)
```bash
dotnet build
dotnet run --project src/Forgelingo.CLI
# or run Avalonia UI
dotnet run --project src/Forgelingo.UI
```

## Next steps
- Implement provider specifics and better prompt composition
- Improve JSON translation
- Integrate translation memory and glossary
- Add more tests
