using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.SignalR.Web.Clients.Tests;

internal sealed class BlockingDisposalHandler : HttpMessageHandler
{
    public TaskCompletionSource Disposing { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public ManualResetEventSlim Release { get; } = new();
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromException<HttpResponseMessage>(new HttpRequestException("Unavailable"));
    protected override void Dispose(bool disposing)
    {
        if (disposing) { Disposing.TrySetResult(); Release.Wait(); }
        base.Dispose(disposing);
    }
}
