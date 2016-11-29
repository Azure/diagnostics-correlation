using System;
using Microsoft.Diagnostics.Correlation.Common.Http;
using Xunit;

namespace Microsoft.Diagnostics.Correlation.Test.Http
{
    public class EndpointValidatorTest
    {
        [Fact]
        public void EndpointValidatorEmpty()
        {
            var validator = new EndpointFilter(new string[0]);
            Assert.True(validator.Validate(new Uri("http://google.com")));
        }

        [Fact]
        public void EndpointValidatorDefault()
        {
            var validator = new EndpointFilter();
            Assert.True(validator.Validate(new Uri("http://google.com")));
            Assert.False(validator.Validate(new Uri("https://storagesample.blob.core.windows.net")));
        }

        [Fact]
        public void EndpointValidatorWhitelist()
        {
            var validator = new EndpointFilter(new [] {"bing", "microsoft", "visualstudio"} );
            Assert.False(validator.Validate(new Uri("http://google.com")));
            Assert.True(validator.Validate(new Uri("http://bing.com")));
            Assert.True(validator.Validate(new Uri("http://microsoft.com")));
            Assert.True(validator.Validate(new Uri("http://visualstudio.com")));
        }

        [Fact]
        public void EndpointValidatorBlacklist()
        {
            var validator = new EndpointFilter(new[] {"google"}, false);
            Assert.False(validator.Validate(new Uri("http://google.com")));
            Assert.True(validator.Validate(new Uri("http://bing.com")));
            Assert.True(validator.Validate(new Uri("http://microsoft.com")));
            Assert.True(validator.Validate(new Uri("http://visualstudio.com")));
        }


        [Fact]
        public void EndpointValidatorRegexp()
        {
            var validator = new EndpointFilter(new[] { "[a-z]+.com" });
            Assert.True(validator.Validate(new Uri("http://bing.com")));
            Assert.False(validator.Validate(new Uri("http://bing123.com")));
        }
    }
}

