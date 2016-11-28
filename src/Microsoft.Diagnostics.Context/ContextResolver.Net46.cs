// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if NETSTANDARD1_6 || NET46
using System.Threading;

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
        private static readonly AsyncLocal<object> Context = new AsyncLocal<object>();

        /// <summary>
        /// Sets context.
        /// </summary>
        /// <typeparam name="TContext">Type of correlation context</typeparam>
        /// <param name="context">Correlation context value</param>
        /// <returns>Context value</returns>
        public static TContext SetContext<TContext>(TContext context)
        {
            Context.Value = context;
            return context;
        }

        /// <summary>
        /// Gets correlation context
        /// </summary>
        /// <typeparam name="TContext">Type of correlation context</typeparam>
        /// <returns>Context value</returns>
        public static TContext GetContext<TContext>()
        {
            return (TContext)Context.Value;
        }

        /// <summary>
        /// Clears context
        /// </summary>
        public static void ClearContext()
        {
            Context.Value = null;
        }
    }
}
#endif