using System.Collections.Specialized;
using System.Net;

namespace Nock.net
{
    public class WebResponse
    {
        public NameValueCollection ResponseHeaders { get; private set; }
        public string ResponseBody { get; private set; }

        public WebResponse(NameValueCollection responseHeaders, string responseBody)
        {
            if (responseHeaders == null)
                responseHeaders = new NameValueCollection();

            if (responseBody == null)
                responseBody = string.Empty;

            ResponseHeaders = responseHeaders;
            ResponseBody = responseBody;
        }
    }
}
