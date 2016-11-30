// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.AspNetCore.Internal;
using Microsoft.Diagnostics.Correlation.Common.Http;
using Microsoft.Diagnostics.Correlation.Common.Instrumentation;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Diagnostics.Correlation.AspNetCore
{
    /// <summary>
    /// Implements outgoing HTTP requests instrumentation with <see cref="DiagnosticListener"/> 
    /// </summary>
    public class ContextTracingInstrumentation
    {
        private const string HttpListenerName = "HttpHandlerDiagnosticListener";
        private const string AspNetListenerName = "Microsoft.AspNetCore";

        /// <summary>
        /// Enables instrumentation of outgoing (and possibly incoming requests)
        /// Extracts <see cref="CorrelationContext"/> from incoming request and injects it to outgoing requests
        /// </summary>
        /// <param name="configuration">Instrumentation configuration</param>
        /// <returns><see cref="IDisposable"/> for the <see cref="DiagnosticListener"/> observers</returns>
        public static IDisposable Enable(IConfiguration configuration)
        {
            return Enable<CorrelationContext>(new AspNetCoreCorrelationConfiguration(configuration));
        }

        /// <summary>
        /// Enables instrumentation of outgoing (and possibly incoming requests)
        /// Extracts <see cref="CorrelationContext"/> from incoming request and injects it to outgoing requests
        /// </summary>
        /// <param name="configuration">Instrumentation configuration <see cref="AspNetCoreConfiguration{TContext}"/></param>
        /// <returns><see cref="IDisposable"/> for the <see cref="DiagnosticListener"/> observers</returns>
        public static IDisposable Enable(AspNetCoreCorrelationConfiguration configuration)
        {
            return Enable<CorrelationContext>(configuration);
        }

        /// <summary>
        /// Enables instrumentation of outgoing (and possibly incoming requests)
        /// Extracts generic correlation context from incoming request and injects it to outgoing requests
        /// </summary>
        /// <param name="configuration">Instrumentation <see cref="Configuration{TContext,TOutRequest,TOutResponse}"/></param>
        /// <returns><see cref="IDisposable"/> for the <see cref="DiagnosticListener"/> observers</returns>
        public static IDisposable Enable<TContext>(Configuration<TContext, HttpRequestMessage, HttpResponseMessage>  configuration) where TContext : ICorrelationContext<TContext>
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var coreConfig = configuration as AspNetCoreConfiguration<TContext>;
            if (coreConfig == null)
                throw new ArgumentException($"{nameof(configuration)} is not instance of AspNetCoreConfiguration");

            if (configuration.ContextInjectors == null)
                throw new ArgumentNullException(nameof(configuration.ContextInjectors));

            if (coreConfig.ContextFactory == null)
                throw new ArgumentNullException(nameof(coreConfig.ContextFactory));

            var observers = new Dictionary<string, IObserver<KeyValuePair<string, object>>>
            {
                {
                    AspNetListenerName, new AspNetDiagnosticListenerObserver<TContext>(coreConfig.ContextFactory)
                }
            };

            if (coreConfig.InstrumentOutgoingRequests)
            {
                observers.Add(
                    HttpListenerName, new HttpDiagnosticListenerObserver<TContext>(
                        configuration.ContextInjectors,
                        configuration.EndpointFilter,
                        new NotifierWrapper<TContext>(configuration.RequestNotifier)));
            }

            return DiagnosticListener.AllListeners.Subscribe(new DiagnosticListenersObserver(observers));
        }

        private class NotifierWrapper<TContext> : IOutgoingRequestNotifier<TContext, HttpRequestMessage, HttpResponseMessage> where TContext : ICorrelationContext<TContext>
        {
            private readonly IOutgoingRequestNotifier<TContext, HttpRequestMessage, HttpResponseMessage> requestNotifier;

            public NotifierWrapper(IOutgoingRequestNotifier<TContext, HttpRequestMessage, HttpResponseMessage> requestNotifier)
            {
                this.requestNotifier = requestNotifier;
            }

            public void OnBeforeRequest(TContext context, HttpRequestMessage request)
            {
                try
                {
                    requestNotifier?.OnBeforeRequest(context.GetChildRequestContext(request.GetChildRequestId()), request);
                }
                catch (Exception)
                {
                    //ignored
                }
            }

            public void OnAfterResponse(TContext context, HttpResponseMessage response)
            {
                try
                {
                    requestNotifier?.OnAfterResponse(context.GetChildRequestContext(response.RequestMessage.GetChildRequestId()), response);
                }
                catch (Exception)
                {
                    //ignored
                }

            }
        }
    }
}