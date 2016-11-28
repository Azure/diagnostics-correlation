// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if !NET40
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;

namespace Microsoft.Diagnostics.Correlation.Owin
{
    /// <summary>
    /// OWIN-specific implementation of <see cref="IContextInjector{TContext,TRequest}"/> for <see cref="CorrelationContext"/>
    /// </summary>
    public class OwinCorrelationContextFactory : IContextFactory<CorrelationContext, IDictionary<string, object>>
    {
        /// <summary>
        /// Creates CorrelationContext from OWIN environment
        /// </summary>
        /// <param name="environment">OWIN environment</param>
        /// <returns></returns>
        public CorrelationContext CreateContext(IDictionary<string, object> environment)
        {
            if (environment == null)
                throw new ArgumentNullException(nameof(environment));

            object headersObj;
            string correlationId = null;
            string requestId = null;
            if (environment.TryGetValue("owin.RequestHeaders", out headersObj))
            {
                var requestHeaders = headersObj as IDictionary<string, string[]>;
                if (requestHeaders != null)
                {
                    correlationId = getHeaderValue(requestHeaders, CorrelationHeaderInfo.CorrelationIdHeaderName);
                    requestId = getHeaderValue(requestHeaders, CorrelationHeaderInfo.RequestIdHeaderName);
                }
            }

            object requestIdObj;
            if (environment.TryGetValue("owin.RequestId", out requestIdObj))
            {
                if (requestId == null)
                    requestId = (string)requestIdObj;
            }
            else
            {
                requestId = Guid.NewGuid().ToString();
                environment.Add("owin.RequestId", requestId);
            }

            return new CorrelationContext(correlationId ?? Guid.NewGuid().ToString(), requestId);
        }

        private string getHeaderValue(IDictionary<string, string[]> requestHeaders, string name)
        {
            string[] header;
            return requestHeaders.TryGetValue(name, out header) ? header.First() : null;
        }
    }
}
#endif