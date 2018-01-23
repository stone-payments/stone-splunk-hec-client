using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.SplunkHECLibrary.DataContracts
{
    /// <summary>
    /// A contract class for the Splunk Http Event Collector.
    /// </summary>
    public class SplunkHECDocument : ISplunkHECDocument
    {

        #region Private Properties

        /// <summary>
        /// Serializer used by this class.
        /// </summary>
        private JsonSerializer Serializer { get; set; }

        /// <summary>
        /// Json serializer settings.
        /// </summary>
        private JsonSerializerSettings Settings { get; set; }

        #endregion

        #region Public properties

        /// <summary>
        /// Timestamp in Unix Epoch Time format.
        /// </summary>
        [JsonProperty("time")]
        public long Time { get; set; }

        /// <summary>
        /// The source name.
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// The source name.
        /// </summary>
        [JsonProperty("sourcetype")]
        public string SourceType { get; set; }

        /// <summary>
        /// The index name.
        /// </summary>
        [JsonProperty("index")]
        public string Index { get; set; }

        /// <summary>
        /// The event to index.
        /// </summary>
        [JsonProperty("event")]
        public JObject Event { get; set; }

        /// <summary>
        /// The host name.
        /// </summary>
        [JsonProperty("host")]
        public string Host { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor used for testint purposes.
        /// </summary>
        public SplunkHECDocument()
        {
            this.Configure();
        }

        /// <summary>
        /// Constructor with document.
        /// </summary>
        /// <param name="doc"></param>
        public SplunkHECDocument(object doc = null)
        {
            this.Configure();
            this.Event = JObject.FromObject(doc, Serializer);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Configure serialization behavior.
        /// </summary>
        private void Configure()
        {
            Settings = Serialization.Settings;
            Serializer = Serialization.Serializer;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Serializes this object in Json notation.
        /// </summary>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }

        #endregion
    }
}
