# MIBO
MIBO Router

## Requirements

- [.NET SDK 10.0.100 or later](https://dotnet.microsoft.com/en-us/download) (the repository includes a `global.json` to pin this version).

## Development

Restore dependencies and run the test suite to verify your environment:

```
dotnet restore
dotnet test
```

To run the web project locally:

```
dotnet run --project src/MIBO.Web
```

## Static web assets

The Blazor runtime assets (for example `_framework/blazor.web.js`) are produced by the .NET build and are served from the framework reference; they are not committed to the repository. If you suspect missing files, run `dotnet restore` followed by a build or test run to regenerate the static assets and confirm everything resolves correctly.
