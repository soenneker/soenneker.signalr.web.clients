namespace Soenneker.SignalR.Web.Clients;

/// <summary>Limits retained clients and controls repeated registration. Values are copied at collection construction.</summary>
public sealed class SignalRWebClientsOptions
{
    /// <summary>Maximum retained entries, including entries being disposed. Default is 1024. Remove unused entries explicitly.</summary>
    public int Capacity { get; set; } = 1024;
    /// <summary>Whether incompatible options for an existing identifier throw. Default is true. False explicitly selects first-registration-wins behavior.</summary>
    public bool RejectConfigurationConflicts { get; set; } = true;
}
