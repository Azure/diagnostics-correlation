// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.AspNetCore.Internal;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Common.Instrumentation;
using Microsoft.Diagnostics.Correlation.Common.Http;

namespace Microsoft.Diagnostics.Correlation.AspNetCore
{
    /// <summary>
    /// Provides generic instrumentation configuration for ASP.NET Core apps: set of injectors and other instrumetnation parameters
    /// </summary>
    /// <typeparam name="TContext">Type of correlation context</typeparam>
    public class AspNetCoreConfiguration<TContext> : Configuration<TContext, HttpRequestMessage, HttpResponseMessage> where TContext : ICorrelationContext<TContext>
    {
        /// <summary>
        /// Constructs default <see cref="AspNetCoreConfiguration{TContext}"/> with enabled outgoing requests instrumentation and default <see cref="EndpointFilter"/>
        /// </summary>
        public AspNetCoreConfiguration()
        {
            InstrumentOutgoingRequests = true;
            EndpointFilter = new EndpointFilter();
        }

        /// <summary>
        /// Constructs <see cref="AspNetCoreConfiguration{TContext}"/>
        /// </summary>
        /// <param name="configuration">Correlation configuration</param>
        public AspNetCoreConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var settings = new CorrelationConfigurationSettings();
            configuration.Bind(settings);

            InstrumentOutgoingRequests = settings.InstrumentOutgoingRequests ?? true;
            if (settings.Headers != null)
            {
                if (settings.Headers.CorrelationIdHeaderName != null && settings.Headers.RequestIdHeaderName != null)
                {
                    CorrelationHeaderInfo.CorrelationIdHeaderName = settings.Headers.CorrelationIdHeaderName;
                    CorrelationHeaderInfo.RequestIdHeaderName = settings.Headers.RequestIdHeaderName;
                }
                else
                {
                    throw new ArgumentException($"\"Headers\" section must define \"{nameof(CorrelationHeaderInfo.CorrelationIdHeaderName)}\" and \"{CorrelationHeaderInfo.RequestIdHeaderName}\"");
                }
            }

            if (settings.EndpointFilter != null)
            {
                if (settings.EndpointFilter.Endpoints != null)
                {
                    EndpointFilter = new EndpointFilter(settings.EndpointFilter.Endpoints, settings.EndpointFilter.Allow);
                }
                else
                {
                    throw new ArgumentException($"\"EndpointFilter\" section must define \"{nameof(settings.EndpointFilter.Endpoints)}\" list");
                }
            }
            else
            {
                EndpointFilter = new EndpointFilter();
            }
        }

        /// <summary>
        /// Controls if outgoing requests should be instumented with <see cref="System.Diagnostics.DiagnosticListener"/>
        /// </summary>
        public bool InstrumentOutgoingRequests { get; set; }

        /// <summary>
        /// Sets <see cref="IContextFactory{TContext,TRequest}"/> to control how context is extracted from the incoming request
        /// </summary>
        public IContextFactory<TContext, HttpRequest> ContextFactory { get; set; }

        /// <summary>
        /// Sets <see cref="IContextFactory{TContext,TRequest}"/> to control how context is extracted from the incoming request
        /// </summary>
        /// <param name="contextFactory"><see cref="IContextFactory{TContext,TRequest}"/> instance</param>
        /// <returns>Current <see cref="Configuration{TContext,TOutRequest,TOutResponse}"/> for chaining</returns>
        public Configuration<TContext, HttpRequestMessage, HttpResponseMessage> WithContextFactory(
            IContextFactory<TContext, HttpRequest> contextFactory)
        {
            ContextFactory = contextFactory;
            return this;
        }

        /// <summary>
        /// Controls if incoming requests should be instumented with <see cref="System.Diagnostics.DiagnosticListener"/>
        /// </summary>
        /// <returns>Current <see cref="Configuration{TContext,TOutRequest,TOutResponse}"/> for chaining</returns>
        public Configuration<TContext, HttpRequestMessage, HttpResponseMessage> DiableOutgoingRequestInstrumentation()
        {
            InstrumentOutgoingRequests = false;
            return this;
        }
    }
}
