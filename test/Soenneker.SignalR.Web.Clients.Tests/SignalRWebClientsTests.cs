using Soenneker.SignalR.Web.Clients.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.SignalR.Web.Clients.Tests;

[Collection("Collection")]
public class SignalRWebClientsTests : FixturedUnitTest
{
    private readonly ISignalRWebClients _util;

    public SignalRWebClientsTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<ISignalRWebClients>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
