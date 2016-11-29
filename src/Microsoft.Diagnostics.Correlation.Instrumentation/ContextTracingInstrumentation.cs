// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.Diagnostics.Instrumentation.Extensions.Intercept;
using System.Net;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common.Instrumentation;
using Microsoft.Diagnostics.Correlation.Http;

namespace Microsoft.Diagnostics.Correlation.Instrumentation
{
    /// <summary>
    /// Implements outgoing HTTP requests instrumentation with profiler
    /// </summary>
    public class ContextTracingInstrumentation
    {
        /// <summary>
         /// Enables outgoing request instrumentation: injects <see cref="CorrelationContext"/> in outgoing HttpClient call or WebRequest
        /// </summary>
        /// <param name="configuration"><see cref="ProfilerCorrelationConfiguration"/> instance</param>
        public static void Enable(ProfilerCorrelationConfiguration configuration)
        {
            Enable<CorrelationContext>(configuration);
        }

        /// <summary>
        /// Enables outgoing request instrumentation: inject generic correlation context in outgoing <see cref="System.Net.Http.HttpClient"/> or <see cref="System.Net.WebRequest"/> call
        /// </summary>
        /// <typeparam name="TContext">Type of correlation context</typeparam>
        /// <param name="configuration">Collection of <see cref="Common.IContextInjector{TContext,TRequest}"/> to inject context into outgoing request</param>
        public static void Enable<TContext>(Configuration<TContext, WebRequest, WebResponse> configuration) where TContext : ICorrelationContext<TContext>
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (configuration.ContextInjectors == null)
                throw new ArgumentNullException(nameof(configuration.ContextInjectors));

            if (!Decorator.IsHostEnabled())
            {
                throw new InvalidOperationException("Profiler is not available on the host");
            }

            var extesionBaseDirectory = string.IsNullOrWhiteSpace(AppDomain.CurrentDomain.RelativeSearchPath)
                ? AppDomain.CurrentDomain.BaseDirectory
                : AppDomain.CurrentDomain.RelativeSearchPath;
                Decorator.InitializeExtension(extesionBaseDirectory);

            Decorator.Decorate(
                "System",
                "System.dll",
                "System.Net.HttpWebRequest.BeginGetResponse",
                2,
                (thisObj, callback, state) => OnBeginGetResponse(thisObj, configuration),
                null,
                null);

            Decorator.Decorate(
                "System",
                "System.dll",
                "System.Net.HttpWebRequest.EndGetResponse",
                1,
                null,
                (context, returnValue, thisObj, asyncResult) => OnEndGetResponse(context, returnValue, thisObj, asyncResult, configuration),
                null);

            Decorator.Decorate(
                "System",
                "System.dll",
                "System.Net.HttpWebRequest.GetResponse",
                0,
                thisObj => OnBeginGetResponse(thisObj, configuration),
                (context, returnValue, thisObj) => OnEndGetResponse(context, returnValue, thisObj, null, configuration),
                null);
        }

        private static object OnBeginGetResponse<TContext>(object requestObj, Configuration<TContext, WebRequest, WebResponse> config) where TContext : ICorrelationContext<TContext>
        {
            var request = requestObj as HttpWebRequest;

            if (request != null)
            {
                if (config.EndpointFilter.Validate(request.RequestUri))
                {
                    var ctx = ContextResolver.GetContext<TContext>();
                    if (ctx != null)
                    {
                        foreach (var injector in config.ContextInjectors)
                        {
                            injector.UpdateRequest(ctx, request);
                        }

                        try
                        {
                            config.RequestNotifier?.OnBeforeRequest(ctx.GetChildRequestContext(request.GetChildRequestId()), request);
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
                }
            }

            return requestObj;
        }

        private static object OnEndGetResponse<TContext>(object context, object returnValue, object thisObj, object asyncResult, Configuration<TContext, WebRequest, WebResponse> config) where TContext : ICorrelationContext<TContext>
        {
            var response = returnValue as WebResponse;
            var request = thisObj as WebRequest;
            if (request != null && response != null)
            {
                if (config.EndpointFilter.Validate(request.RequestUri))
                {

                    var ctx = ContextResolver.GetContext<TContext>();
                    try
                    {
                        config.RequestNotifier?.OnAfterResponse(
                            ctx.GetChildRequestContext(request.GetChildRequestId()), response);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }
            }
            return returnValue;
        }
    }
}
