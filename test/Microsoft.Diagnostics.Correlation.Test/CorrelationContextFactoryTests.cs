using System;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Http;
using Moq;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Test
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
            var mock = new Mock<HttpWorkerRequest>();
            Mock<RequestContext> requestContext = new Mock<RequestContext>();
            Mock<HttpContextBase> httpContextBase = new Mock<HttpContextBase>();
            httpContextBase.Setup(c => c.GetService(It.Is<Type>(v => v == typeof(HttpWorkerRequest)))).Returns(mock.Object);
            requestContext.SetupGet(rc => rc.HttpContext).Returns(httpContextBase.Object);
            var request = new HttpRequest("", "http://bing.com", "") {RequestContext = requestContext.Object};

            return request;
        }

        private HttpRequest createRequest(string correlationId, string requestIdHeader, string requestIdWorker)
        {
            var mock = new Mock<HttpWorkerRequest>();

            bool setupRequestIdHeader = requestIdHeader != null;
            var unknownHeaders = new string[setupRequestIdHeader ? 2 : 1][];
            unknownHeaders[0] = new string[2];
            unknownHeaders[0][0] = CorrelationHeaderInfo.CorrelationIdHeaderName;
            unknownHeaders[0][1] = correlationId;
            if (setupRequestIdHeader)
            {
                unknownHeaders[1] = new string[2];
                unknownHeaders[1][0] = CorrelationHeaderInfo.RequestIdHeaderName;
                unknownHeaders[1][1] = requestIdHeader;
            }

            mock.Setup(r => r.GetUnknownRequestHeaders()).Returns(unknownHeaders);
            if (requestIdWorker != null)
                mock.SetupGet(r => r.RequestTraceIdentifier).Returns(Guid.Parse(requestIdWorker));

            Mock<RequestContext> requestContext = new Mock<RequestContext>();
            Mock<HttpContextBase> httpContextBase = new Mock<HttpContextBase>();
            httpContextBase.Setup(c => c.GetService(It.Is<Type>(v => v == typeof(HttpWorkerRequest)))).Returns(mock.Object);
            requestContext.SetupGet(rc => rc.HttpContext).Returns(httpContextBase.Object);

            var paramTypes = new Type[] { typeof(HttpWorkerRequest), typeof(HttpContext) };
            var constructor = typeof(HttpRequest).GetConstructor(BindingFlags.NonPublic|BindingFlags.Instance, null, paramTypes, null);
            var paramValues = new object[] { mock.Object, null };
            var request = (HttpRequest)constructor.Invoke(paramValues);
            request.RequestContext = requestContext.Object;
            
            return request;
        }
    }
}
