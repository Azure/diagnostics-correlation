// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common.Instrumentation;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Diagnostics.Correlation.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Enables Correlation instrumentation
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> application builder</param>
        /// <param name="configuration">Correlation confgiuration</param>
        /// <returns><see cref="IApplicationBuilder"/> application builder</returns>
        /// <remarks>Add <see cref="IOutgoingRequestNotifier{CorrelationContext,HttpRequestMessage,HttpResponseMessage}"/> singleton to services to receive outgoing requests events</remarks>
        public static IApplicationBuilder UseCorrelationInstrumentation(this IApplicationBuilder app, IConfiguration configuration)
        {
            Initialize(app, new AspNetCoreCorrelationConfiguration(configuration));
            return app;
        }

        /// <summary>
        /// Enables Correlation instrumentation
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> application builder</param>
        /// <returns><see cref="IApplicationBuilder"/> application builder</returns>
        /// <remarks>Add <see cref="IOutgoingRequestNotifier{CorrelationContext,HttpRequestMessage,HttpResponseMessage}"/> singleton to services to receive outgoing requests events</remarks>
        public static IApplicationBuilder UseCorrelationInstrumentation(this IApplicationBuilder app)
        {
            Initialize(app, new AspNetCoreCorrelationConfiguration());
            return app;
        }

        private static void Initialize(IApplicationBuilder app, AspNetCoreCorrelationConfiguration configuration)
        {
            var notifier = app.ApplicationServices.GetService(typeof(IOutgoingRequestNotifier<CorrelationContext, HttpRequestMessage, HttpResponseMessage>));

            if (notifier != null)
                configuration.RequestNotifier = notifier as IOutgoingRequestNotifier<CorrelationContext, HttpRequestMessage, HttpResponseMessage>;

            var instrumentaion = ContextTracingInstrumentation.Enable(configuration);

            var appLifetime = app.ApplicationServices.GetService(typeof(IApplicationLifetime)) as IApplicationLifetime;

            appLifetime?.ApplicationStopped.Register(() => instrumentaion?.Dispose());
        }
    }
}