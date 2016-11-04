// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Diagnostics.Correlation.Common
{
    /// <summary>
    /// Injects context into request
    /// </summary>
    /// <typeparam name="TContext">Type of correlation context</typeparam>
    /// <typeparam name="TRequest">Type of HTTP request</typeparam>
    public interface IContextInjector<in TContext, in TRequest>
    {
        /// <summary>
        /// Injects TContext into TRequest
        /// </summary>
        /// <param name="ctx">Generic correlation context instance to be injected into request</param>
        /// <param name="request">Outgoing request instance</param>
        void UpdateRequest(TContext ctx, TRequest request);
    }
}
