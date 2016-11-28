// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using System.Web;


namespace Microsoft.Diagnostics.Correlation.Http
{
    /// <summary>
    /// <see cref="IContextFactory{TContext,TRequest}"/> Implementation to extract <see cref="CorrelationContext"/> from <see cref="HttpRequest"/>
    /// </summary>
    public class CorrelationContextFactory : IContextFactory<CorrelationContext, HttpRequest>
    {
        /// <summary>
        /// Extracts <see cref="CorrelationContext"/> from <see cref="HttpRequest"/>
        /// </summary>
        /// <param name="request">Incoming <see cref="HttpRequest"/></param>
        /// <returns>Extracted context <see cref="CorrelationContext"/></returns>
        public CorrelationContext CreateContext(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            string correlationId = getHeaderValue(request, CorrelationHeaderInfo.CorrelationIdHeaderName) ?? 
                newGuid();
            string requestId = getHeaderValue(request, CorrelationHeaderInfo.RequestIdHeaderName) ??
                getWorkerTraceIdentifier(request);

            return new CorrelationContext(correlationId, requestId ?? newGuid());
        }

        private string getHeaderValue(HttpRequest request, string name)
        {
            var requestHeader = request.Headers.GetValues(name);
            return requestHeader?.First();
        }

        private string getWorkerTraceIdentifier(HttpRequest request)
        {
            var httpWorkerRequest = (HttpWorkerRequest)request.RequestContext.HttpContext.GetService(typeof(HttpWorkerRequest));
            return httpWorkerRequest?.RequestTraceIdentifier.ToString();
        }

        private string newGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}