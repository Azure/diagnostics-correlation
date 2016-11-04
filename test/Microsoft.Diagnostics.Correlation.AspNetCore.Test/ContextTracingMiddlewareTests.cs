using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Diagnostics.Correlation.AspNetCore.Middleware;
using Microsoft.Diagnostics.Correlation.Common;
using Moq;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Test
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
            var middleware = new ContextTracingMiddleware<CorrelationContext>(assertEmptyContext, new CorrelationContextFactory());
            await middleware.Invoke(createEmptyRequest());
        }

        [Fact]
        public async Task FromHeader()
        {
            var middleware = new ContextTracingMiddleware<CorrelationContext>(AssertContext, new CorrelationContextFactory());
            await middleware.Invoke(createRequest(correlationId));
        }

        private Task assertEmptyContext(HttpContext context)
        {
            var ctx = ContextResolver.GetRequestContext<CorrelationContext>();
            Assert.NotNull(ctx);
            return Task.FromResult(1);
        }

        private Task AssertContext(HttpContext context)
        {
            var ctx = ContextResolver.GetRequestContext<CorrelationContext>();
            Assert.NotNull(ctx);
            Assert.Equal(correlationId, ctx.CorrelationId);
            return Task.FromResult(1);
        }

        private HttpContext createEmptyRequest()
        {
            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            request.SetupGet(r => r.Headers).Returns(new HeaderDictionary());
            context.SetupGet(c => c.Request).Returns(request.Object);
            request.SetupGet(r => r.HttpContext).Returns(context.Object);
            return context.Object;
        }

        private HttpContext createRequest(string correlationId)
        {
            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var headers = new HeaderDictionary{{CorrelationHeaderInfo.CorrelationIdHeaderName, new[] {correlationId}}};
            request.SetupGet(r => r.Headers).Returns(headers);

            context.SetupGet(c => c.Request).Returns(request.Object);
            request.SetupGet(r => r.HttpContext).Returns(context.Object);

            return context.Object;
        }
    }
}