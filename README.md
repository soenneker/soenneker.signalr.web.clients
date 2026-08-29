[![](https://img.shields.io/nuget/v/soenneker.signalr.web.clients.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.signalr.web.clients/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.signalr.web.clients/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.signalr.web.clients/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.signalr.web.clients.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.signalr.web.clients/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.signalr.web.clients/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.signalr.web.clients/actions/workflows/codeql.yml)

# Soenneker.SignalR.Web.Clients

Providing async thread-safe resilient and dependable SignalR web client singletons.

## Install

```bash
dotnet add package Soenneker.SignalR.Web.Clients
```

## Quick start

```csharp
using Soenneker.SignalR.Web.Clients.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddSignalRWebClientsAsSingleton();
```

Adds `ISignalRWebClients` as a singleton service.

## What you get

- `ISignalRWebClients` — Providing async thread-safe resilient and dependable SignalR web client singletons.
- `SignalRWebClientsRegistrar` — Providing async thread-safe resilient and dependable SignalR web client singletons.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `ISignalRWebClients.Get(id, options, cancellationToken)` | Gets a SignalR web client by its identifier, creating it if it doesn't already exist. | A task whose result is the requested signal R Web Client. |
| `ISignalRWebClients.GetSync(id, options, cancellationToken)` | Synchronously gets a SignalR web client by its identifier, creating it if it doesn't already exist. | The SignalR web client. |
| `ISignalRWebClients.Remove(id)` | Removes a SignalR web client by its identifier. | true if removes a SignalR web client by its identifier; otherwise, false. |
| `ISignalRWebClients.RemoveSync(id)` | Synchronously removes a SignalR web client by its identifier. | Returns no value; the requested change is complete when the method returns. |
| `SignalRWebClientsRegistrar.AddSignalRWebClientsAsSingleton(services)` | Adds `ISignalRWebClients` as a singleton service. | The same service collection, so additional registrations can be chained. |
| `SignalRWebClientsRegistrar.AddSignalRWebClientsAsScoped(services)` | Registers Signal R Web Clients with a scoped lifetime. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
- Reuse the registered client instead of constructing one per operation.
- Calls that return a cached or singleton value reuse the same instance until the owning service is disposed.
- Dispose instances you own when their scope ends so held resources can be released.
