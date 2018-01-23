using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StoneCo.SplunkHECLibrary.DataContracts;

namespace StoneCo.SplunkHECLibrary
{
    /// <summary>
    /// A client for Splunk Http Events Collector.
    /// </summary>
    public class SplunkHECClient : ISplunkHECClient
    {

        #region Private properties

        /// <summary>
        /// Http Client.
        /// </summary>
        private HttpClient Client { get; set; }

        /// <summary>
        /// The client configuration.
        /// </summary>
        private ISplunkHECClientConfiguration Configuration { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Event raised before send the request.
        /// </summary>
        public event RequestEventHandler BeforeSend;

        /// <summary>
        /// Event raised after receive the response.
        /// </summary>
        public event ResponseEventHandler AfterSend;

        #endregion

        #region Constructors

        /// <summary>
        /// Base constructor.
        /// </summary>
        public SplunkHECClient()
        {

        }

        /// <summary>
        /// Constructor with configuration and http message handler.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="handler">The http message handler.</param>
        public SplunkHECClient(ISplunkHECClientConfiguration config, HttpMessageHandler handler = null)
        {

            if (handler == null)
            {                               
                this.Client = new HttpClient();
            }
            else
            {
                this.Client = new HttpClient(handler);
            }

            this.Configuration = config ?? throw new ArgumentNullException(nameof(config));
            this.Client.BaseAddress = config.Endpoint;
            this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Splunk", config.Token);
        }

        /// <summary>
        /// Constructor with http message handler.
        /// </summary>
        /// <param name="handler">The http message handler.</param>
        public SplunkHECClient(HttpMessageHandler handler)
        {
            #region Validations

            if(handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            #endregion

            this.Client = new HttpClient(handler);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Override doc field values according to a configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="doc">The document.</param>
        protected virtual void OverrideEventFields(ISplunkHECClientConfiguration config, ISplunkHECDocument doc)
        {
            if (config.UseTimestampField != null)
            {

                if (doc.Event.TryGetValue(config.UseTimestampField.FieldName, out JToken value) == false)
                {
                    throw new ArgumentException("The timestamp field name don't exists on event.", nameof(config.UseTimestampField));
                }

                try
                {

                    JsonSerializerSettings Settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DateFormatString = config.UseTimestampField.Format,
                        DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                        DateParseHandling = DateParseHandling.DateTimeOffset
                    };

                    JsonSerializer Serializer = JsonSerializer.Create(Settings);

                    JToken token = doc.Event.GetValue(config.UseTimestampField.FieldName);
                    DateTimeOffset dateTime = token.ToObject<DateTimeOffset>(Serializer);

                    doc.Time = dateTime.ToUnixTimeMilliseconds();
                }catch
                {
                    throw new ArgumentException("Invalid configuration.", nameof(config.UseTimestampField));
                }              
            }

            if ((string.IsNullOrWhiteSpace(config.DefaultIndex) == false) && string.IsNullOrWhiteSpace(doc.Index))
            {
                doc.Index = config.DefaultIndex;
            }

            if ((string.IsNullOrWhiteSpace(config.DefaultHost) == false) && string.IsNullOrWhiteSpace(doc.Host))
            {
                doc.Host = config.DefaultHost;
            }

            if ((string.IsNullOrWhiteSpace(config.DefaultSource) == false) && string.IsNullOrWhiteSpace(doc.Source))
            {
                doc.Source = config.DefaultSource;
            }

            if ((string.IsNullOrWhiteSpace(config.DefaultSourceType) == false) && string.IsNullOrWhiteSpace(doc.SourceType))
            {
                doc.SourceType = config.DefaultSourceType;
            }
        }

        /// <summary>
        /// Send the request to Splunk HEC asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A Task that will returns the http response.</returns>
        protected virtual async Task<HttpResponseMessage> InternalSendAsync(ISplunkHECRequest request)
        {
            #region Validations

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            #endregion

            // Override events fields values according the configuration.
            Parallel.ForEach(request.Items, doc =>
            {
                this.OverrideEventFields(this.Configuration, doc);
            });
            
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, this.Configuration.Endpoint.AbsolutePath);            
            httpRequest.Content = new StringContent(request.Serialize(), Encoding.UTF8, "application/json");            
            return await this.Client.SendAsync(httpRequest);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Send the request to Splunk HEC synchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        public ISplunkHECResponse Send(ISplunkHECRequest request)
        {
            Task<ISplunkHECResponse> response = this.SendAsync(request);
            return response.Result;
        }

        /// <summary>
        /// Send the request to Splunk HEC asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        public async Task<ISplunkHECResponse> SendAsync(ISplunkHECRequest request)
        {
            return await Task.Run<ISplunkHECResponse>(async () =>
            {
                ISplunkHECResponse response;

                this.BeforeSend?.Invoke(this, request);
                HttpResponseMessage httpResponse = await InternalSendAsync(request);

                response = new SplunkHECResponse();                
                string strResponse = httpResponse.Content.ReadAsStringAsync().Result;
                if(string.IsNullOrWhiteSpace(strResponse) == false)
                {
                    response = JsonConvert.DeserializeObject<SplunkHECResponse>(strResponse);
                }

                response.HttpResponseCode = httpResponse.StatusCode;

                this.AfterSend?.Invoke(this, response);
                return response;
            });
            
        }

        #endregion
    }
}
