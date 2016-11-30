// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Net.Http;
using Microsoft.Diagnostics.Context;
using Microsoft.Diagnostics.Correlation.Common.Instrumentation;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Diagnostics.Correlation.AspNetCore
{
    public static class ServiceCollectionExtenstions
    {
        /// <summary>
        /// Adds request notifier service
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> services</param>
        /// <param name="notifier">IInstance of <see cref="IOutgoingRequestNotifier{CorrelationContext,HttpRequestMessage,HttpResponseMessage}"/></param>
        /// <returns><see cref="IServiceCollection"/> services</returns>
        public static IServiceCollection AddRequestNotifier(this IServiceCollection services,
            IOutgoingRequestNotifier<CorrelationContext, HttpRequestMessage, HttpResponseMessage> notifier)
        {
            services.AddSingleton<IOutgoingRequestNotifier<CorrelationContext, HttpRequestMessage, HttpResponseMessage>>
                (notifier);
            return services;
        }
    }
}
