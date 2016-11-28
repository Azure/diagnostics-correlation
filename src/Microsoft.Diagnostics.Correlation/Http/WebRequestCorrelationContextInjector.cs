// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Net;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;

namespace Microsoft.Diagnostics.Correlation.Http
{
    /// <summary>
    /// <see cref="IContextInjector{TContext,TRequest}"/> implementation specific to <see cref="CorrelationContext"/> and <see cref="WebRequest"/>
    /// </summary>
    public class WebRequestCorrelationContextInjector : IContextInjector<CorrelationContext, WebRequest>
    {
        /// <summary>
        /// Injects context into outgoing request
        /// </summary>
        /// <param name="context">Context to inject</param>
        /// <param name="request">Outgoing request instance</param>
        public void UpdateRequest(CorrelationContext context, WebRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (request.Headers.GetValues(CorrelationHeaderInfo.CorrelationIdHeaderName) != null)
                throw new ArgumentException(CorrelationHeaderInfo.CorrelationIdHeaderName + " header already exists");

            request.Headers.Add(CorrelationHeaderInfo.CorrelationIdHeaderName, context.CorrelationId);
            request.Headers.Add(CorrelationHeaderInfo.RequestIdHeaderName, Guid.NewGuid().ToString());
        }
    }
}
