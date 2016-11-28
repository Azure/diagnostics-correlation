// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if !NET40
using System;
using System.Net.Http;
using Microsoft.Diagnostics.Context;

namespace Microsoft.Diagnostics.Correlation.Common.Http
{
    /// <summary>
    /// <see cref="IContextInjector{TContext,TRequest}"/>  implementation specific to <see cref="CorrelationContext"/> and HttpRequestMessage
    /// </summary>
    /// <remarks><see cref="CorrelationContextInjector"/> injects correlation-id header and child-request-id header. Names could be configured with <see cref="CorrelationHeaderInfo"/></remarks>
    public class CorrelationContextInjector : IContextInjector<CorrelationContext, HttpRequestMessage>
    {
        /// <summary>
        /// Injects <see cref="CorrelationContext"/> into <see cref="HttpRequestMessage"/> 
        /// </summary>
        /// <param name="context"><see cref="CorrelationContext"/> to be injected</param>
        /// <param name="request">Outgoing request</param>
        public void UpdateRequest(CorrelationContext context, HttpRequestMessage request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (request.Headers.Contains(CorrelationHeaderInfo.CorrelationIdHeaderName))
                throw new ArgumentException($"{CorrelationHeaderInfo.CorrelationIdHeaderName} header already exists");

            request.Headers.Add(CorrelationHeaderInfo.CorrelationIdHeaderName, context.CorrelationId);
            request.Headers.Add(CorrelationHeaderInfo.RequestIdHeaderName, Guid.NewGuid().ToString());
        }
    }
}
#endif
