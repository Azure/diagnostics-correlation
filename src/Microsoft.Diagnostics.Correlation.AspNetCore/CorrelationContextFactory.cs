// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;

namespace Microsoft.Diagnostics.Correlation.AspNetCore
{
    /// <summary>
    /// <see cref="IContextFactory{TContext,TRequest}"/> Implementation to extract <see cref="CorrelationContext"/> from <see cref="HttpRequest"/>
    /// </summary>
    public class CorrelationContextFactory : IContextFactory<CorrelationContext, HttpRequest>
    {
        /// <summary>
        /// Extracts <see cref="CorrelationContext"/> from <see cref="HttpRequest"/>
        /// </summary>
        /// <param name="request">Incoming HTTP request</param>
        /// <returns>Correlation context</returns>
        public CorrelationContext CreateContext(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            string correlationId = null;
            if (request.Headers.ContainsKey(CorrelationHeaderInfo.CorrelationIdHeaderName))
                correlationId = request.Headers[CorrelationHeaderInfo.CorrelationIdHeaderName].First();

            string requestId = null;
            if (request.Headers.ContainsKey(CorrelationHeaderInfo.RequestIdHeaderName))
                requestId = request.Headers[CorrelationHeaderInfo.RequestIdHeaderName].First();

            if (requestId == null)
                requestId = request.HttpContext.TraceIdentifier;

            return new CorrelationContext(correlationId ?? newGuid(), requestId ?? newGuid());
        }

        private string newGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}