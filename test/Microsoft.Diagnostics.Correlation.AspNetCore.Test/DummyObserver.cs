using System.Net.Http;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common.Instrumentation;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Test
{
    public class DummyObserver<TContext> : IOutgoingRequestNotifier<TContext, HttpRequestMessage, HttpResponseMessage> where TContext : ICorrelationContext<TContext>
    {
        public void OnBeforeRequest(TContext context, HttpRequestMessage request)
        {
        }

        public void OnAfterResponse(TContext context, HttpResponseMessage response)
        {
        }
    }
}
