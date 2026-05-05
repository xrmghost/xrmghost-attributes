# Contributing

Thank you for your interest in contributing to this project!

## Reporting Issues

Use [GitHub Issues](../../issues) to report bugs or request features. Before opening a new issue, please search existing issues to avoid duplicates. Include as much context as possible: version, .NET runtime, reproduction steps.

## Submitting a Pull Request

1. Fork the repository.
2. Create a feature branch from `main`: `git checkout -b feature/your-change`.
3. Make your changes.
4. Ensure all tests pass (see below).
5. Open a pull request against `main` with a clear description of the change and its motivation.

PRs must pass all CI checks before being reviewed.

## Build Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later

## Build and Test

```bash
# Build
dotnet build XrmGhost.Attributes.csproj -c Release

# Test
dotnet test
```

## Code Style

- Follow the existing code patterns in the repository.
- `<Nullable>enable</Nullable>` is required — all nullable warnings must be addressed.
- Target `LangVersion 8.0` as declared in the project file.
- Keep changes focused and minimal.
