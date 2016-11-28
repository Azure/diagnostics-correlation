using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Common.Http;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Test.Http
{
    public class CorrelationContextInjectorTests
    {
        private readonly CorrelationContextInjector injector;

        public CorrelationContextInjectorTests()
        {
            injector = new CorrelationContextInjector();
        }

        [Fact]
        public void RequestNull()
        {
            Assert.Throws<ArgumentNullException>(() => injector.UpdateRequest(new CorrelationContext(""), null));
        }

        [Fact]
        public void CorrelationIdNull()
        {
            Assert.Throws<ArgumentNullException>(() => injector.UpdateRequest(null, new HttpRequestMessage()));
        }

        [Fact]
        public void ValidUpdateRequest()
        {
            var request = new HttpRequestMessage();
            var correlationId = Guid.NewGuid().ToString();
            injector.UpdateRequest(new CorrelationContext(correlationId), request);

            IEnumerable<string> correlationIdHeader;
            Assert.True(request.Headers.TryGetValues(CorrelationHeaderInfo.CorrelationIdHeaderName, out correlationIdHeader));
            Assert.Equal(1, correlationIdHeader.Count());
            Assert.Equal(correlationId, correlationIdHeader.First());

            IEnumerable<string> requestIdHeader;
            Assert.True(request.Headers.TryGetValues(CorrelationHeaderInfo.RequestIdHeaderName, out requestIdHeader));
            Assert.Equal(1, requestIdHeader.Count());
            Assert.Equal(request.GetChildRequestId(), requestIdHeader.First());

        }

        [Fact]
        public void HeaderAlreadyExists()
        {
            var correlationId = Guid.NewGuid().ToString();
            var request = new HttpRequestMessage();
            request.Headers.Add(CorrelationHeaderInfo.CorrelationIdHeaderName, correlationId);

            Assert.Throws<ArgumentException>(() => injector.UpdateRequest(new CorrelationContext(correlationId), request));
        }
    }
}
