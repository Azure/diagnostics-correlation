// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Diagnostics.Correlation.Common
{
    /// <summary>
    /// Creates context from request
    /// </summary>
    /// <typeparam name="TContext">Type of context</typeparam>
    /// <typeparam name="TRequest">Type of request</typeparam>
    public interface IContextFactory<out TContext, in TRequest>
    {
        /// <summary>
        /// Creates correlation context from the request
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Correlation context</returns>
        TContext CreateContext(TRequest request);
    }
}
