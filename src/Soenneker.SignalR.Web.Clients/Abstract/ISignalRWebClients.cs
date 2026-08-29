using Soenneker.SignalR.Web.Client.Options;
using Soenneker.SignalR.Web.Client;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.SignalR.Web.Clients.Abstract;

/// <summary>
/// Providing async thread-safe resilient and dependable SignalR web client singletons
/// </summary>
public interface ISignalRWebClients : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Gets a SignalR web client by its identifier, creating it if it doesn't already exist.
    /// </summary>
    /// <param name="id">Identifier of the Signal R Web Clients instance or registration to target.</param>
    /// <param name="options">The options used to configure the SignalR web client.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task whose result is the requested signal R Web Client.</returns>
    ValueTask<SignalRWebClient> Get(string id, SignalRWebClientOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronously gets a SignalR web client by its identifier, creating it if it doesn't already exist.
    /// </summary>
    /// <param name="id">The identifier of the SignalR web client.</param>
    /// <param name="options">The options used to configure the SignalR web client.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The SignalR web client.</returns>
    SignalRWebClient GetSync(string id, SignalRWebClientOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a SignalR web client by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the SignalR web client to remove.</param>
    /// <returns>true if removes a SignalR web client by its identifier; otherwise, false.</returns>
    ValueTask<bool> Remove(string id);

    /// <summary>
    /// Synchronously removes a SignalR web client by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the SignalR web client to remove.</param>
    void RemoveSync(string id);
}
