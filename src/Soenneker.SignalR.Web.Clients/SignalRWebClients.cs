using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.SignalR.Web.Client;
using Soenneker.SignalR.Web.Client.Options;
using Soenneker.SignalR.Web.Clients.Abstract;

namespace Soenneker.SignalR.Web.Clients;

public sealed class SignalRWebClients : ISignalRWebClients
{
    private readonly object _gate = new();
    private readonly Dictionary<string, ClientEntry> _clients = new(StringComparer.Ordinal);
    private readonly int _capacity;
    private readonly bool _rejectConflicts;
    private bool _disposed;
    private Task? _disposal;

    public SignalRWebClients() : this(new SignalRWebClientsOptions()) { }

    public SignalRWebClients(SignalRWebClientsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfLessThan(options.Capacity, 1);
        _capacity = options.Capacity;
        _rejectConflicts = options.RejectConfigurationConflicts;
    }

    public async ValueTask<SignalRWebClient> Get(string id, SignalRWebClientOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Task removing;
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                if (_clients.TryGetValue(id, out ClientEntry? entry))
                {
                    if (entry.Removal == null)
                    {
                        if (options != null && _rejectConflicts && !entry.Configuration.Matches(options))
                            throw new InvalidOperationException("This identifier is registered with different options. Remove it before changing configuration, or use a separate tenant/user identifier.");
                        return entry.Client;
                    }
                    removing = entry.Removal;
                }
                else
                {
                    ArgumentNullException.ThrowIfNull(options);
                    if (_clients.Count >= _capacity) throw new InvalidOperationException("The SignalR client collection is at capacity. Remove an unused client before adding another.");
                    var configuration = new ClientConfiguration(options);
                    var client = new SignalRWebClient(configuration.Options);
                    _clients.Add(id, new ClientEntry(client, configuration));
                    return client;
                }
            }
            await removing.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public SignalRWebClient GetSync(string id, SignalRWebClientOptions? options = null, CancellationToken cancellationToken = default) =>
        Get(id, options, cancellationToken).AsTask().GetAwaiter().GetResult();

    public async ValueTask<bool> Remove(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Task removal;
        bool owner;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_clients.TryGetValue(id, out ClientEntry? entry)) return false;
            owner = entry.Removal == null;
            removal = BeginRemove(id, entry);
        }
        await removal.ConfigureAwait(false);
        return owner;
    }

    private Task BeginRemove(string id, ClientEntry entry) => entry.Removal ??= Task.Run(async () =>
    {
        try { await entry.Client.DisposeAsync().ConfigureAwait(false); }
        finally
        {
            lock (_gate)
                if (_clients.TryGetValue(id, out ClientEntry? current) && ReferenceEquals(current, entry)) _clients.Remove(id);
        }
    });

    public void RemoveSync(string id) => Remove(id).AsTask().GetAwaiter().GetResult();

    public ValueTask DisposeAsync()
    {
        lock (_gate)
        {
            if (_disposal != null) return new ValueTask(_disposal);
            _disposed = true;
            _disposal = Task.WhenAll(_clients.Select(pair => BeginRemove(pair.Key, pair.Value)).ToArray());
            return new ValueTask(_disposal);
        }
    }

    public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}
