using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoneCo.SplunkHECLibrary.DataContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.SplunkHECLibrary.UnitTest
{
    [TestClass]
    public class SplunkHECDocumentTest
    {

        #region Constructors

        [TestMethod]
        public void Constructor_with_document()
        {
            dynamic obj = new { Prop1 = "Value1" };
            ISplunkHECDocument doc = new SplunkHECDocument(obj);

            Assert.AreEqual(obj.Prop1, doc.Event["Prop1"].ToString());
        }

        #endregion

        #region Serialize

        /// <summary>
        /// Must serializes successfuly with all class properties in lower case.
        /// </summary>
        [TestMethod]
        public void Serialize_sucessfull()
        {
            string expectedSerializedDoc = "{\"time\":1514958976,\"source\":\"source\",\"sourcetype\":\"sourcetype\",\"index\":\"index\"}";
            SplunkHECDocument doc = new SplunkHECDocument { Event = null, SourceType = "sourcetype", Index = "index", Source = "source", Time = 1514958976 };
            string serializedDoc = doc.Serialize();

            Assert.AreEqual(expectedSerializedDoc, serializedDoc);            
        }

        /// <summary>
        /// Must return the default value (zero) for the time property.
        /// </summary>
        [TestMethod]
        public void Serialize_when_time_is_not_defined()
        {
            string expectedSerializedDoc = "{\"time\":0,\"source\":\"source\",\"sourcetype\":\"sourcetype\",\"index\":\"index\"}";
            SplunkHECDocument doc = new SplunkHECDocument { Event = null, SourceType = "sourcetype", Index = "index", Source = "source" };
            string serializedDoc = doc.Serialize();

            Assert.AreEqual(expectedSerializedDoc, serializedDoc);
        }

        /// <summary>
        /// Must return null for the property value.
        /// </summary>
        [TestMethod]
        public void Serialize_when_sourcetype_is_null()
        {
            string expectedSerializedDoc = "{\"time\":1514958976,\"source\":\"source\",\"index\":\"index\"}";
            SplunkHECDocument doc = new SplunkHECDocument { Event = null, Index = "index", Source = "source", Time = 1514958976 };
            string serializedDoc = doc.Serialize();

            Assert.AreEqual(expectedSerializedDoc, serializedDoc);
        }

        /// <summary>
        /// Must return an empy string for the property value.
        /// </summary>
        [TestMethod]
        public void Serialize_when_sourcetype_is_empty()
        {
            string expectedSerializedDoc = "{\"time\":1514958976,\"source\":\"source\",\"sourcetype\":\"\",\"index\":\"index\"}";
            SplunkHECDocument doc = new SplunkHECDocument { Event = null, SourceType = string.Empty, Index = "index", Source = "source", Time = 1514958976 };
            string serializedDoc = doc.Serialize();

            Assert.AreEqual(expectedSerializedDoc, serializedDoc);
        }

        /// <summary>
        /// Must return null for the property value.
        /// </summary>
        [TestMethod]
        public void Serialize_when_source_is_null()
        {
            string expectedSerializedDoc = "{\"time\":1514958976,\"sourcetype\":\"sourcetype\",\"index\":\"index\"}";
            SplunkHECDocument doc = new SplunkHECDocument { Event = null, SourceType = "sourcetype", Index = "index", Time = 1514958976 };
            string serializedDoc = doc.Serialize();

            Assert.AreEqual(expectedSerializedDoc, serializedDoc);
        }

        /// <summary>
        /// Must return an empy string for the property value.
        /// </summary>
        [TestMethod]
        public void Serialize_when_source_is_empty()
        {
            string expectedSerializedDoc = "{\"time\":1514958976,\"source\":\"\",\"sourcetype\":\"sourcetype\",\"index\":\"index\"}";
            SplunkHECDocument doc = new SplunkHECDocument { Event = null, SourceType = "sourcetype", Index = "index", Source = string.Empty, Time = 1514958976 };
            string serializedDoc = doc.Serialize();

            Assert.AreEqual(expectedSerializedDoc, serializedDoc);
        }

        /// <summary>
        /// Must return null for the property value.
        /// </summary>
        [TestMethod]
        public void Serialize_when_index_is_null()
        {
            string expectedSerializedDoc = "{\"time\":1514958976,\"source\":\"source\",\"sourcetype\":\"sourcetype\"}";
            SplunkHECDocument doc = new SplunkHECDocument { Event = null, SourceType = "sourcetype", Source = "source", Time = 1514958976 };
            string serializedDoc = doc.Serialize();

            Assert.AreEqual(expectedSerializedDoc, serializedDoc);
        }

        /// <summary>
        /// Must return an empy string for the property value.
        /// </summary>
        [TestMethod]
        public void Serialize_when_index_is_empty()
        {
            string expectedSerializedDoc = "{\"time\":1514958976,\"source\":\"source\",\"sourcetype\":\"sourcetype\",\"index\":\"\"}";
            SplunkHECDocument doc = new SplunkHECDocument { Event = null, SourceType = "sourcetype", Index = string.Empty, Source = "source", Time = 1514958976 };
            string serializedDoc = doc.Serialize();

            Assert.AreEqual(expectedSerializedDoc, serializedDoc);
        }

        #endregion
    }
}
