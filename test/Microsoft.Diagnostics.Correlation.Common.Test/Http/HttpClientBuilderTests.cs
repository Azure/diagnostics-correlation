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
    public class HttpClientBuilderTests
    {
        [Fact]
        public async Task SuccessFlow()
        {
            var correlationId = Guid.NewGuid().ToString();
            ContextResolver.SetContext(new CorrelationContext(correlationId));

            var innerHandler = setupMockHandler(validateHeader);
            var client = HttpClientBuilder.CreateClient(innerHandler.Object, new[] {new CorrelationContextInjector()});
            await client.GetAsync("http://bing.com");
        }

        [Fact]
        public async Task SuccessFlowCustomHandlers()
        {
            var correlationId = Guid.NewGuid().ToString();
            ContextResolver.SetContext(new CorrelationContext(correlationId));

            var innerHandler = setupMockHandler(validateHeader);
            var client = HttpClientBuilder.CreateClient(innerHandler.Object, new[] {new CorrelationContextInjector()},
                new TestHandler());
            await client.GetAsync("http://bing.com");
        }

        [Fact]
        public async Task NoInjectors()
        {
            var correlationId = Guid.NewGuid().ToString();
            ContextResolver.SetContext(correlationId);

            var innerHandler = setupMockHandler(validateNoHeader);
            var client = HttpClientBuilder.CreateClient(innerHandler.Object, new List<IContextInjector<string, HttpRequestMessage>>());

            await client.GetAsync("http://bing.com");
        }

        [Fact]
        public async Task ValidateMultipleInjectors()
        {
            var correlationId = Guid.NewGuid().ToString();
            ContextResolver.SetContext(new CorrelationContext(correlationId));

            var testInjector = new TestInjector();
            var innerHandler = setupMockHandler(r =>
            {
                validateHeader(r);
                testInjector.ValidateHeader(r);
            });

            var client = HttpClientBuilder.CreateClient(innerHandler.Object, new List<IContextInjector<CorrelationContext, HttpRequestMessage>> {new CorrelationContextInjector(), testInjector});

            await client.GetAsync("http://bing.com");
        }

        private static void validateHeader(HttpRequestMessage request)
        {
            IEnumerable<string> actualHeader;
            Assert.True(request.Headers.TryGetValues(CorrelationHeaderInfo.CorrelationIdHeaderName, out actualHeader));
            Assert.Equal(1, actualHeader.Count());
            Assert.Equal(ContextResolver.GetContext<CorrelationContext>().CorrelationId, actualHeader.First());
        }
        private static void validateNoHeader(HttpRequestMessage request)
        {
            IEnumerable<string> actualHeader;
            Assert.False(request.Headers.TryGetValues(CorrelationHeaderInfo.CorrelationIdHeaderName, out actualHeader));
        }

        private static Mock<HttpMessageHandler> setupMockHandler(Action<HttpRequestMessage> callback)
        {
            var innerHandler = new Mock<HttpMessageHandler>();
            innerHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Callback<HttpRequestMessage, CancellationToken>((r, c) => { callback(r); });
            return innerHandler;
        }

        private class TestInjector : IContextInjector<CorrelationContext, HttpRequestMessage>
        {
            private readonly string id;

            public TestInjector()
            {
                id = Guid.NewGuid().ToString();
            }

            public void UpdateRequest(CorrelationContext ctx, HttpRequestMessage request)
            {
                request.Headers.Add(id, id);
            }

            public void ValidateHeader(HttpRequestMessage request)
            {
                IEnumerable<string> actualHeader;
                Assert.True(request.Headers.TryGetValues(id, out actualHeader));
                Assert.Equal(1, actualHeader.Count());
                Assert.Equal(id, actualHeader.First());
            }
        }

        private class TestHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                validateHeader(request);
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
