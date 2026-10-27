using System.Threading.Tasks;
using Soenneker.SignalR.Web.Client;

namespace Soenneker.SignalR.Web.Clients;

internal sealed class ClientEntry(SignalRWebClient client, ClientConfiguration configuration)
{
    public SignalRWebClient Client { get; } = client;
    public ClientConfiguration Configuration { get; } = configuration;
    public Task? Removal;
}
