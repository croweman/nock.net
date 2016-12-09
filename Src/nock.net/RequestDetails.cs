using System.Collections.Specialized;

namespace Nock.net
{
    public class RequestDetails
    {
        public string Url { get; internal set; }
        public NameValueCollection Headers { get; internal set; }
        public NameValueCollection Query { get; internal set; }
        public string Body { get; internal set; }

        public RequestDetails(string url, NameValueCollection headers, NameValueCollection query, string body)
        {
            if (headers == null)
                headers = new NameValueCollection();

            if (query == null)
                query = new NameValueCollection();

            Url = url;
            Headers = headers;
            Query = query;
            Body = body;
        }
    }
}
