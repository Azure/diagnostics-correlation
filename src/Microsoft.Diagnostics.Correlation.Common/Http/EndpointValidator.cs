// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Diagnostics.Correlation.Common.Http
{
    /// <summary>
    /// Implements <see cref="IEndpointValidator"/> allowing to disable/enable outgoing request instrumentation for list of HTTP endpoints
    /// </summary>
    public class EndpointValidator : IEndpointValidator
    {
        private readonly HashSet<string> endpoints = new HashSet<string>();
        private readonly bool whitelist;

        /// <summary>
        /// Constructs EndpointValidator to disable instrumentation for HTTP requests done to Azure Storage and ApplicationInsights APIs
        /// </summary>
        public EndpointValidator()
        {
            whitelist = false;
            endpoints.Add(@"core\.windows\.net");
            endpoints.Add(@"dc\.services\.visualstudio\.com");
        }

        /// <summary>
        /// Constructs EndpointValidator
        /// </summary>
        /// <param name="endpointPatterns">List of Regex patterns for endpoints</param>
        /// <param name="whitelist">Allows to instrument enpoints matching the pattern (if set to true) or NOT matching the pattern (if set to false)</param>
        public EndpointValidator(IEnumerable<string> endpointPatterns, bool whitelist = true)
        {
            if (endpoints == null) return;

            foreach (var endpoint in endpointPatterns)
            {
                endpoints.Add(endpoint);
            }

            this.whitelist = whitelist;
        }

        /// <summary>
        /// Adds endpoint pattern
        /// </summary>
        /// <param name="endpointPattern">Pattern to add</param>
        /// <returns>Current EndpointValidator</returns>
        public EndpointValidator AddEndpoint(string endpointPattern)
        {
            endpoints.Add(endpointPattern);
            return this;
        }

        /// <summary>
        /// Validates Uri to check if it should be insrumented
        /// </summary>
        /// <param name="uri">Uri to check</param>
        /// <returns>True if endoiunt should be instrumented, false otherwise</returns>
        public bool Validate(Uri uri)
        {
            if (endpoints.Count == 0) return true;

            if (endpoints.Any(endpoint => Regex.Match(uri.ToString(), endpoint).Success))
            {
                return whitelist;
            }
            return !whitelist;
        }
    }
}
