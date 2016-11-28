// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if !NET40
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Diagnostics.Context;

namespace Microsoft.Diagnostics.Correlation.Common.Http
{
    /// <summary>
    /// Builds HttpClient with <see cref="CorrelationContextRequestHandler"/> in DelegaingHandler's pipeline
    /// </summary>
    public class CorrelationHttpClientBuilder : HttpClientBuilder
    {
        private static readonly List<IContextInjector<CorrelationContext, HttpRequestMessage>> Injectors = new List
            <IContextInjector<CorrelationContext, HttpRequestMessage>>
            {
                new CorrelationContextInjector()
            };

        /// <summary>
        /// Create HttpClient with single <see cref="CorrelationContextRequestHandler"/> and default <see cref="HttpMessageHandler"/> 
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateClient()
        {
            return CreateClient(Injectors);
        }

        /// <summary>
        ///  Create HttpClient with <see cref="CorrelationContextRequestHandler"/>, configured inner HTTP handler and custom DelegatingHandlers
        /// </summary>
        /// <param name="innerHandler">Instance of inner <see cref="HttpMessageHandler"/> </param>
        /// <param name="handlers">Custom handlers to be added to the <see cref="HttpClient"/> pipeline after <see cref="CorrelationContextRequestHandler"/></param>
        /// <returns><see cref="HttpClient"/> instance</returns>
        public static HttpClient CreateClient(HttpMessageHandler innerHandler,
            params DelegatingHandler[] handlers)
        {
            return CreateClient(innerHandler, Injectors, handlers);
        }

        /// <summary>
        ///  Create HttpClient with <see cref="CorrelationContextRequestHandler"/>, default <see cref="HttpMessageHandler"/> and collection of custom <see cref="DelegatingHandler"/>
        /// </summary>
        /// <param name="handlers">Custom handlers to be added to the <see cref="HttpClient"/> pipeline after <see cref="CorrelationContextRequestHandler"/></param>
        /// <returns><see cref="HttpClient"/> instance</returns>
        public static HttpClient CreateClient(params DelegatingHandler[] handlers)
        {
            return CreateClient(Injectors, handlers);
        }
    }
}
#endif