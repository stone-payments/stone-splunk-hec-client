using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoneCo.SplunkHECLibrary.DataContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.SplunkHECLibrary.UnitTest
{
    [TestClass]
    public class SplunkHECClientConfigurationTest
    {

        private IConfigurationBuilder Builder { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Builder = new ConfigurationBuilder();
        }

        [TestMethod]
        public void Constructor_with_config_success()
        {
            IDictionary<string, string> dict = new Dictionary<string, string>
            {
                { "Splunk:Endpoint", "http://localhost:8888/endpoint" },
                { "Splunk:Token", "mytoken" },
                { "Splunk:DefaultSourceType", "sourcetype" }
            };

            IConfigurationRoot configRoot = Builder
                .AddInMemoryCollection(dict)
                .Build();
            IConfigurationSection config = Builder.Build().GetSection("Splunk");

            SplunkHECClientConfiguration splConfig = new SplunkHECClientConfiguration(config);
        }

        [TestMethod]
        public void Constructor_with_config_without_endpoint()
        {
            IDictionary<string, string> dict = new Dictionary<string, string>
            {
                { "Splunk:Token", "mytoken" },
                { "Splunk:DefaultSourceType", "sourcetype" }
            };

            IConfigurationRoot configRoot = Builder
                .AddInMemoryCollection(dict)
                .Build();
            IConfigurationSection config = Builder.Build().GetSection("Splunk");

            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(()=>
            {
                SplunkHECClientConfiguration splConfig = new SplunkHECClientConfiguration(config);
            });

            Assert.AreEqual("Endpoint", ex.ParamName);
        }

        [TestMethod]
        public void Constructor_with_config_with_invalid_endpoint()
        {
            IDictionary<string, string> dict = new Dictionary<string, string>
            {
                { "Splunk:Endpoint", "fdsfsfdsfsfsd" },
                { "Splunk:Token", "mytoken" },
                { "Splunk:DefaultSourceType", "sourcetype" }
            };

            IConfigurationRoot configRoot = Builder
                .AddInMemoryCollection(dict)
                .Build();
            IConfigurationSection config = Builder.Build().GetSection("Splunk");

            ArgumentException ex = Assert.ThrowsException<ArgumentException>(() =>
            {
                SplunkHECClientConfiguration splConfig = new SplunkHECClientConfiguration(config);
            });

            Assert.AreEqual("Endpoint", ex.ParamName);
            Assert.AreEqual("Please inform a valid absolute Uri.\r\nParameter name: Endpoint", ex.Message);
        }

        [TestMethod]
        public void Constructor_with_config_without_sourcetype()
        {
            IDictionary<string, string> dict = new Dictionary<string, string>
            {
                { "Splunk:Endpoint", "http://localhost:8888/endpoint" },
                { "Splunk:Token", "mytoken" }
            };

            IConfigurationRoot configRoot = Builder
                .AddInMemoryCollection(dict)
                .Build();
            IConfigurationSection config = Builder.Build().GetSection("Splunk");

            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                SplunkHECClientConfiguration splConfig = new SplunkHECClientConfiguration(config);
            });

            Assert.AreEqual("DefaultSourceType", ex.ParamName);
        }

        [TestMethod]
        public void Constructor_with_config_with_empty_sourcetype()
        {
            IDictionary<string, string> dict = new Dictionary<string, string>
            {
                { "Splunk:Endpoint", "http://localhost:8888/endpoint" },
                { "Splunk:Token", "mytoken" },
                { "Splunk:DefaultSourceType", "" }
            };

            IConfigurationRoot configRoot = Builder
                .AddInMemoryCollection(dict)
                .Build();
            IConfigurationSection config = Builder.Build().GetSection("Splunk");

            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                SplunkHECClientConfiguration splConfig = new SplunkHECClientConfiguration(config);
            });

            Assert.AreEqual("DefaultSourceType", ex.ParamName);
        }

        [TestMethod]
        public void Constructor_with_config_without_token()
        {
            IDictionary<string, string> dict = new Dictionary<string, string>
            {
                { "Splunk:Endpoint", "http://localhost:8888/endpoint" },
                { "Splunk:DefaultSourceType", "sourcetype" }
            };

            IConfigurationRoot configRoot = Builder
                .AddInMemoryCollection(dict)
                .Build();
            IConfigurationSection config = Builder.Build().GetSection("Splunk");

            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                SplunkHECClientConfiguration splConfig = new SplunkHECClientConfiguration(config);
            });

            Assert.AreEqual("Token", ex.ParamName);
        }

    }
}
