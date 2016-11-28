// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if !NET40
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Microsoft.Diagnostics.Correlation.Owin
{
    /// <summary>
    /// Provides OWIN middleware to extract and retain generic correlation context from the incoming request
    /// </summary>
    /// <typeparam name="TContext">Type of context</typeparam>
    public class ContextTracingMiddleware<TContext>
    {
        private readonly AppFunc next;
        private readonly IContextFactory<TContext, IDictionary<string, object>> contextFactory;

        /// <summary>
        /// Middleware constructor
        /// </summary>
        /// <param name="next">Next middleware</param>
        /// <param name="contextFactory">Implementation of <see cref="IContextFactory{TContext,TRequest}"/> to be used for request parsing</param>
        public ContextTracingMiddleware(AppFunc next, IContextFactory<TContext, IDictionary<string, object>> contextFactory)
        {
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));

            this.next = next;
            this.contextFactory = contextFactory;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            try
            {
                var ctx = contextFactory.CreateContext(environment);
                ContextResolver.SetContext(ctx);

                await next.Invoke(environment).ConfigureAwait(false);
            }
            finally
            {
                ContextResolver.ClearContext();
            }
        }
    }
}
#endif