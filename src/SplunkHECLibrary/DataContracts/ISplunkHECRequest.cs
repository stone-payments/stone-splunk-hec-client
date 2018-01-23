using System.Collections.Generic;

namespace StoneCo.SplunkHECLibrary.DataContracts
{
    /// <summary>
    /// An interface for a Splunk Http Events Collector request class.
    /// </summary>
    public interface ISplunkHECRequest
    {
        /// <summary>
        /// A collection with request items.
        /// </summary>
        IList<ISplunkHECDocument> Items { get; }

        /// <summary>
        /// Serializes the request correctly to send to Splunk HEC.
        /// </summary>
        string Serialize();
    }
}