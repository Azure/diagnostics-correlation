using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Common.Http;
using Moq;
using Moq.Protected;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Test.Http
{
    public class ContextRequestHandlerTests
    {
        [Fact]
        public void CtorInjectorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ContextRequestHandler<string>(null));
        }

        [Fact]
        public async Task SuccessFlow()
        {
            var correlationId = Guid.NewGuid().ToString();
            ContextResolver.SetContext(new CorrelationContext(correlationId));

            var innerHandler = setupMockHandler(createSuccessResponse);
            var corrHandler = new ContextRequestHandler<CorrelationContext>(new [] {new CorrelationContextInjector()}, innerHandler.Object);
            var client = new HttpClient(corrHandler);

            await client.GetAsync("http://bing.com");
        }

        private Task<HttpResponseMessage> createSuccessResponse()
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }

        private static void validateRequest(HttpRequestMessage request)
        {
            var ctx = ContextResolver.GetContext<CorrelationContext>();
            IEnumerable<string> correlationIdHeader;
            Assert.True(request.Headers.TryGetValues(CorrelationHeaderInfo.CorrelationIdHeaderName, out correlationIdHeader));
            Assert.Equal(1, correlationIdHeader.Count());
            Assert.Equal(ctx.CorrelationId, correlationIdHeader.First());

            IEnumerable<string> requestIdHeader;
            Assert.True(request.Headers.TryGetValues(CorrelationHeaderInfo.RequestIdHeaderName, out requestIdHeader));
            Assert.Equal(1, requestIdHeader.Count());
            Assert.Equal(request.GetChildRequestId(), requestIdHeader.First());

        }

        private static Mock<HttpMessageHandler> setupMockHandler(Func<Task<HttpResponseMessage>> response)
        {
            var innerHandler = new Mock<HttpMessageHandler>();
            innerHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(response)
                .Callback<HttpRequestMessage, CancellationToken>((r, c) => { validateRequest(r); });
            return innerHandler;
        }
    }
}
