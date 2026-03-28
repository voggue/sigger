# Sigger

> Important note: sigger is currently still under development and cannot yet be used as a product.
> Since the interface declarations and interfaces are not yet sufficiently stable.

Sigger is essentially a code generator that generates client code from a SignalR hub.
A schema file is generated for the application, which then generates client code via an npm application (`sigger-gen`).
The name is derived from swagger (see [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)) but for SignalR.

## What is Sigger

**Target frameworks:** the NuGet libraries **Voggue.Sigger**, **Voggue.Sigger.Abstractions**, **Voggue.Sigger.Extensions**, and **Voggue.Sigger.UI** multi-target **.NET 8, 9, and 10** (`TargetFrameworks` in the `.csproj` files). The repo root [global.json](global.json) pins a .NET SDK that can build all three (for local work you may need the [.NET 10 runtime/SDK](https://dotnet.microsoft.com/download) or a matching preview, depending on your machine).

Sigger consists of several parts:

- **Backend:** parses SignalR hubs, serves the JSON schema (for codegen), optional middleware.
- **Client:** `sigger-gen` (npm) generates stubs (Angular first; further generators possible).
- **Sigger.Extensions:** helpers such as a user/topic registry.
- **Sigger.UI:** embedded dev UI to exercise hubs (by default only in **Development**; configurable).

## Security

See [SECURITY.md](SECURITY.md) (schema endpoint, Sigger UI, CORS, `sigger-gen` TLS).

## Getting started

[doc/GettingStarted.md](doc/GettingStarted.md)

## Build / CI

- **Backend:** from `backend/`: `dotnet build Sigger.sln`, `dotnet test Sigger.sln`. Before packaging **Sigger.UI**, build the Svelte client: `backend/src/Sigger.UI/Client/sigger-ui` → `npm ci` && `npm run deploy` (copies bundles into `Resources/build/`).
- **sigger-gen:** from `client/sigger-gen/`: `npm ci` && `npm test`.
- GitHub Actions: [.github/workflows/ci.yml](.github/workflows/ci.yml).
