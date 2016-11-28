// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Net;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Http;

namespace Microsoft.Diagnostics.Correlation.Instrumentation
{
    /// <summary>
    /// Provides default configuration for profiler instrumentation, configures <see cref="CorrelationContext"/> to be used
    /// </summary>
    public class ProfilerCorrelationConfiguration : ProfilerConfiguration<CorrelationContext>
    {
        /// <summary>
        /// <see cref="ProfilerCorrelationConfiguration"/> constructor, sets up default <see cref="CorrelationContextFactory"/> and <see cref="WebRequestCorrelationContextInjector"/>
        /// </summary>
        public ProfilerCorrelationConfiguration()
        {
            ContextInjectors = new List<IContextInjector<CorrelationContext, WebRequest>>
            {
                new WebRequestCorrelationContextInjector()
            };
        }
    }

}
