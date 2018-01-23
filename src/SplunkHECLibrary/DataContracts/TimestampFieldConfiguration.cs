using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.SplunkHECLibrary.DataContracts
{
    public class TimestampFieldConfiguration
    {
        /// <summary>
        /// Field name to use as Timestamp.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Field timestamp format.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Base constructor.
        /// </summary>
        public TimestampFieldConfiguration()
        {

        }
    }
}
