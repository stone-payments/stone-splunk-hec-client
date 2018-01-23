using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace StoneCo.SplunkHECLibrary.DataContracts
{
    /// <summary>
    /// An interaface for a contract class for the Splunk Http Event Collector.
    /// </summary>
    public interface ISplunkHECDocument
    {
        /// <summary>
        /// The event to index.
        /// </summary>
        JObject Event { get; set; }

        /// <summary>
        /// The host name.
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// The index name.
        /// </summary>
        string Index { get; set; }

        /// <summary>
        /// The source name.
        /// </summary>
        string Source { get; set; }

        /// <summary>
        /// The source type name.
        /// </summary>
        string SourceType { get; set; }

        /// <summary>
        /// Timestamp in Unix Epoch Time format.
        /// </summary>
        long Time { get; set; }

        /// <summary>
        /// Serializes this object in Json notation.
        /// </summary>
        string Serialize();
    }
}