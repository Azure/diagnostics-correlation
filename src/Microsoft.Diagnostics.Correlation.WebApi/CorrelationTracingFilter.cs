// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Http;

namespace Microsoft.Diagnostics.Correlation.WebApi
{
    /// <summary>
    /// Provides WebApi <see cref="ActionFilterAttribute"/> to extract <see cref="CorrelationContext"/> from HttpContext.Current
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
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var ctx = contextFactory.CreateContext(HttpContext.Current.Request);
            ContextResolver.SetContext(ctx);
        }
    }
}