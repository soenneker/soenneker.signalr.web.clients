using Soenneker.SignalR.Web.Clients.Abstract;
using Soenneker.Utils.SingletonDictionary;
using Soenneker.SignalR.Web.Client;
using Soenneker.SignalR.Web.Client.Options;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.SignalR.Web.Clients;

/// <inheritdoc cref="ISignalRWebClients"/>
public sealed class SignalRWebClients: ISignalRWebClients
{
    private readonly SingletonDictionary<SignalRWebClient, SignalRWebClientOptions> _clients;

    public SignalRWebClients()
    {
        _clients = new SingletonDictionary<SignalRWebClient, SignalRWebClientOptions>(options => new SignalRWebClient(options));
    }

    public ValueTask<SignalRWebClient> Get(string id, SignalRWebClientOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _clients.Get(id, options, cancellationToken);
    }

    public SignalRWebClient GetSync(string id, SignalRWebClientOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _clients.GetSync(id, options, cancellationToken);
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
        return _clients.DisposeAsync();
    }

    public void Dispose()
    {
        _clients.Dispose();
    }
}
