using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Common.Http;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Test
{
    public class CorrelationContextTests
    {
        [Fact]
        public void CorrelationIdNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CorrelationContext(null));
        }

        [Fact]
        public void CorrelationIdValid()
        {
            var tId = Guid.NewGuid().ToString();
            var ctx = new CorrelationContext(tId);
            Assert.Equal(tId, ctx.CorrelationId);
            Assert.Null(ctx.RequestId);
            Assert.Equal(2, ctx.Count);
        }

        [Fact]
        public void RequestIdValid()
        {
            var tId = Guid.NewGuid().ToString();
            var rId = Guid.NewGuid().ToString();
            var ctx = new CorrelationContext(tId, rId);
            Assert.Equal(rId, ctx.RequestId);
        }

        [Fact]
        public void AddCustomField()
        {
            var tId = Guid.NewGuid().ToString();
            var ctx = new CorrelationContext(tId);
            Assert.Throws<ArgumentException>(() => ctx.Add("correlationId", "id"));
            Assert.Throws<ArgumentException>(() => ctx.Add("requestId", "id"));
            
            ctx.Add("custom", 123);
            Assert.Equal(123, ctx["custom"]);
            Assert.Equal(3, ctx.Count);
        }

        [Fact]
        public void KeysAndValues()
        {
            var tId = Guid.NewGuid().ToString();
            var ctx = new CorrelationContext(tId)
            {
                { "custom1", 1},
                { "custom2", 2}
            };

            Assert.Contains("custom1", ctx.Keys);
            Assert.Contains("custom2", ctx.Keys);
            Assert.Contains("correlationId", ctx.Keys);
            Assert.Contains("requestId", ctx.Keys);

            Assert.Contains(1, ctx.Values);
            Assert.Contains(2, ctx.Values);
            Assert.Contains(tId, ctx.Values);

            Assert.Contains(new KeyValuePair<string, object>("custom1", 1), ctx);
            Assert.Contains(new KeyValuePair<string, object>("custom2", 2), ctx);
            Assert.Contains(new KeyValuePair<string, object>("correlationId", tId), ctx);
        }

        public void ChildContext()
        {
            var ctx = new CorrelationContext(Guid.NewGuid().ToString());

            var request = new HttpRequestMessage();
            var childId = Guid.NewGuid().ToString();
            request.Headers.Add(CorrelationHeaderInfo.RequestIdHeaderName, childId);
            var childCtx = ctx.GetChildRequestContext(request.GetChildRequestId());
            Assert.NotEqual(childCtx, ctx);

            Assert.Null(ctx.ChildRequestId);
            Assert.Equal(childId, childCtx.ChildRequestId);
            Assert.Equal(3, childCtx.Count);

            ctx["some-id"] = "some-value";
            Assert.Equal(3, childCtx.Count);
        }

        public void ChildContextNull()
        {
            var ctx = new CorrelationContext(Guid.NewGuid().ToString());

            var request = new HttpRequestMessage();
            var childCtx = ctx.GetChildRequestContext(request.GetChildRequestId());
            Assert.Equal(2, childCtx.Count);
        }

    }
}
