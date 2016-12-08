using System.Collections.Specialized;

namespace Nock.net
{
    class NockHttpWebResponse
    {
        public int Status { get; set; }
        public NameValueCollection Headers { get; set; }
        public string Body { get; set; }
    }
}
