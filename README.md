[![](https://img.shields.io/nuget/v/soenneker.signalr.web.clients.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.signalr.web.clients/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.signalr.web.clients/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.signalr.web.clients/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.signalr.web.clients.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.signalr.web.clients/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.signalr.web.clients/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.signalr.web.clients/actions/workflows/codeql.yml)

# Soenneker.SignalR.Web.Clients

A thread-safe, keyed collection of reusable `SignalRWebClient` instances with coordinated creation and disposal.

## Installation

```bash
dotnet add package Soenneker.SignalR.Web.Clients
```

## Registration

```csharp
using Soenneker.SignalR.Web.Clients.Registrars;

builder.Services.AddSignalRWebClientsAsSingleton();
```

Use `AddSignalRWebClientsAsSingleton` when connections should live for the application lifetime. `AddSignalRWebClientsAsScoped` creates a separate keyed collection per dependency-injection scope and disposes its connections when that scope ends.

## Creating and using a client

```csharp
using Microsoft.AspNetCore.SignalR.Client;
using Soenneker.SignalR.Web.Client.Options;
using Soenneker.SignalR.Web.Clients.Abstract;

public sealed class UpdateSubscriber(ISignalRWebClients clients)
{
    public async Task Start(CancellationToken cancellationToken)
    {
        var options = new SignalRWebClientOptions
        {
            HubUrl = "https://api.example.com/hubs/updates",
            AccessTokenProvider = () => Task.FromResult(GetAccessToken())
        };

        var client = await clients.Get("updates", options, cancellationToken);

        client.Connection.On<OrderUpdated>("OrderUpdated", Apply);
        await client.StartConnection(cancellationToken);
    }
}
```

The identifier defines the cache entry. Concurrent calls for the same identifier receive the same client. Options are used only when that identifier is first created; later calls return the existing instance rather than reconfiguring it. Always supply valid options when an identifier may not exist yet.

`Get` creates or retrieves a client but does not start its connection. Register hub handlers before calling `StartConnection`.

## Removing clients

```csharp
bool removed = await clients.Remove("updates");
```

Removing an identifier also disposes that client and its underlying SignalR connection. A later `Get` creates a new instance. Prefer the asynchronous `Get` and `Remove` methods because SignalR connection disposal is asynchronous; the synchronous variants block until asynchronous cleanup completes.

Disposing `ISignalRWebClients` disposes every cached client. Clients returned by the collection are owned by the collection and should not be disposed independently.
