using System.Net;

namespace StoneCo.SplunkHECLibrary.DataContracts
{
    public interface ISplunkHECResponse
    {
        HttpStatusCode HttpResponseCode { get; set; }

        int Code { get; set; }

        int InvalidEventNumber { get; set; }

        string Text { get; set; }
    }
}