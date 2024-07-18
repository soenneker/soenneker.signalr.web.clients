using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.SignalR.Web.Clients.Abstract;

namespace Soenneker.SignalR.Web.Clients.Registrars;

/// <summary>
/// Providing async thread-safe resilient and dependable SignalR web client singletons
/// </summary>
public static class SignalRWebClientsRegistrar
{
    /// <summary>
    /// Adds <see cref="ISignalRWebClients"/> as a singleton service. <para/>
    /// </summary>
    public static void AddSignalRWebClients(this IServiceCollection services)
    {
        services.TryAddSingleton<ISignalRWebClients, SignalRWebClients>();
    }
}
