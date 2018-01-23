using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace StoneCo.SplunkHECLibrary.DataContracts
{
    /// <summary>
    /// Represents a Json response from Splunk Http Events Collector.
    /// </summary>
    public class SplunkHECResponse : ISplunkHECResponse
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("invalid-event-number")]
        public int InvalidEventNumber { get; set; }

        public HttpStatusCode HttpResponseCode { get; set; }
    }
}
