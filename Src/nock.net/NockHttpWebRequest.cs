using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace Nock.net
{
    class NockHttpWebRequest
    {
        public  NockHttpWebRequest()
        {
            Headers = new NameValueCollection();
        }

        public string RequestUri { get; set; }
        public string Method { get; set; }
        public NameValueCollection Headers { get; set; }
        public Stream InputStream { get; set; }
    }
}
