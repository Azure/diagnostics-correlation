// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Internal
{
    internal class AspNetDiagnosticListenerObserver<TContext> : IObserver<KeyValuePair<string, object>>
    {
        private readonly IContextFactory<TContext, HttpRequest> contextFactory;

        public AspNetDiagnosticListenerObserver(IContextFactory<TContext, HttpRequest> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            if (value.Key != "Microsoft.AspNetCore.Hosting.BeginRequest" || value.Value == null)
                return;

            var httpContextInfo = value.Value.GetType().GetProperty("httpContext");
            var httpContext = (DefaultHttpContext) httpContextInfo?.GetValue(value.Value, null);
            if (httpContext != null)
            {
                var ctx = contextFactory.CreateContext(httpContext.Request);
                if (ctx != null)
                    ContextResolver.SetContext(ctx);
            }
        }

        public void OnCompleted() { }

        public void OnError(Exception error) { }
    }
}