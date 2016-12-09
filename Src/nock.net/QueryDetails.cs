using System.Collections.Specialized;

namespace Nock.net
{
    public class QueryDetails
    {
        public NameValueCollection Query { get; internal set; }
        public string RequestUrl { get; internal set; }

        public QueryDetails(string requestUrl, NameValueCollection query)
        {
            if (query == null)
                query = new NameValueCollection();

            Query = query;
            RequestUrl = requestUrl;
        }
    }
}
