// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common;
using Microsoft.Diagnostics.Correlation.Common.Http;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Diagnostics.Correlation.AspNetCore
{
    /// <summary>
    /// Provides default instrumentation configuration for ASP.NET Core apps, configures <see cref="CorrelationContext"/> to be used
    /// </summary>
    public class AspNetCoreCorrelationConfiguration : AspNetCoreConfiguration<CorrelationContext>
    {
        /// <summary>
        ///<see cref="AspNetCoreCorrelationConfiguration"/> constructor, sets up default <see cref="CorrelationContextFactory"/> and <see cref="CorrelationContextInjector"/>
        /// </summary>
        public AspNetCoreCorrelationConfiguration()
        {
            Initialize();
        }

        /// <summary>
        /// <see cref="AspNetCoreCorrelationConfiguration"/> constructor, sets up default <see cref="CorrelationContextFactory"/> and <see cref="CorrelationContextInjector"/>
        /// </summary>
        /// <param name="configuration">Correlation configuration</param>
        public AspNetCoreCorrelationConfiguration(IConfiguration configuration)
            : base(configuration)
        {
            Initialize();
        }

        private void Initialize()
        {
            ContextFactory = new CorrelationContextFactory();
            ContextInjectors = new List<IContextInjector<CorrelationContext, HttpRequestMessage>>
            {
                new CorrelationContextInjector()
            };
        }
    }
}
