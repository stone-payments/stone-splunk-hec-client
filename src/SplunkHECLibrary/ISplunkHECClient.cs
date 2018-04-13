using StoneCo.SplunkHECLibrary.DataContracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoneCo.SplunkHECLibrary
{
    public delegate void RequestEventHandler(object sender, ISplunkHECRequest request);

    public delegate void ResponseEventHandler(object sender, ISplunkHECResponse response);

    public interface ISplunkHECClient
    { 
        event RequestEventHandler BeforeSend;

        event ResponseEventHandler AfterSend;

        Task<ISplunkHECResponse> HealthCheckAsync(string token = "");

        ISplunkHECResponse HealthCheck(string token = "");

        ISplunkHECResponse Send(ISplunkHECRequest request);

        Task<ISplunkHECResponse> SendAsync(ISplunkHECRequest request);
    }
}
