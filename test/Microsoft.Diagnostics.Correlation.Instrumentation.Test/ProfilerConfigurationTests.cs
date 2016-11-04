using System.Linq;
using Microsoft.Diagnostics.Correlation.Http;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Instrumentation.Tests
{
    public class ProfilerConfigurationTests
    {
        [Fact]
        public void DefaultConfiguration()
        {
            var config = new ProfilerCorrelationConfiguration();
            Assert.NotNull(config.ContextInjectors);
            Assert.True(config.ContextInjectors.First() is WebRequestCorrelationContextInjector);
        }
    }
}
