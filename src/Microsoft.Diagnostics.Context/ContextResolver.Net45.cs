// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if NET45 || NET40
using System.Runtime.Remoting.Messaging;

namespace Microsoft.Diagnostics.Context
{
    /// <summary>
    /// Provides access to generic correlation context
    /// </summary>
    /// <remarks>
    /// Context is stored as object
    /// </remarks>
    public class ContextResolver
    {
        private const string Slot = "x-ms-correlation-id-slot";

        /// <summary>
        /// Sets context.
        /// </summary>
        /// <typeparam name="TContext">Type of correlation context</typeparam>
        /// <param name="context">Correlation context value</param>
        /// <returns>Context value</returns>
        public static TContext SetContext<TContext>(TContext context)
        {
            CallContext.LogicalSetData(Slot, context);
            return context;
        }

        /// <summary>
        /// Gets correlation context
        /// </summary>
        /// <typeparam name="TContext">Type of correlation context</typeparam>
        /// <returns>Context value</returns>
        public static TContext GetContext<TContext>()
        {
            return (TContext)CallContext.LogicalGetData(Slot);
        }

        /// <summary>
        /// Clears context
        /// </summary>
        public static void ClearContext()
        {
            CallContext.LogicalSetData(Slot, null);
        }
    }
}
#endif