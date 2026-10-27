using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.SignalR.Web.Client.Options;

namespace Soenneker.SignalR.Web.Clients.Tests;

public sealed class SignalRWebClientsTests
{
    private static SignalRWebClientOptions Options() => new() { HubUrl = "http://localhost/hub" };

    [Test]
    public async ValueTask Concurrent_gets_share_one_client_and_disposal_rejects_new_work(CancellationToken cancellationToken)
    {
        var clients = new SignalRWebClients();
        var options = Options();
        var results = await Task.WhenAll(Enumerable.Range(0, 50).Select(_ => clients.Get("tenant:user:hub", options, cancellationToken).AsTask()));
        await Assert.That(results.All(client => ReferenceEquals(client, results[0]))).IsTrue();
        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => clients.DisposeAsync().AsTask()));
        await Assert.That(async () => await clients.Get("tenant:user:hub", options, cancellationToken)).Throws<ObjectDisposedException>();
        await Assert.That(async () => await clients.Remove("tenant:user:hub")).Throws<ObjectDisposedException>();
    }

    [Test]
    public async ValueTask Conflicting_credentials_and_mutated_headers_are_rejected(CancellationToken cancellationToken)
    {
        await using var clients = new SignalRWebClients();
        var options = Options();
        options.Headers = new Dictionary<string, string> { ["X-Tenant"] = "one" };
        var first = await clients.Get("hub", options, cancellationToken);
        options.Headers["X-Tenant"] = "two";
        await Assert.That(async () => await clients.Get("hub", options, cancellationToken)).Throws<InvalidOperationException>();
        await Assert.That(ReferenceEquals(first, await clients.Get("hub", cancellationToken: cancellationToken))).IsTrue();
        var credentials = Options();
        credentials.AccessTokenProvider = () => Task.FromResult("other-user");
        await Assert.That(async () => await clients.Get("hub", credentials, cancellationToken)).Throws<InvalidOperationException>();
    }

    [Test]
    public async ValueTask Capacity_is_reclaimed_only_after_removal_and_new_client_is_distinct(CancellationToken cancellationToken)
    {
        await using var clients = new SignalRWebClients(new() { Capacity = 1 });
        var first = await clients.Get("one", Options(), cancellationToken);
        await Assert.That(async () => await clients.Get("two", Options(), cancellationToken)).Throws<InvalidOperationException>();
        await clients.Remove("one");
        var second = await clients.Get("two", Options(), cancellationToken);
        await Assert.That(ReferenceEquals(first, second)).IsFalse();
        await Assert.That(await clients.Remove("missing")).IsFalse();
    }

    [Test]
    public async ValueTask Concurrent_removals_have_one_owner_and_replacement_is_not_disposed(CancellationToken cancellationToken)
    {
        await using var clients = new SignalRWebClients();
        var first = await clients.Get("hub", Options(), cancellationToken);
        bool[] removed = await Task.WhenAll(Enumerable.Range(0, 50).Select(_ => clients.Remove("hub").AsTask()));
        await Assert.That(removed.Count(value => value)).IsEqualTo(1);
        var second = await clients.Get("hub", Options(), cancellationToken);
        await Assert.That(ReferenceEquals(first, second)).IsFalse();
        await Assert.That(ReferenceEquals(second, await clients.Get("hub", cancellationToken: cancellationToken))).IsTrue();
    }

    [Test]
    public async ValueTask Legacy_conflict_policy_is_explicit(CancellationToken cancellationToken)
    {
        await using var clients = new SignalRWebClients(new() { RejectConfigurationConflicts = false });
        var first = await clients.Get("hub", Options(), cancellationToken);
        var changed = Options(); changed.HubUrl = "http://localhost/other";
        await Assert.That(ReferenceEquals(first, await clients.Get("hub", changed, cancellationToken))).IsTrue();
    }

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async ValueTask Removal_keeps_replacement_and_collection_disposal_waiting_for_cleanup(bool disposeCollection, CancellationToken cancellationToken)
    {
        var handler = new BlockingDisposalHandler();
        await using var clients = new SignalRWebClients();
        var options = Options();
        options.MaxRetryAttempts = 0;
        options.ReconnectIndefinitely = false;
        options.HttpMessageHandlerFactory = original => { original.Dispose(); return handler; };
        var first = await clients.Get("hub", options, cancellationToken);
        Task start = first.StartConnection(cancellationToken).AsTask();
        try
        {
            await handler.Disposing.Task.WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);
            Task<bool> remove = clients.Remove("hub").AsTask();
            Task next = disposeCollection ? clients.DisposeAsync().AsTask() : clients.Get("hub", Options(), cancellationToken).AsTask();
            await Assert.That(remove.IsCompleted).IsFalse();
            await Assert.That(next.IsCompleted).IsFalse();
            handler.Release.Set();
            try { await start; } catch (OperationCanceledException) { }
            await Task.WhenAll(remove, next).WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);
            await Assert.That(await remove).IsTrue();
            if (!disposeCollection)
                await Assert.That(ReferenceEquals(first, await clients.Get("hub", cancellationToken: cancellationToken))).IsFalse();
        }
        finally { handler.Release.Set(); }
    }
}
