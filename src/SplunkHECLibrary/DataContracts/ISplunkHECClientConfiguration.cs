using System;

namespace StoneCo.SplunkHECLibrary.DataContracts
{
    public interface ISplunkHECClientConfiguration
    {
        /// <summary>
        /// Endpoint to Splunk Http Event Collector.
        /// </summary>
        Uri Endpoint { get; set; }

        /// <summary>
        /// Index to use if is not defined in document.
        /// </summary>
        string DefaultIndex { get; set; }

        /// <summary>
        /// Source to use if is not defined in document.
        /// </summary>
        string DefaultSource { get; set; }

        /// <summary>
        /// SourceType to use if is not defined in document.
        /// </summary>
        string DefaultSourceType { get; set; }

        /// <summary>
        /// Host to use if is not defined in document.
        /// </summary>
        string DefaultHost { get; set; }

        /// <summary>
        /// Authentication token.
        /// </summary>
        string Token { get; set; }

        /// <summary>
        /// Field to use as timestamp. If not defined, the timestamp will be created by Splunk at insert time.
        /// </summary>
        TimestampFieldConfiguration UseTimestampField { get; set; }
    }
}