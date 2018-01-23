using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoneCo.SplunkHECLibrary.DataContracts;
using System;
using System.Collections.Generic;

namespace StoneCo.SplunkHECLibrary.UnitTest
{
    [TestClass]
    public class SplunkHECRequestTest
    {
        #region Constructor

        /// <summary>
        /// Constructor must initializes the Items collection with zero items.
        /// </summary>
        [TestMethod]
        public void Constructor_successfull()
        {
            ISplunkHECRequest request = new SplunkHECRequest();
            Assert.AreEqual(0, request.Items.Count);
        }

        [TestMethod]
        public void Constructor_with_document()
        {
            ISplunkHECDocument doc = new SplunkHECDocument();
            ISplunkHECRequest request = new SplunkHECRequest(doc);
            Assert.AreEqual(1, request.Items.Count);
            Assert.AreSame(doc, request.Items[0]);
        }

        [TestMethod]
        public void Constructor_with_document_when_document_is_null()
        {
            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(()=>
            {
                ISplunkHECDocument doc = null;
                ISplunkHECRequest request = new SplunkHECRequest(doc);
            });

            Assert.AreEqual("doc", ex.ParamName);
        }

        [TestMethod]
        public void Constructor_with_list()
        {
            ISplunkHECDocument doc1 = new SplunkHECDocument();
            ISplunkHECDocument doc2 = new SplunkHECDocument();
            List<ISplunkHECDocument> list = new List<ISplunkHECDocument>();
            list.Add(doc1);
            list.Add(doc2);

            ISplunkHECRequest request = new SplunkHECRequest(list);
            Assert.AreEqual(2, request.Items.Count);
            Assert.AreSame(doc1, request.Items[0]);
            Assert.AreSame(doc2, request.Items[1]);
        }

        [TestMethod]
        public void Constructor_with_list_when_list_is_null()
        {
            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                IEnumerable<ISplunkHECDocument> list = null;
                ISplunkHECRequest request = new SplunkHECRequest(list);
            });

            Assert.AreEqual("documents", ex.ParamName);
        }

        #endregion

        #region Serialize

        /// <summary>
        /// Must returns an empty string.
        /// </summary>
        [TestMethod]
        public void Serialize_with_no_items()
        {
            ISplunkHECRequest request = new SplunkHECRequest();
            string serializedRequest = request.Serialize();

            Assert.AreEqual(string.Empty, serializedRequest);
        }

        /// <summary>
        /// Must return one item with no new line and carriage return char at the end of string.
        /// </summary>
        [TestMethod]
        public void Serialize_with_one_item()
        {
            string expectedSerializedDoc = "{\"time\":1514958976,\"source\":\"source\",\"sourcetype\":\"sourcetype\",\"index\":\"index\"}";
            ISplunkHECDocument doc = new SplunkHECDocument { Event = null, SourceType = "sourcetype", Index = "index", Source = "source", Time = 1514958976 };
            ISplunkHECRequest request = new SplunkHECRequest();
            request.Items.Add(doc);
            string serializedRequest = request.Serialize();

            Assert.AreEqual(expectedSerializedDoc, serializedRequest);
        }

        /// <summary>
        /// Must return two items with no new line and carriage return char at the end of string.
        /// </summary>
        [TestMethod]
        public void Serialize_with_two_items()
        {
            string expectedSerializedDoc = "{\"time\":1514958976,\"source\":\"source\",\"sourcetype\":\"sourcetype\",\"index\":\"index\"}\r\n{\"time\":1514958976,\"source\":\"source\",\"sourcetype\":\"sourcetype\",\"index\":\"index\"}";
            ISplunkHECDocument doc1 = new SplunkHECDocument { Event = null, SourceType = "sourcetype", Index = "index", Source = "source", Time = 1514958976 };
            ISplunkHECDocument doc2 = new SplunkHECDocument { Event = null, SourceType = "sourcetype", Index = "index", Source = "source", Time = 1514958976 };
            ISplunkHECRequest request = new SplunkHECRequest();
            request.Items.Add(doc1);
            request.Items.Add(doc2);
            string serializedRequest = request.Serialize();

            Assert.AreEqual(expectedSerializedDoc, serializedRequest);
        }

        #endregion
    }
}
