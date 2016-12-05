#if NETCOREAPP1_0
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Common.Http;
using Microsoft.Diagnostics.Correlation.Common.Instrumentation;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Test
{
    public class ContextTracingInstumentationListenerTests
    {
        [Fact]
        public void ConfigurationNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => ContextTracingInstrumentation.Enable((AspNetCoreCorrelationConfiguration) null));
        }

        [Fact]
        public void IConfigurationNull()
        {
            Assert.Throws<ArgumentNullException>(() => ContextTracingInstrumentation.Enable((IConfiguration) null));
        }

        [Fact]
        public void ContextFactoryNull()
        {
            var config = new AspNetCoreCorrelationConfiguration()
                .WithContextFactory(null);

            Assert.Throws<ArgumentNullException>(() => ContextTracingInstrumentation.Enable(config));
        }

        [Fact]
        public void ContextInjectorNull()
        {
            var config = new AspNetCoreConfiguration<CorrelationContext>();

            Assert.Throws<ArgumentNullException>(() => ContextTracingInstrumentation.Enable(config));
        }

        [Fact]
        public async Task SuccessFlowCustomInjector()
        {
            var injector = new InjectorMock();

            var config = new AspNetCoreCorrelationConfiguration()
                .WithContextInjectors(new[] {injector});

            ContextTracingInstrumentation.Enable(config);

            var correlationId = Guid.NewGuid().ToString();
            ContextResolver.SetContext(new CorrelationContext(correlationId));

            var client = new HttpClient();
            await client.GetAsync("http://bing.com");
            Assert.True(injector.WasCalled);
        }

        [Fact]
        public async Task SuccessFlowCustomInjectorBlockedEndpoint()
        {
            var injector = new InjectorMock();
            var validator = new EndpointFilter();
            validator.AddEndpoint("google.com");

            var config = new AspNetCoreCorrelationConfiguration()
                .WithEndpointValidator(validator);

            ContextTracingInstrumentation.Enable(config);

            var correlationId = Guid.NewGuid().ToString();
            ContextResolver.SetContext(new CorrelationContext(correlationId));

            var client = new HttpClient();
            await client.GetAsync("http://google.com");
            Assert.False(injector.WasCalled);
        }

        [Fact]
        public async Task SuccessFlowNotifier()
        {
            var notifier = new RequestNotifier();
            var config = new AspNetCoreCorrelationConfiguration().WithOutgoingRequestNotifier(notifier);
            ContextTracingInstrumentation.Enable(config);

            var correlationId = Guid.NewGuid().ToString();
            ContextResolver.SetContext(new CorrelationContext(correlationId));

            var client = new HttpClient();
            await client.GetAsync("http://google.com");
            Assert.True(notifier.BeforeWasCalled);
            Assert.True(notifier.AfterWasCalled);
        }

        private class InjectorMock : IContextInjector<CorrelationContext, HttpRequestMessage>
        {
            public bool WasCalled { get; private set; }

            public void UpdateRequest(CorrelationContext ctx, HttpRequestMessage request)
            {
                WasCalled = true;
            }
        }

        private class RequestNotifier :
            IOutgoingRequestNotifier<CorrelationContext, HttpRequestMessage, HttpResponseMessage>
        {
            public bool BeforeWasCalled { get; private set; }
            public bool AfterWasCalled { get; private set; }

            public void OnBeforeRequest(CorrelationContext context, HttpRequestMessage request)
            {
                BeforeWasCalled = true;
                Assert.NotNull(context.ChildRequestId);
                Assert.NotNull(request);
            }

            public void OnAfterResponse(CorrelationContext context, HttpResponseMessage response)
            {
                AfterWasCalled = true;
                Assert.NotNull(context.ChildRequestId);
                Assert.NotNull(response);
            }
        }
    }
}
#endif