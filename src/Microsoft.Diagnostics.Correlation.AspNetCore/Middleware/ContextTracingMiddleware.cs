// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Diagnostics.Correlation.Common;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Middleware
{
    /// <summary>
    /// Provides ASP.NET Core middleware to extract and retain generic correlation context from the incoming <see cref="HttpRequest"/>
    /// </summary>
    /// <typeparam name="TContext">Type of context</typeparam>
    public class ContextTracingMiddleware<TContext>
    {
        private readonly RequestDelegate next;
        private readonly IContextFactory<TContext, HttpRequest> contextFactory;

        /// <summary>
        /// ContextTracingMiddleware constructor
        /// </summary>
        /// <param name="next">Next middleware</param>
        /// <param name="contextFactory">Context factory to extract context from the <see cref="HttpRequest"/></param>
        public ContextTracingMiddleware(RequestDelegate next, IContextFactory<TContext, HttpRequest> contextFactory)
        {
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));

            this.next = next;
            this.contextFactory = contextFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var ctx = default(TContext);
            try
            {
                ctx = contextFactory.CreateContext(context.Request);
                ContextResolver.SetRequestContext(ctx);

                await next.Invoke(context).ConfigureAwait(false);
            }
            finally
            {
                if (ctx != null)
                    ContextResolver.ClearContext();
            }
        }
    }
}