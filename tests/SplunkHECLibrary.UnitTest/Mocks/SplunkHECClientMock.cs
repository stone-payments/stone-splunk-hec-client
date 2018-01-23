using StoneCo.SplunkHECLibrary;
using StoneCo.SplunkHECLibrary.DataContracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StoneCo.LapiSplunkConsumer.UnitTest.Mocks
{
    public class SplunkHECClientMock : SplunkHECClient
    {
        public new void OverrideEventFields(ISplunkHECClientConfiguration config, ISplunkHECDocument doc) => base.OverrideEventFields(config, doc);

        public new async Task<HttpResponseMessage> InternalSendAsync(ISplunkHECRequest request) => await base.InternalSendAsync(request);
    }
}
