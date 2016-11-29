// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Internal
{
    internal class CorrelationConfigurationSettings
    {
        internal class HeadersSettings
        {
            public string CorrelationIdHeaderName { get; set; }
            public string RequestIdHeaderName { get; set; }
        }

        internal class EndpointFilterSettings
        {
            public bool Allow { get; set; }
            public List<string> Endpoints { get; set; }
        }

        public bool? InstrumentOutgoingRequests { get; set; }
        public HeadersSettings Headers { get; set; }
        public EndpointFilterSettings EndpointFilter { get; set; }
    }
}
