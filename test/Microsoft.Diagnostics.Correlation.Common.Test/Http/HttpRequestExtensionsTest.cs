using System;
using System.Net.Http;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Common.Http;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Test.Http
{
    public class HttpRequestExtensionsTest
    {
        [Fact]
        public void HeaderExists()
        {
            var request = new HttpRequestMessage();

            var requestId = Guid.NewGuid().ToString();
            request.Headers.Add(CorrelationHeaderInfo.RequestIdHeaderName, requestId);
            Assert.Equal(requestId, request.GetChildRequestId());
        }

        [Fact]
        public void HeaderDoesNotExists()
        {
            var request = new HttpRequestMessage();
            Assert.Null(request.GetChildRequestId());
        }
    }
}
