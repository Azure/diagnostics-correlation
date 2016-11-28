// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if !NET40
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.Diagnostics.Correlation.Common.Http
{
    /// <summary>
    /// Provides HttpClient builder with <see cref="ContextRequestHandler{TContext}"/>
    /// </summary>
    public class HttpClientBuilder
    {
        //There is HttpClientFactory class in Microsoft.AspNet.WebApi.Client package
        //We implement it here to avoid bringing new dependencies
        protected static class HttpClientFactory
        {
            public static HttpMessageHandler CreatePipeline(HttpMessageHandler innerHandler, IEnumerable<DelegatingHandler> handlers)
            {
                HttpMessageHandler httpMessageHandler = innerHandler;
                foreach (var delegatingHandler in handlers.Reverse())
                {
                    delegatingHandler.InnerHandler = httpMessageHandler;
                    httpMessageHandler = delegatingHandler;
                }
                return httpMessageHandler;
            }
        }

        /// <summary>
        /// Creates HttpClient with <see cref="ContextRequestHandler{TContext}"/> and custom DelegatingHandlers
        /// </summary>
        /// <typeparam name="TContext">Type of correlation context</typeparam>
        /// <param name="contextInjectors">Collection of <see cref="IContextInjector{TContext,TRequest}"/> to be used for headers injection</param>
        /// <param name="handlers">Custom handlers to be added to the HttpClient pipeline after <see cref="ContextRequestHandler{TContext}"/></param>
        /// <returns>HttpClient instance</returns>
        public static HttpClient CreateClient<TContext>(IEnumerable<IContextInjector<TContext, HttpRequestMessage>> contextInjectors,
            params DelegatingHandler[] handlers)
        {
            HttpMessageHandler innerHandler = new HttpClientHandler();
            return CreateClient(innerHandler, contextInjectors, handlers);
        }

        /// <summary>
        /// Creates HttpClient with ContextRequestHandler, configured inner HTTP handler and custom DelegatingHandlers
        /// </summary>
        /// <typeparam name="TContext">Type of correlation context</typeparam>
        /// <param name="innerHandler">Instance of inner HttpMessageHandler</param>
        /// <param name="contextInjectors">Collection of <see cref="IContextInjector{TContext,TRequest}"/> to be used for headers injection</param>
        /// <param name="handlers">Custom handlers to be added to the HttpClient pipeline after <see cref="ContextRequestHandler{TContext}"/></param>
        /// <returns>HttpClient instance</returns>
        public static HttpClient CreateClient<TContext>(HttpMessageHandler innerHandler,
            IEnumerable<IContextInjector<TContext, HttpRequestMessage>> contextInjectors, 
            params  DelegatingHandler[] handlers)
        {
            if (innerHandler == null)
                throw new ArgumentNullException(nameof(innerHandler));

            var allHandlers = new List<DelegatingHandler> { new ContextRequestHandler<TContext>(contextInjectors) };
            if (handlers != null)
                allHandlers.AddRange(handlers);

            var pipeline = HttpClientFactory.CreatePipeline(innerHandler, allHandlers);
            return new HttpClient(pipeline);
        }
    }
}
#endif