using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Diagnostics.Correlation.Common;
using Moq;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Test
{
    public class CorrelationContextFactoryTests
    {
        private readonly CorrelationContextFactory factory;

        public CorrelationContextFactoryTests()
        {
            factory = new CorrelationContextFactory();
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

        private HttpRequest createEmptyRequest()
        {
            var request = new Mock<HttpRequest>();
            var context = new Mock<HttpContext>();
            request.SetupGet(r => r.Headers).Returns(new HeaderDictionary());
            request.SetupGet(r => r.HttpContext).Returns(context.Object);
            return request.Object;
        }

        private HttpRequest createRequest(string correlationId, string requestIdHeader, string requestIdTrace)
        {
            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var headers = new HeaderDictionary { { CorrelationHeaderInfo.CorrelationIdHeaderName, new[] { correlationId } } };
            if (requestIdHeader != null)
                headers.Add(CorrelationHeaderInfo.RequestIdHeaderName, requestIdHeader);

            request.SetupGet(r => r.Headers).Returns(headers);

            if (requestIdTrace != null)
                context.SetupGet(r => r.TraceIdentifier).Returns(requestIdTrace);

            request.SetupGet(r => r.HttpContext).Returns(context.Object);

            return request.Object;
        }
    }
}