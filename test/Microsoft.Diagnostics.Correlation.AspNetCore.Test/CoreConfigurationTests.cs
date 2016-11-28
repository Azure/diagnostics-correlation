using System.Linq;
using Microsoft.Diagnostics.Correlation.Common.Http;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Test
{
    public class CoreConfigurationTests
    {
        [Fact]
        public void DefaultConfiguration()
        {
            var config = new AspNetCoreCorrelationConfiguration();
            Assert.NotNull(config.ContextFactory);
            Assert.NotNull(config.ContextInjectors);
            Assert.True(config.InstrumentOutgoingRequests);

            Assert.True(config.ContextInjectors.First() is CorrelationContextInjector);
        }
    }
}
