using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Diagnostics.Correlation.Common;

namespace Microsoft.Diagnostics.Correlation.Test
{
    public class ContextResolverTests
    {
        [Fact]
        public void ContextResolverEmpty()
        {
            Assert.Null(ContextResolver.GetRequestContext<TestContext>());
        }

        [Fact]
        public void ContextResolverSetGet()
        {
            var ctx = new TestContext {CorrelationId = "1", OtherId = "2"};

            ContextResolver.SetRequestContext(ctx);
            var gotCtx = ContextResolver.GetRequestContext<TestContext>();

            TestContextHelper.AssertAreEqual(ctx, gotCtx);
        }

        [Fact]
        public async Task ContextResolverSetGetAsyncTask()
        {
            var ctx = new TestContext { CorrelationId = "1", OtherId = "2" };

            ContextResolver.SetRequestContext(ctx);

            var t = Task.Run(() => ContextResolver.GetRequestContext<TestContext>());
            await t.ConfigureAwait(false);
            TestContextHelper.AssertAreEqual(ctx, t.Result);
        }

        [Fact]
        public void ContextResolverSetOtherThread()
        {
            var ctx = new TestContext { CorrelationId = "1", OtherId = "2" };

            ContextResolver.SetRequestContext(ctx);

            TestContext gotCtx = null;
            var t = new Thread(() =>
            {
                gotCtx = ContextResolver.GetRequestContext<TestContext>();
            });
            t.Start();
            t.Join();

            TestContextHelper.AssertAreEqual(ctx, gotCtx);
        }

        [Fact]
        public void ContextResolverSetClear()
        {
            var ctx = new TestContext { CorrelationId = "1", OtherId = "2" };

            ContextResolver.SetRequestContext(ctx);
            ContextResolver.ClearContext();
            Assert.Null(ContextResolver.GetRequestContext<TestContext>());
        }

        [Fact]
        public void PutStringGetObject()
        {
            var guid = Guid.NewGuid().ToString();
            ContextResolver.SetRequestContext(guid);

            var got = ContextResolver.GetRequestContext<object>();
            Assert.NotNull(got);
            Assert.Equal(guid, got.ToString());
        }
    }

    public class TestContext
    {
        public string CorrelationId;
        public string OtherId;
    }

    public static class TestContextHelper
    {
        public static void AssertAreEqual(TestContext expected, TestContext actual)
        {
            if (expected == null && actual == null) return;

            Assert.NotNull(expected);
            Assert.NotNull(actual);
            Assert.Equal(expected.CorrelationId, actual.CorrelationId);
            Assert.Equal(expected.OtherId, actual.OtherId);
        }
    }
}
