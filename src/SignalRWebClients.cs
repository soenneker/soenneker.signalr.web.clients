using Soenneker.SignalR.Web.Clients.Abstract;
using Soenneker.Utils.SingletonDictionary;
using Soenneker.SignalR.Web.Client;
using Soenneker.SignalR.Web.Client.Options;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Soenneker.SignalR.Web.Clients;

/// <inheritdoc cref="ISignalRWebClients"/>
public class SignalRWebClients: ISignalRWebClients
{
    private readonly SingletonDictionary<SignalRWebClient> _clients;

    public SignalRWebClients()
    {
        _clients = new SingletonDictionary<SignalRWebClient>(objects =>
        {
            if (objects.Length == 0)
                throw new Exception("SignalR options are required on first Get() call");

            var options = (SignalRWebClientOptions)objects[0];

            var client = new SignalRWebClient(options);

            return client;
        });
    }

    public ValueTask<SignalRWebClient> Get(string id, SignalRWebClientOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _clients.Get(id, cancellationToken, options!);
    }

    public SignalRWebClient GetSync(string id, SignalRWebClientOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _clients.GetSync(id, cancellationToken, options!);
    }

    public ValueTask Remove(string id)
    {
        return _clients.Remove(id);
    }

    public void RemoveSync(string id)
    {
        _clients.RemoveSync(id);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return _clients.DisposeAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _clients.Dispose();
    }
}
