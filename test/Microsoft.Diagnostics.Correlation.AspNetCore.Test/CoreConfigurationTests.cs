using System;
using System.IO;
using System.Linq;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Common.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

        [Fact]
        public void IConfigurationOutRequests()
        {
            var configObj = new {InstrumentOutgoingRequests = false};
            using (var configFile = new TemporaryFile())
            {
                configFile.Write(JsonConvert.SerializeObject(configObj));

                var config = new AspNetCoreCorrelationConfiguration(build(configFile.FilePath));
                Assert.NotNull(config.ContextFactory);
                Assert.NotNull(config.ContextInjectors);
                Assert.NotNull(config.EndpointFilter);
                Assert.False(config.InstrumentOutgoingRequests);

                Assert.True(config.ContextInjectors.First() is CorrelationContextInjector);
            }
        }

        [Fact]
        public void IConfigurationInvalidOutRequests()
        {
            var configObj = new { InstrumentOutgoingRequests = 123 };

            using (var configFile = new TemporaryFile())
            {
                configFile.Write(JsonConvert.SerializeObject(configObj));

                Assert.Throws<InvalidOperationException>(() => new AspNetCoreCorrelationConfiguration(build(configFile.FilePath)));
            }
        }

        [Fact]
        public void IConfigurationValidHeaders()
        {
            var configObj = new {Headers = new {CorrelationIdHeaderName = "correlation", RequestIdHeaderName = "request"}};
            using (var configFile = new TemporaryFile())
            {
                configFile.Write(JsonConvert.SerializeObject(configObj));

                var config = new AspNetCoreCorrelationConfiguration(build(configFile.FilePath));
                Assert.True(config.InstrumentOutgoingRequests);
                Assert.Equal("correlation", CorrelationHeaderInfo.CorrelationIdHeaderName);
                Assert.Equal("request", CorrelationHeaderInfo.RequestIdHeaderName);
            }
        }

        [Theory]
        [InlineData(null, "request")]
        [InlineData("correlation", null)]
        public void IConfigurationInvalidHeaders(string corrHeader, string requestHeader)
        {
            var configObj = new
            {
                Headers = corrHeader == null && requestHeader == null
                    ? null
                    : new
                    {
                        CorrelationIdHeaderName = corrHeader,
                        RequestIdHeaderName = requestHeader
                    }
            };
            using (var configFile = new TemporaryFile())
            {
                configFile.Write(JsonConvert.SerializeObject(configObj, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));

                Assert.Throws<ArgumentException>(() => new AspNetCoreCorrelationConfiguration(build(configFile.FilePath)));
            }
        }

        [Fact]
        public void IConfigurationValidEndpointFilter()
        {
            var configObj = new {EndpointFilter = new {Allow = true, Endpoints = new [] { "1.com", "2.com" } }};
            using (var configFile = new TemporaryFile())
            {
                configFile.Write(JsonConvert.SerializeObject(configObj));

                var config = new AspNetCoreCorrelationConfiguration(build(configFile.FilePath));
                Assert.False(config.EndpointFilter.Validate(new Uri("http://3.com")));
                Assert.True(config.EndpointFilter.Validate(new Uri("http://2.com")));
            }
        }

        [Fact]
        public void IConfigurationNoEndpoints()
        {
            var configObj = new { EndpointFilter = new { Allow = true }};
            using (var configFile = new TemporaryFile())
            {
                configFile.Write(JsonConvert.SerializeObject(configObj));

                Assert.Throws<ArgumentException>( () => new AspNetCoreCorrelationConfiguration(build(configFile.FilePath)));
            }
        }

        private IConfiguration build(string fileName)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(fileName);
            return configBuilder.Build();
        }

        public class TemporaryFile : IDisposable
        {
            public TemporaryFile() : this(Path.GetTempPath())
            { }

            public TemporaryFile(string directory)
            {
                Create(Path.Combine(directory, Path.GetRandomFileName()));
            }

            public void Dispose()
            {
                Delete();
            }

            public void Write(string contents)
            {
                if (FilePath == null)
                {
                    throw new ObjectDisposedException(nameof(TemporaryFile));
                }

                if (string.IsNullOrEmpty(contents))
                {
                    return;
                }

                File.AppendAllText(FilePath, contents);
            }

            public void Clear()
            {
                if (FilePath == null)
                {
                    throw new ObjectDisposedException(nameof(TemporaryFile));
                }

                File.WriteAllText(FilePath, string.Empty);
            }

            public string FilePath { get; private set; }

            private void Create(string path)
            {
                FilePath = path;
                using (File.Create(FilePath)) { };
            }

            private void Delete()
            {
                if (FilePath == null) return;
                File.Delete(FilePath);
                FilePath = null;
            }
        }

    }
}
