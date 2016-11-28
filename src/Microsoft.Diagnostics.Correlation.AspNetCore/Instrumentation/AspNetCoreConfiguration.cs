// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Common.Instrumentation;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Instrumentation
{
    /// <summary>
    /// Provides generic instrumentation configuration for ASP.NET Core apps: set of injectors and other instrumetnation parameters
    /// </summary>
    /// <typeparam name="TContext">Type of correlation context</typeparam>
    public class AspNetCoreConfiguration<TContext> : Configuration<TContext, HttpRequestMessage, HttpResponseMessage> where TContext : ICorrelationContext<TContext>
    {
        /// <summary>
        /// Controls if incoming requests should be instumented with <see cref="System.Diagnostics.DiagnosticListener"/>, <see cref="Microsoft.Diagnostics.Correlation.AspNetCore.Middleware.ContextTracingMiddleware{TContext}"/> and <see cref="Microsoft.Diagnostics.Correlation.AspNetCore.Middleware.CorrelationContextTracingMiddleware"/> for alternative incoming requests handling
        /// </summary>
        public bool InstrumentIncomingRequests { get; set; } = true;

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
        /// Controls if incoming requests should be instumented with <see cref="System.Diagnostics.DiagnosticListener"/>, <see cref="Microsoft.Diagnostics.Correlation.AspNetCore.Middleware.ContextTracingMiddleware{TContext}"/> and <see cref="Microsoft.Diagnostics.Correlation.AspNetCore.Middleware.CorrelationContextTracingMiddleware"/> for alternative incoming requests handling
        /// </summary>
        /// <returns>Current <see cref="Configuration{TContext,TOutRequest,TOutResponse}"/> for chaining</returns>
        public Configuration<TContext, HttpRequestMessage, HttpResponseMessage> DiableIncomingRequestInstrumentation()
        {
            InstrumentIncomingRequests = false;
            return this;
        }
    }
}
