using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Owin;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Middleware.Test
{
    public class ContextTracingMiddlewareTests
    {
        private readonly string correlationId = Guid.NewGuid().ToString();

        [Fact]
        public void NullFactory()
        {
            Assert.Throws<ArgumentNullException>(() => new ContextTracingMiddleware<string>(assertEmptyContext, null));
        }

        [Fact]
        public async Task NoHeader()
        {
            var middleware = new ContextTracingMiddleware<CorrelationContext>(assertEmptyContext, new OwinCorrelationContextFactory());
            await middleware.Invoke(createEmptyRequest());
        }

        [Fact]
        public async Task FromHeader()
        {
            var middleware = new ContextTracingMiddleware<CorrelationContext>(AssertContext, new OwinCorrelationContextFactory());
            await middleware.Invoke(createRequest(correlationId));
        }

        private Task assertEmptyContext(IDictionary<string, object> environment)
        {
            var ctx = ContextResolver.GetContext<CorrelationContext>();
            Assert.NotNull(ctx);
            return Task.FromResult(1);
        }

        private Task AssertContext(IDictionary<string, object> environment)
        {
            var ctx = ContextResolver.GetContext<CorrelationContext>();
            Assert.NotNull(ctx);
            Assert.Equal(correlationId, ctx.CorrelationId);
            return Task.FromResult(1);
        }

        private IDictionary<string, object> createEmptyRequest()
        {
            return new Dictionary<string, object>
            {
                {"owin.RequestHeaders", new Dictionary<string, string[]>()}
            };
        }

        private IDictionary<string, object> createRequest(string header)
        {
            var headerDict = new Dictionary<string, string[]>
            {
                {CorrelationHeaderInfo.CorrelationIdHeaderName, new[] {header}}
            };

            return new Dictionary<string, object>
            {
                {"owin.RequestHeaders", headerDict }
            };
        }
    }
}