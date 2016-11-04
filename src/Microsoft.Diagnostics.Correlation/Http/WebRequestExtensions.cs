// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using System.Net;
using Microsoft.Diagnostics.Correlation.Common;

namespace Microsoft.Diagnostics.Correlation.Http
{
    public static class WebRequestExtensions
    {
        /// <summary>
        /// Gets child request id from the <see cref="WebRequest"/>
        /// </summary>
        /// <param name="request"><see cref="WebRequest"/> containing child request id</param>
        /// <returns>Child request id or null</returns>
        public static string GetChildRequestId(this WebRequest request)
        {
            var requestIdHeader = request.Headers.GetValues(CorrelationHeaderInfo.RequestIdHeaderName);
            return requestIdHeader?.First();
        }
    }
}
