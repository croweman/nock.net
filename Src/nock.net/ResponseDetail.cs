using System;
using System.Net;

namespace Nock.net
{
    internal class ResponseDetail
    {
        public Nock.Method Method { get; private set; }
        public string Url { get; private set; }
        public string Path { get; private set; }
        public string Response { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public WebHeaderCollection Headers { get; private set; }
        public Exception Exception { get; private set; }
        public string ContentType { get; private set; }
        public TestHttpWebResponse TestHttpWebResponse { get; private set; }

        public ResponseDetail(string url, string path, string response, HttpStatusCode statusCode,
            Nock.Method method, WebHeaderCollection headers, Exception exception, string contentType,
            TestHttpWebResponse testHttpWebResponse)
        {
            Url = url;
            Path = path;
            Response = response;
            Method = method;
            StatusCode = statusCode;
            Headers = headers;
            Exception = exception;
            ContentType = contentType;
            TestHttpWebResponse = testHttpWebResponse;
        }
    }
}
