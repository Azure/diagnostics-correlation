// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Diagnostics.Context;

namespace Microsoft.Diagnostics.Correlation.Common.Instrumentation
{
    /// <summary>
    /// Provides generic instrumentation configuration: set of factory, injectors and other instrumetnation parameters
    /// </summary>
    /// <typeparam name="TContext">Type of correlation context</typeparam>
    /// <typeparam name="TOutRequest">Type of ougoing request</typeparam>
    /// <typeparam name="TOutResponse">Type of response to outgoing request</typeparam>
    public class Configuration<TContext, TOutRequest, TOutResponse> where TContext : ICorrelationContext<TContext>
    {
        /// <summary>
        /// Optional <see cref="IEndpointFilter"/> to control which requests to instrument based on their Uri
        /// <see cref="EndpointFilter"/> is used by default
        /// </summary>
        public IEndpointFilter EndpointFilter { get; set; }

        /// <summary>
        /// Optional <see cref="IOutgoingRequestNotifier{TContext,TRequest,TResponse}"/> notifies about outgoing request events
        /// Could be used for logging
        /// </summary>
        public IOutgoingRequestNotifier<TContext, TOutRequest, TOutResponse> RequestNotifier { get; set; }

        private IEnumerable<IContextInjector<TContext, TOutRequest>> contextInjectors;

        /// <summary>
        /// Collection of <see cref="IContextInjector{TContext,TRequest}"/> to inject headers into outgoing request
        /// </summary>
        public IEnumerable<IContextInjector<TContext, TOutRequest>> ContextInjectors
        {
            get { return contextInjectors; }
            set { contextInjectors = new List<IContextInjector<TContext, TOutRequest>>(value); }
        }

        /// <summary>
        /// Sets <see cref="IEndpointFilter"/>
        /// </summary>
        /// <param name="endpointFilter"><see cref="IEndpointFilter"/> instance</param>
        /// <returns>Current <see cref="Configuration{TContext,TOutRequest,TOutResponse}"/> for chaining</returns>
        public Configuration<TContext, TOutRequest, TOutResponse> WithEndpointValidator(IEndpointFilter endpointFilter)
        {
            EndpointFilter = endpointFilter;
            return this;
        }

        /// <summary>
        /// Sets <see cref="IOutgoingRequestNotifier{TContext,TRequest,TResponse}"/>
        /// </summary>
        /// <param name="requestNotifier"><see cref="IOutgoingRequestNotifier{TContext,TRequest,TResponse}"/> instance</param>
        /// <returns>Current <see cref="Configuration{TContext,TOutRequest,TOutResponse}"/> for chaining</returns>
        public Configuration<TContext, TOutRequest, TOutResponse> WithOutgoingRequestNotifier(
            IOutgoingRequestNotifier<TContext, TOutRequest, TOutResponse> requestNotifier)
        {
            RequestNotifier = requestNotifier;
            return this;
        }

        /// <summary>
        /// Sets collection of <see cref="IContextInjector{TContext,TRequest}"/>
        /// </summary>
        /// <param name="injectors">collection of <see cref="IContextInjector{TContext,TRequest}"/></param>
        /// <returns>Current <see cref="Configuration{TContext,TOutRequest,TOutResponse}"/> for chaining</returns>
        public Configuration<TContext, TOutRequest, TOutResponse> WithContextInjectors(
            IEnumerable<IContextInjector<TContext, TOutRequest>> injectors)
        {
            contextInjectors = new List<IContextInjector<TContext, TOutRequest>>(injectors); //prevent list modification
            return this;
        }
    }
}
