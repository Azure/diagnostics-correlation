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
    /// Provides <see cref="DelegatingHandler"/> to inject to <see cref="CorrelationContext"/> to <see cref="HttpRequestMessage"/> 
    /// </summary>
    public class CorrelationContextRequestHandler : ContextRequestHandler<CorrelationContext>
    {
        /// <summary>
        /// CorrelationContextRequestHandler constructor
        /// </summary>
        /// <param name="contextInjectors">Collection of <see cref="IContextInjector{TContext,TRequest}"/> to be used for header injection</param>
        public CorrelationContextRequestHandler(IEnumerable<IContextInjector<CorrelationContext, HttpRequestMessage>> contextInjectors) :
            base(contextInjectors)
        {
        }

        /// <summary>
        /// CorrelationContextRequestHandler constructor
        /// </summary>
        /// <param name="contextInjectors">Collection of <see cref="IContextInjector{TContext,TRequest}"/> to be used for header injection</param>
        /// <param name="innerHandler">Inner handler to be called after CorrelationContextRequestHandler</param>
        public CorrelationContextRequestHandler(IEnumerable<IContextInjector<CorrelationContext, HttpRequestMessage>> contextInjectors, HttpMessageHandler innerHandler) : 
            base(contextInjectors, innerHandler)
        {
        }
    }
}
#endif