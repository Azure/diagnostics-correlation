// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if !NET40
using Microsoft.Diagnostics.Context;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Microsoft.Diagnostics.Correlation.Owin
{
    /// <summary>
    /// Provides OWIN middleware to extract and retain <see cref="CorrelationContext"/> from the incoming request
    /// </summary>
    public class CorrelationContextTracingMiddleware : ContextTracingMiddleware<CorrelationContext>
    {
        /// <summary>
        /// Middleware constructor
        /// </summary>
        /// <param name="next">Next middleware</param>
        public CorrelationContextTracingMiddleware(AppFunc next) : base(next, new OwinCorrelationContextFactory())
        {
        }
    }
}
#endif