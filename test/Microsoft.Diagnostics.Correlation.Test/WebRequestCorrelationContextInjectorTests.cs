using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Http;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Test
{
    public class WebRequestCorrelationContextInjectorTests
    {
        private readonly WebRequestCorrelationContextInjector injector;

        public WebRequestCorrelationContextInjectorTests()
        {
            injector = new WebRequestCorrelationContextInjector();
        }

        [Fact]
        public void RequestNull()
        {
            Assert.Throws<ArgumentNullException>(() => injector.UpdateRequest(new CorrelationContext(""), null));
        }

        [Fact]
        public void CorrelationIdNull()
        {
            Assert.Throws<ArgumentNullException>(() => injector.UpdateRequest(null, WebRequest.Create("http://bing.com")));
        }

        [Fact]
        public void ValidUpdateRequest()
        {
            var request = WebRequest.Create("http://bing.com");
            var correlationId = Guid.NewGuid().ToString();
            injector.UpdateRequest(new CorrelationContext(correlationId), request);

            IEnumerable<string> actualHeader = request.Headers.GetValues(CorrelationHeaderInfo.CorrelationIdHeaderName);
            Assert.NotNull(actualHeader);
            Assert.Equal(1, actualHeader.Count());
            Assert.Equal(correlationId, actualHeader.First());
        }

        [Fact]
        public void HeaderAlreadyExists()
        {
            var correlationId = Guid.NewGuid().ToString();
            var request = WebRequest.Create("http://bing.com");
            request.Headers.Add(CorrelationHeaderInfo.CorrelationIdHeaderName, correlationId);

            Assert.Throws<ArgumentException>(() => injector.UpdateRequest(new CorrelationContext(correlationId), request));
        }
    }
}