// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Web;
using Microsoft.Diagnostics.Context;

namespace Microsoft.Diagnostics.Correlation.Http
{
    /// <summary>
    /// <see cref="IHttpModule"/> implementation for incoming request handling
    /// </summary>
    public sealed class CorrelationTracingHttpModule : IHttpModule
    {
        private readonly CorrelationContextFactory factory = new CorrelationContextFactory();

        /// <summary>
        /// Inherited from <see cref="IHttpModule"/>
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += application_BeginRequest;
        }

        /// <summary>
        /// Inherited from <see cref="IHttpModule"/>
        /// </summary>
        public void Dispose()
        {

        }

        private void application_BeginRequest(object source,
             EventArgs e)
        {
            HttpApplication application = (HttpApplication)source;
            var ctx = factory.CreateContext(application.Context.Request);
            if (ctx != null)
                ContextResolver.SetContext(ctx);
        }
    }
}
