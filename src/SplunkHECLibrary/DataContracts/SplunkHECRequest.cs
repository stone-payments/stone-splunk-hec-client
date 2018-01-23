using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.SplunkHECLibrary.DataContracts
{
    /// <summary>
    /// A Splunk Http Events Collector request class.
    /// </summary>
    public class SplunkHECRequest : ISplunkHECRequest
    {

        #region Public properties

        /// <summary>
        /// A collection with request items.
        /// </summary>
        public IList<ISplunkHECDocument> Items { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Base constructor initializing Items property.
        /// </summary>
        public SplunkHECRequest()
        {
            this.Items = new List<ISplunkHECDocument>();
        }

        /// <summary>
        /// Initialize with one document.
        /// </summary>
        /// <param name="doc"></param>
        public SplunkHECRequest(ISplunkHECDocument doc)
        {
            #region Validations

            if(doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            #endregion

            this.Items = new List<ISplunkHECDocument>();
            this.Items.Add(doc);
        }

        /// <summary>
        /// Initialize with a list with documents.
        /// </summary>
        /// <param name="documents"></param>
        public SplunkHECRequest(IEnumerable<ISplunkHECDocument> documents)
        {
            #region Validations

            if(documents == null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            #endregion

            this.Items = new List<ISplunkHECDocument>(documents);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Serializes the request correctly to send to Splunk HEC.
        /// </summary>
        public string Serialize()
        {
            StringBuilder builder = new StringBuilder();

            foreach (ISplunkHECDocument item in this.Items)
            {
                builder.AppendLine(item.Serialize());
            }

            return builder.ToString().TrimEnd('\r', '\n');
        }

        #endregion
    }
}
