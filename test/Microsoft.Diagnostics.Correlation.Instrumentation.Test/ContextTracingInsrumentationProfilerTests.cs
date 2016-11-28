using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Common.Instrumentation;
using Microsoft.Diagnostics.Correlation.Http;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Instrumentation.Tests
{
    public class ProfilerFixture : IDisposable
    {
        public readonly InjectorMock Injector;
        public readonly RequestNotifier Notifier;
        public ProfilerFixture()
        {
            Injector = new InjectorMock();
            Notifier = new RequestNotifier();

            var profilerConfig = new ProfilerCorrelationConfiguration()
                .WithContextInjectors(new[] {Injector})
                .WithOutgoingRequestNotifier(Notifier);

            ContextTracingInstrumentation.Enable(profilerConfig);
        }

        public void Dispose()
        {
        }
    }

    public class ContextTracingInsrumentationProfilerTests : IClassFixture<ProfilerFixture>, IDisposable
    {
        private readonly ProfilerFixture fixture;

        public ContextTracingInsrumentationProfilerTests(ProfilerFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact(Skip = "profiler is not configured in VSO Build")]
        public void ContextInjectorNull()
        {
            var profilerConfig = new Configuration<CorrelationContext,WebRequest,WebResponse>();

            Assert.Throws<ArgumentNullException>(() => ContextTracingInstrumentation.Enable(profilerConfig));
        }

        [Fact(Skip = "profiler is not configured in VSO Build")]
        public async Task SuccessFlowHttpClient()
        {
            fixture.Notifier.BeforeWasCalled = false;
            fixture.Notifier.AfterWasCalled = false;
            fixture.Injector.WasCalled = false;

            var correlationId = Guid.NewGuid().ToString();
            ContextResolver.SetContext(new CorrelationContext(correlationId));

            var client = new HttpClient();
            await client.GetAsync("http://bing.com");
            Assert.True(fixture.Injector.WasCalled);
            Assert.True(fixture.Notifier.BeforeWasCalled);
            Assert.True(fixture.Notifier.AfterWasCalled);
        }

        [Fact(Skip = "profiler is not configured in VSO Build")]
        public async Task SuccessFlowWebRequest()
        {
            fixture.Notifier.BeforeWasCalled = false;
            fixture.Notifier.AfterWasCalled = false;
            fixture.Injector.WasCalled = false;

            var correlationId = Guid.NewGuid().ToString();
            ContextResolver.SetContext(new CorrelationContext(correlationId));

            var request = WebRequest.CreateHttp("http://bing.com");
            try
            {
                await request.GetResponseAsync();
            }
            catch
            {
                // ignored
            }

            Assert.True(fixture.Injector.WasCalled);
            Assert.True(fixture.Notifier.BeforeWasCalled);
            Assert.True(fixture.Notifier.AfterWasCalled);
        }

        public void Dispose()
        {
        }
    }

    public class RequestNotifier : IOutgoingRequestNotifier<CorrelationContext, WebRequest, WebResponse>
    {
        public bool BeforeWasCalled { get; set; }
        public bool AfterWasCalled { get; set; }

        public void OnBeforeRequest(CorrelationContext context, WebRequest request)
        {
            BeforeWasCalled = true;
            Assert.NotNull(context.ChildRequestId);
            Assert.NotNull(request);
        }

        public void OnAfterResponse(CorrelationContext context, WebResponse response)
        {
            AfterWasCalled = true;
            Assert.NotNull(context.ChildRequestId);
            Assert.NotNull(response);
        }
    }

    public class InjectorMock : IContextInjector<CorrelationContext, WebRequest>
    {
        private readonly WebRequestCorrelationContextInjector injector = new WebRequestCorrelationContextInjector();
        public bool WasCalled { get; set; }

        public void UpdateRequest(CorrelationContext ctx, WebRequest request)
        {
            WasCalled = true;
            injector.UpdateRequest(ctx, request);
        }
    }

}