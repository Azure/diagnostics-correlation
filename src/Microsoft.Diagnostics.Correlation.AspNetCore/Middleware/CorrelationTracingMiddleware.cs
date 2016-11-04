// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.Diagnostics.Correlation.Common;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Middleware
{
    /// <summary>
    /// Provides ASP.NET Core middleware to extract and retain <see cref="CorrelationContext"/> from the incoming <see cref="HttpRequest"/>
    /// </summary>
    public class CorrelationContextTracingMiddleware : ContextTracingMiddleware<CorrelationContext>
    {
        /// <summary>
        /// ContextTracingMiddleware constructor
        /// </summary>
        /// <param name="next">Next middleware</param>
        public CorrelationContextTracingMiddleware(RequestDelegate next) : base(next, new CorrelationContextFactory())
        {
        }
    }
}