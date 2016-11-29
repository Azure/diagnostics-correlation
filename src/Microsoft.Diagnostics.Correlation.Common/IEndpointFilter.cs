// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.Diagnostics.Correlation.Common
{
    /// <summary>
    /// Validates outgoing request Uri determining if it should be instrumented
    /// </summary>
    public interface IEndpointFilter
    {
        /// <summary>
        /// Validates Uri to check if it should be instrumented
        /// </summary>
        /// <param name="uri">Uri to check</param>
        /// <returns>True if endoiunt should be instrumented, false otherwise</returns>
        bool Validate(Uri uri);
    }
}
