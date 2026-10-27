using System;
using System.Collections.Generic;
using System.Linq;
using Soenneker.SignalR.Web.Client.Options;

namespace Soenneker.SignalR.Web.Clients;

internal sealed class ClientConfiguration
{
    public SignalRWebClientOptions Options { get; }
    public ClientConfiguration(SignalRWebClientOptions options)
    {
        if ((object)options is ICloneable cloneable)
        {
            Options = (SignalRWebClientOptions)cloneable.Clone();
            return;
        }
        Options = new SignalRWebClientOptions
        {
            HubUrl = options.HubUrl,
            MaxRetryAttempts = options.MaxRetryAttempts,
            ReconnectIndefinitely = options.ReconnectIndefinitely,
            InitialRetryDelay = options.InitialRetryDelay,
            ConnectionAttemptTimeout = options.ConnectionAttemptTimeout,
            AccessTokenTimeout = options.AccessTokenTimeout,
            RestorationTimeout = options.RestorationTimeout,
            AutomaticReconnectTimeout = options.AutomaticReconnectTimeout,
            AuthenticationRetryDelay = options.AuthenticationRetryDelay,
            ResumeDetectionThreshold = options.ResumeDetectionThreshold,
            TimeProvider = options.TimeProvider,
            Logger = options.Logger,
            Log = options.Log,
            AccessTokenProvider = options.AccessTokenProvider,
            AccessTokenProviderWithCancellation = options.AccessTokenProviderWithCancellation,
            HttpMessageHandlerFactory = options.HttpMessageHandlerFactory,
            TransportType = options.TransportType,
            KeepAliveInterval = options.KeepAliveInterval,
            ServerTimeout = options.ServerTimeout,
            RetryDelayProvider = options.RetryDelayProvider,
            UseRetryJitter = options.UseRetryJitter,
            ConnectionClosed = options.ConnectionClosed,
            ConnectionReconnecting = options.ConnectionReconnecting,
            ConnectionReconnected = options.ConnectionReconnected,
            ConnectionRestored = options.ConnectionRestored,
            RetriesExhausted = options.RetriesExhausted,
            ConnectionError = options.ConnectionError,
            StatefulReconnect = options.StatefulReconnect,
            StatefulReconnectBufferSize = options.StatefulReconnectBufferSize,
            Headers = options.Headers == null ? null : new Dictionary<string, string>(options.Headers, StringComparer.OrdinalIgnoreCase)
        };
    }
    public bool Matches(SignalRWebClientOptions other) =>
        (object)Options is IEquatable<SignalRWebClientOptions> comparable ? comparable.Equals(other) : MatchesLegacy(other);

    private bool MatchesLegacy(SignalRWebClientOptions other) =>
        Equals(Options.HubUrl, other.HubUrl) &&
        Equals(Options.MaxRetryAttempts, other.MaxRetryAttempts) &&
        Equals(Options.ReconnectIndefinitely, other.ReconnectIndefinitely) &&
        Equals(Options.InitialRetryDelay, other.InitialRetryDelay) &&
        Equals(Options.ConnectionAttemptTimeout, other.ConnectionAttemptTimeout) &&
        Equals(Options.AccessTokenTimeout, other.AccessTokenTimeout) &&
        Equals(Options.RestorationTimeout, other.RestorationTimeout) &&
        Equals(Options.AutomaticReconnectTimeout, other.AutomaticReconnectTimeout) &&
        Equals(Options.AuthenticationRetryDelay, other.AuthenticationRetryDelay) &&
        Equals(Options.ResumeDetectionThreshold, other.ResumeDetectionThreshold) &&
        Equals(Options.TimeProvider, other.TimeProvider) &&
        Equals(Options.Logger, other.Logger) &&
        Equals(Options.Log, other.Log) &&
        Equals(Options.AccessTokenProvider, other.AccessTokenProvider) &&
        Equals(Options.AccessTokenProviderWithCancellation, other.AccessTokenProviderWithCancellation) &&
        Equals(Options.HttpMessageHandlerFactory, other.HttpMessageHandlerFactory) &&
        Equals(Options.TransportType, other.TransportType) &&
        Equals(Options.KeepAliveInterval, other.KeepAliveInterval) &&
        Equals(Options.ServerTimeout, other.ServerTimeout) &&
        Equals(Options.RetryDelayProvider, other.RetryDelayProvider) &&
        Equals(Options.UseRetryJitter, other.UseRetryJitter) &&
        Equals(Options.ConnectionClosed, other.ConnectionClosed) &&
        Equals(Options.ConnectionReconnecting, other.ConnectionReconnecting) &&
        Equals(Options.ConnectionReconnected, other.ConnectionReconnected) &&
        Equals(Options.ConnectionRestored, other.ConnectionRestored) &&
        Equals(Options.RetriesExhausted, other.RetriesExhausted) &&
        Equals(Options.ConnectionError, other.ConnectionError) &&
        Equals(Options.StatefulReconnect, other.StatefulReconnect) &&
        Equals(Options.StatefulReconnectBufferSize, other.StatefulReconnectBufferSize) &&
        HeadersMatch(Options.Headers, other.Headers);

    private static bool HeadersMatch(IDictionary<string, string>? first, IDictionary<string, string>? second)
    {
        if ((first?.Count ?? 0) != (second?.Count ?? 0)) return false;
        return first == null || first.All(pair => second != null && second.Any(candidate =>
            string.Equals(pair.Key, candidate.Key, StringComparison.OrdinalIgnoreCase) && pair.Value == candidate.Value));
    }
}
