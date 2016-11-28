// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Diagnostics.Context
{
    /// <summary>
    /// Base ICorrelationContext interface
    /// </summary>
    /// <typeparam name="TContext">Type of context</typeparam>
    public interface ICorrelationContext<out TContext>
    {
        /// <summary>
        /// Creates copy of 'parent' context for outgoing request with child request id
        /// </summary>
        /// <param name="childRequestId">Unique id for outgoing request</param>
        /// <returns>Child context</returns>
        TContext GetChildRequestContext(string childRequestId);
    }
}
