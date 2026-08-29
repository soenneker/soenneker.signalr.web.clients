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
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddSignalRWebClientsAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<ISignalRWebClients, SignalRWebClients>();
        return services;
    }

    /// <summary>
    /// Registers Signal R Web Clients with a scoped lifetime.
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddSignalRWebClientsAsScoped(this IServiceCollection services)
    {
        services.TryAddScoped<ISignalRWebClients, SignalRWebClients>();
        return services;
    }
}
