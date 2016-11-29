namespace Microsoft.Diagnostics.Correlation.Common
{
    /// <summary>
    /// Correlation header names
    /// </summary>
    public class CorrelationHeaderInfo
    {
        /// <summary>
        /// CorrelationId header name
        /// </summary>
        public static string CorrelationIdHeaderName = "x-ms-request-root-id";

        /// <summary>
        /// RequestId header name
        /// </summary>
        public static string RequestIdHeaderName = "x-ms-request-id";
    }
}
