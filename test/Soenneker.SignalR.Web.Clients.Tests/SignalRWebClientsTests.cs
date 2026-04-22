using Soenneker.SignalR.Web.Clients.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.SignalR.Web.Clients.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class SignalRWebClientsTests : HostedUnitTest
{
    private readonly ISignalRWebClients _util;

    public SignalRWebClientsTests(Host host) : base(host)
    {
        _util = Resolve<ISignalRWebClients>(true);
    }

    [Test]
    public void Default()
    {

    }
}
