using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Owin;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Middleware.Test
{
    public class OwinCorrelationContextFactoryTests
    {
        private readonly OwinCorrelationContextFactory factory;

        public OwinCorrelationContextFactoryTests()
        {
            factory = new OwinCorrelationContextFactory();
        }

        [Fact]
        public void CreateContextNull()
        {
            Assert.Throws<ArgumentNullException>(() => factory.CreateContext(null));
        }

        [Fact]
        public void CreateContextNoHeader()
        {
            var ctx = factory.CreateContext(createEmptyRequest());
            Assert.NotNull(ctx);

            Guid guid;
            Assert.True(Guid.TryParse(ctx.CorrelationId, out guid));
            Assert.True(Guid.TryParse(ctx.RequestId, out guid));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("11111111-1111-1111-1111-111111111111", "22222222-2222-2222-2222-222222222222")]
        [InlineData(null, "22222222-2222-2222-2222-222222222222")]
        public void CreateContextFromHeader(string requestIdHeader, string requestIdWorker)
        {
            var correlationId = Guid.NewGuid().ToString();
            var ctx = factory.CreateContext(createRequest(correlationId, requestIdHeader, requestIdWorker));
            Assert.Equal(correlationId, ctx.CorrelationId);
            Assert.NotNull(ctx.RequestId);
            if (requestIdHeader != null)
                Assert.Equal(requestIdHeader, ctx.RequestId);
            else if (requestIdWorker != null)
                Assert.Equal(requestIdWorker, ctx.RequestId);
        }

        private IDictionary<string, object> createEmptyRequest()
        {
            return new Dictionary<string, object>();
        }

        private IDictionary<string, object> createRequest(string correlationId, string requestIdHeader, string requestIdWorker)
        {
            var headers = new Dictionary<string, string[]>
            {
                {CorrelationHeaderInfo.CorrelationIdHeaderName, new[] {correlationId}}
            };
            if (requestIdHeader != null)
                headers.Add(CorrelationHeaderInfo.RequestIdHeaderName, new [] {requestIdHeader});

            var environment = new Dictionary<string, object>
            {
                {"owin.RequestHeaders", headers}
            };

            if (requestIdWorker != null)
                environment.Add("owin.RequestId", requestIdWorker);

            return environment;
        }
    }
}