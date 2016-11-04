// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if !NET40
using System.Linq;
using System.Net.Http;

namespace Microsoft.Diagnostics.Correlation.Common.Http
{
    /// <summary>
    /// Provides HttpRequestMessage extensions to work with child request id
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Gets child request id from the <see cref="HttpRequestMessage"/>
        /// </summary>
        /// <param name="request"><see cref="HttpRequestMessage"/> containing child request id</param>
        /// <returns>Child request id or null</returns>
        public static string GetChildRequestId(this HttpRequestMessage request)
        {
            if (request.Headers.Contains(CorrelationHeaderInfo.RequestIdHeaderName))
                return request.Headers.GetValues(CorrelationHeaderInfo.RequestIdHeaderName).First();

            return null;
        }
    }
}
#endif