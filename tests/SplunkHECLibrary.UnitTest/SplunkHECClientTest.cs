using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using StoneCo.LapiSplunkConsumer.UnitTest.Mocks;
using StoneCo.SplunkHECLibrary.DataContracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StoneCo.SplunkHECLibrary.UnitTest
{
    [TestClass]
    public class SplunkHECClientTest
    {
        #region Private Properties

        /// <summary>
        /// The valid configuration for testing purposes.
        /// </summary>
        private ISplunkHECClientConfiguration Configuration { get; set; }

        #endregion

        #region TestInitialize

        [TestInitialize]
        public void TestInitialize()
        {
            this.Configuration = new SplunkHECClientConfiguration
            {
                Endpoint = new Uri("http://localhost/endpoint"),
                DefaultSourceType = "sourcetype",
                Token = "token"
            };
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Must pass.
        /// </summary>
        [TestMethod]
        public void Constructor_with_handler_success()
        {
            HttpMessageHandler httpMessageHandler = new HttpClientHandler();
            ISplunkHECClient client = new SplunkHECClient(httpMessageHandler);
        }

        /// <summary>
        /// Must threw an ArgumentNullException.
        /// </summary>
        [TestMethod]
        public void Constructor_with_handler_when_handler_is_null()
        {
            HttpMessageHandler httpMessageHandler = null;            
            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                ISplunkHECClient client = new SplunkHECClient(httpMessageHandler);
            });

            Assert.AreEqual("handler", ex.ParamName);
        }

        /// <summary>
        /// Must pass.
        /// </summary>
        [TestMethod]
        public void Constructor_with_config_and_handler_when_just_config_is_null()
        {
            HttpMessageHandler httpMessageHandler = null;
            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();

            ISplunkHECClient client = new SplunkHECClient(config, httpMessageHandler);
        }

        /// <summary>
        /// Must pass.
        /// </summary>
        [TestMethod]
        public void Constructor_with_config_and_handler_not_passing_handler()
        {
            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            ISplunkHECClient client = new SplunkHECClient(config);
        }

        /// <summary>
        /// Must pass.
        /// </summary>
        [TestMethod]
        public void Constructor_with_config_and_handler_when_config_and_handler_is_not_null()
        {
            HttpMessageHandler httpMessageHandler = new HttpClientHandler();
            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();

            ISplunkHECClient client = new SplunkHECClient(config, httpMessageHandler);
        }

        /// <summary>
        /// Must threw an ArgumentNullException.
        /// </summary>
        [TestMethod]
        public void Constructor_with_config_and_handler_when_config_is_null()
        {            
            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                ISplunkHECClient client = new SplunkHECClient(null, null);
            });

            Assert.AreEqual("config", ex.ParamName);
        }

        #endregion

        #region Send

        /// <summary>
        /// Must pass.
        /// </summary>
        [TestMethod]
        public void Send_when_return_is_ok()
        {
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            string route = string.Format("{0}/event", this.Configuration.Endpoint.OriginalString);
            mockHttp.When(route)
                .Respond("application/javascript", "{\"text\":\"Success\",\"code\":0}");

            ISplunkHECClient client = new SplunkHECClient(this.Configuration, mockHttp);
            ISplunkHECRequest request = null;
            bool eventCalled = false;

            client.BeforeSend += (sender, originalRequest) =>
            {
                Assert.AreSame(request, originalRequest);
            };

            client.AfterSend += (sender, originalResponse) =>
            {
                eventCalled = true;
                Assert.IsTrue(eventCalled);
            };

            JObject obj = new JObject();
            obj.Add("Member", "Value");

            request = new SplunkHECRequest();
            request.Items.Add(new SplunkHECDocument{
                Event = obj,
                SourceType = "sourcetype",
                Index = "index",
                Source = "source",
                Time = 1514958976
            });

            ISplunkHECResponse response = client.Send(request);
            Assert.AreEqual(0, response.Code);
            Assert.AreEqual("Success", response.Text);
            Assert.AreEqual(HttpStatusCode.OK, response.HttpResponseCode);
        }

        /// <summary>
        /// Must pass.
        /// </summary>
        [TestMethod]
        public void Send_when_response_is_empty()
        {
            string route = string.Format("{0}/event", this.Configuration.Endpoint.OriginalString);
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When(route)
                .Respond("application/javascript", "");

            ISplunkHECClient client = new SplunkHECClient(this.Configuration, mockHttp);
            ISplunkHECRequest request = null;
            bool eventCalled = false;

            client.BeforeSend += (sender, originalRequest) =>
            {
                Assert.AreSame(request, originalRequest);
            };

            client.AfterSend += (sender, originalResponse) =>
            {
                eventCalled = true;
                Assert.IsTrue(eventCalled);
            };

            JObject obj = new JObject();
            obj.Add("Member", "Value");

            request = new SplunkHECRequest();
            request.Items.Add(new SplunkHECDocument
            {
                Event = obj,
                SourceType = "sourcetype",
                Index = "index",
                Source = "source",
                Time = 1514958976
            });

            ISplunkHECResponse response = client.Send(request);            
            Assert.AreEqual(0, response.Code);
            Assert.IsNull(response.Text);
            Assert.AreEqual(HttpStatusCode.OK, response.HttpResponseCode);
        }

        [TestMethod]
        public void Send_when_request_data_is_malformed()
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.BadRequest;
            httpResponse.Content = new StringContent("{\"text\":\"Invalid data format\",\"code\":6,\"invalid-event-number\":0}");

            string route = string.Format("{0}/event", this.Configuration.Endpoint.OriginalString);
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When(route)
                .Respond(httpResponse.StatusCode, httpResponse.Content);

            ISplunkHECClient client = new SplunkHECClient(this.Configuration, mockHttp);
            ISplunkHECRequest request = new SplunkHECRequest();

            JObject obj = new JObject();
            obj.Add("Member", "Value");
            request.Items.Add(new SplunkHECDocument
            {
                Event = obj,
                SourceType = "sourcetype",
                Index = "index",
                Source = "source",
                Time = 1514958976
            });

            ISplunkHECResponse response = client.Send(request);
            Assert.AreEqual(6, response.Code);
            Assert.AreEqual("Invalid data format", response.Text);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.HttpResponseCode);
            Assert.AreEqual(0, response.InvalidEventNumber);
        }

        #endregion

        #region OverrideEventFields

        /// <summary>
        /// Doc Timestamp field value must be the same of the given date time.
        /// </summary>
        [TestMethod]
        public void OverrideEventFields_using_timestamp_field_pass()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);
            long expectedUnixTimestamp = dateTime.ToUnixTimeMilliseconds();

            Assert.AreEqual(expectedUnixTimestamp, doc.Time);
        }

        [TestMethod]
        public void OverrideEventFields_using_timestamp_field_when_filed_dont_exists()
        {            
            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();

            ArgumentException ex = Assert.ThrowsException<ArgumentException>(()=>
            {
                SplunkHECClientMock client = new SplunkHECClientMock();
                client.OverrideEventFields(config, doc);
            });

            Assert.AreEqual(nameof(config.UseTimestampField), ex.ParamName);
            Assert.AreEqual($"The timestamp field name don't exists on event.\r\nParameter name: {nameof(config.UseTimestampField)}", ex.Message);
        }

        [TestMethod]
        public void OverrideEventFields_using_timestamp_field_when_format_is_invalid()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = "17/08/1982T10:23:23";            

            ArgumentException ex = Assert.ThrowsException<ArgumentException>(() =>
            {
                SplunkHECClientMock client = new SplunkHECClientMock();
                client.OverrideEventFields(config, doc);
            });

            Assert.AreEqual(nameof(config.UseTimestampField), ex.ParamName);
            Assert.AreEqual($"Invalid configuration.\r\nParameter name: {nameof(config.UseTimestampField)}", ex.Message);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_index_and_index_is_not_null()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultIndex = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.Index = "my_index";

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreNotEqual(config.DefaultIndex, doc.Index);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_index_is_not_null_and_doc_index_is_null()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultIndex = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultIndex, doc.Index);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_index_is_not_null_and_doc_index_is_empty()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultIndex = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.Index = string.Empty;

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultIndex, doc.Index);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_index_is_not_null_and_doc_index_is_filled_with_whitespaces()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultIndex = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.Index = "     ";

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultIndex, doc.Index);
        }


        [TestMethod]
        public void OverrideEventFields_when_default_host_and_host_is_not_null()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultIndex = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.Host = "my_host";

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreNotEqual(config.DefaultHost, doc.Host);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_host_is_not_null_and_doc_host_is_null()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultHost = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultHost, doc.Host);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_host_is_not_null_and_doc_host_is_empty()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultHost = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.Host = string.Empty;

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultHost, doc.Host);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_host_is_not_null_and_doc_host_is_filled_with_whitespaces()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultHost = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.Host = "     ";

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultHost, doc.Host);
        }


        [TestMethod]
        public void OverrideEventFields_when_default_source_and_source_is_not_null()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultIndex = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.Source = "my_source";

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreNotEqual(config.DefaultSource, doc.Source);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_source_is_not_null_and_doc_source_is_null()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultSource = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultSource, doc.Source);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_source_is_not_null_and_doc_source_is_empty()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultSource = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.Source = string.Empty;

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultSource, doc.Source);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_source_is_not_null_and_doc_source_is_filled_with_whitespaces()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultSource = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.Source = "     ";

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultSource, doc.Source);
        }


        [TestMethod]
        public void OverrideEventFields_when_default_source_type_and_source_type_is_not_null()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultIndex = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.SourceType = "my_source_type";

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreNotEqual(config.DefaultSourceType, doc.SourceType);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_source_type_is_not_null_and_doc_source_type_is_null()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultSourceType = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultSourceType, doc.SourceType);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_source_type_is_not_null_and_doc_source_type_is_empty()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultSourceType = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.SourceType = string.Empty;

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultSourceType, doc.SourceType);
        }

        [TestMethod]
        public void OverrideEventFields_when_default_source_type_is_not_null_and_doc_source_type_is_filled_with_whitespaces()
        {
            DateTimeOffset dateTime = DateTimeOffset.UtcNow;

            ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration();
            config.UseTimestampField = new TimestampFieldConfiguration();
            config.UseTimestampField.FieldName = "Timestamp";
            config.UseTimestampField.Format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            config.DefaultSourceType = "default";

            ISplunkHECDocument doc = new SplunkHECDocument();
            doc.Event = new JObject();
            doc.Event["Timestamp"] = dateTime.ToString(config.UseTimestampField.Format);
            doc.SourceType = "     ";

            SplunkHECClientMock client = new SplunkHECClientMock();
            client.OverrideEventFields(config, doc);

            Assert.AreEqual(config.DefaultSourceType, doc.SourceType);
        }

        #endregion

        #region InternalSendAsync

        [TestMethod]
        public void InternalSendAsync_when_request_is_null()
        {
            SplunkHECClientMock client = new SplunkHECClientMock();            

            AggregateException ex = Assert.ThrowsException<AggregateException>(()=>
            {
                client.InternalSendAsync(null).Wait();
            });

            ArgumentNullException internalEx = ex.InnerException as ArgumentNullException;
            Assert.AreEqual("request", internalEx.ParamName);
        }

        #endregion

        #region HealthCheck

        /// <summary>
        /// Must pass.
        /// </summary>
        [TestMethod]
        public void HealthCheck_passing_token_returning_ok()
        {
            string token = "MyToken";
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            string route = string.Format("{0}/health?token={1}", this.Configuration.Endpoint.OriginalString, token);
            mockHttp.When(route)
                .Respond("application/javascript", "{\"text\":\"HEC is healthy\",\"code\":200}");

            ISplunkHECClient client = new SplunkHECClient(this.Configuration, mockHttp);
            ISplunkHECResponse response = client.HealthCheck(token);

            Assert.AreEqual(200, response.Code);
            Assert.AreEqual("HEC is healthy", response.Text);
            Assert.AreEqual(HttpStatusCode.OK, response.HttpResponseCode);
        }

        /// <summary>
        /// Must pass.
        /// </summary>
        [TestMethod]
        public void HealthCheck_not_spassing_token_returning_ok()
        {
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            string route = string.Format("{0}/health", this.Configuration.Endpoint.OriginalString);
            mockHttp.When(route)
                .Respond("application/javascript", "{\"text\":\"HEC is healthy\",\"code\":200}");

            ISplunkHECClient client = new SplunkHECClient(this.Configuration, mockHttp);
            ISplunkHECResponse response = client.HealthCheck();

            Assert.AreEqual(200, response.Code);
            Assert.AreEqual("HEC is healthy", response.Text);
            Assert.AreEqual(HttpStatusCode.OK, response.HttpResponseCode);
        }

        #endregion
    }
}
