using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.SplunkHECLibrary
{
    /// <summary>
    /// Class with serialization settings.
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Serializer used by this class.
        /// </summary>
        public static JsonSerializer Serializer { get; set; }

        /// <summary>
        /// Json serializer settings.
        /// </summary>
        public static JsonSerializerSettings Settings { get; set; }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Serialization()
        {
            Settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                DateParseHandling = DateParseHandling.DateTimeOffset
            };

            Serializer = JsonSerializer.Create(Settings);
        }

    }
}
