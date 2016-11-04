// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Diagnostics.Correlation.Common.Instrumentation
{
    /// <summary>
    /// Notifies about outgoing request events, implement this interface to log outgoing requests when using instrumentation
    /// </summary>
    /// <typeparam name="TContext">Type of correlation context</typeparam>
    /// <typeparam name="TRequest">Type of outgoing request</typeparam>
    /// <typeparam name="TResponse">Type of outgoing response</typeparam>
    public interface IOutgoingRequestNotifier<in TContext, in TRequest, in TResponse>
    {
        /// <summary>
        /// Corresponds to the moment reqest is about to be sent
        /// </summary>
        /// <param name="context">Correlation context containing child request id</param>
        /// <param name="request">Request instance</param>
        void OnBeforeRequest(TContext context, TRequest request);

        /// <summary>
        /// Corresponds to the moment, response is received
        /// </summary>
        /// <param name="context">Correlation context containing child request id</param>
        /// <param name="response">Response instance</param>
        void OnAfterResponse(TContext context, TResponse response);
    }
}