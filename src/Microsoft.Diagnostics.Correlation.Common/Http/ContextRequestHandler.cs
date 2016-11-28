// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if !NET40
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Context;

namespace Microsoft.Diagnostics.Correlation.Common.Http
{
    /// <summary>
    /// Provides <see cref="DelegatingHandler"/> to inject generic correlation context onto the outgoing request
    /// </summary>
    /// <typeparam name="TContext">Type of correlation context</typeparam>
    public class ContextRequestHandler<TContext> : DelegatingHandler
    {
        private readonly IEnumerable<IContextInjector<TContext, HttpRequestMessage>> contextInjectors;

        /// <summary>
        /// ContextRequestHandler constructor
        /// </summary>
        /// <param name="contextInjectors">Collection of <see cref="IContextInjector{TContext,TRequest}"/> to be used for header injection</param>
        public ContextRequestHandler(IEnumerable<IContextInjector<TContext, HttpRequestMessage>> contextInjectors)
        {
            if (contextInjectors == null)
                throw new ArgumentNullException(nameof(contextInjectors));
            this.contextInjectors = new List<IContextInjector<TContext, HttpRequestMessage>>(contextInjectors); //prevent list modification
        }

        /// <summary>
        /// ContextRequestHandler constructor
        /// </summary>
        /// <param name="contextInjectors">Collection of <see cref="IContextInjector{TContext,TRequest}"/> to be used for header injection</param>
        /// <param name="innerHandler">Inner handler to be called after ContextRequestHandler</param>
        public ContextRequestHandler(IEnumerable<IContextInjector<TContext, HttpRequestMessage>> contextInjectors, 
            HttpMessageHandler innerHandler) : base(innerHandler)
        {
            if (contextInjectors == null)
                throw new ArgumentNullException(nameof(contextInjectors));
            this.contextInjectors = new List<IContextInjector<TContext, HttpRequestMessage>>(contextInjectors); //prevent list modification
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var ctx = ContextResolver.GetContext<TContext>();
            if (ctx != null)
            {
                foreach (var injector in contextInjectors)
                {
                    injector.UpdateRequest(ctx, request);
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
#endif