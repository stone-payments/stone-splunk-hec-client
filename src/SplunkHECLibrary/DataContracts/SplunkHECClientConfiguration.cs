using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.SplunkHECLibrary.DataContracts
{
    /// <summary>
    /// A contract class for the configuration of the Splunk Http Events Collector.
    /// </summary>
    public class SplunkHECClientConfiguration : ISplunkHECClientConfiguration
    {

        #region Public properties

        /// <summary>
        /// Endpoint to Splunk Http Event Collector.
        /// </summary>
        public Uri Endpoint { get; set; }

        /// <summary>
        /// Index to use if is not defined in document.
        /// </summary>
        public string DefaultIndex { get; set; }

        /// <summary>
        /// Source to use if is not defined in document.
        /// </summary>
        public string DefaultSource { get; set; }

        /// <summary>
        /// SourceType to use if is not defined in document.
        /// </summary>
        public string DefaultSourceType { get; set; }

        /// <summary>
        /// Host to use if is not defined in document.
        /// </summary>
        public string DefaultHost { get; set; }

        /// <summary>
        /// Authentication token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Field to use as timestamp. If not defined, the timestamp will be created by Splunk at insert time.
        /// </summary>
        public TimestampFieldConfiguration UseTimestampField { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Base constructor.
        /// </summary>
        public SplunkHECClientConfiguration()
        {

        }

        /// <summary>
        /// Constructor with IConfiguration.
        /// </summary>
        /// <param name="config">Configuration.</param>
        public SplunkHECClientConfiguration(IConfiguration config)
        {
            #region Validations                    
           
            if (config.GetSection("Endpoint").Exists() == false)
            {
                throw new ArgumentNullException(nameof(this.Endpoint));
            }

            if (config.GetSection("Token").Exists() == false)
            {
                throw new ArgumentNullException(nameof(this.Token));
            }

            Uri endpoint;
            if(Uri.TryCreate(config["Endpoint"], UriKind.Absolute, out endpoint) == false)
            {
                throw new ArgumentException("Please inform a valid absolute Uri.", nameof(this.Endpoint));
            }

            if (config.GetSection("DefaultSourceType").Exists() == false)
            {
                throw new ArgumentNullException(nameof(this.DefaultSourceType));
            }

            if (string.IsNullOrWhiteSpace(config["DefaultSourceType"]))
            {
                throw new ArgumentNullException(nameof(this.DefaultSourceType));
            }

            #endregion

            this.Endpoint = endpoint;
            this.DefaultSourceType = config["DefaultSourceType"];
            this.Token = config["Token"];
        }

        #endregion

    }
}
