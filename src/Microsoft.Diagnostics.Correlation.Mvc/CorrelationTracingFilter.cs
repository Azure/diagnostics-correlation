﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Web;
using System.Web.Mvc;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Http;

namespace Microsoft.Diagnostics.Correlation.Mvc
{
    /// <summary>
    /// Provides MVC <see cref="ActionFilterAttribute"/> to extract <see cref="CorrelationContext"/> from HttpContext.Current
    /// </summary>
    public class CorrelationTracingFilter : ActionFilterAttribute
    {
        private readonly CorrelationContextFactory contextFactory;

        /// <summary>
        /// CorrelationTracingFilter constructor
        /// </summary>
        public CorrelationTracingFilter()
        {
            contextFactory = new CorrelationContextFactory();
        }

        /// <summary>
        /// Inherited from <see cref="ActionFilterAttribute"/>
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ctx = contextFactory.CreateContext(HttpContext.Current.Request);
            ContextResolver.SetRequestContext(ctx);
        }
    }
}