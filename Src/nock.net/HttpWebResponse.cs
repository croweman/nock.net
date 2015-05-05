using System;
using System.IO;
using System.Net;

namespace Nock.net
{
    public class HttpWebResponse : IHttpWebResponse, IDisposable
    {
        private readonly System.Net.HttpWebResponse _httpWebResponse;
        private bool _disposed;

        public HttpWebResponse(System.Net.HttpWebResponse httpWebResponse)
        {
            _httpWebResponse = httpWebResponse;
        }

        public string CharacterSet
        {
            get { return _httpWebResponse.CharacterSet; }
        }

        public string ContentEncoding
        {
            get { return _httpWebResponse.ContentEncoding; }
        }

        public long ContentLength
        {
            get { return _httpWebResponse.ContentLength; }
        }

        public string ContentType
        {
            get { return _httpWebResponse.ContentType; }
        }

        public CookieCollection Cookies
        {
            get { return _httpWebResponse.Cookies; }
            set { _httpWebResponse.Cookies = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return _httpWebResponse.Headers; }
        }

        public bool IsMutuallyAuthenticated
        {
            get { return _httpWebResponse.IsMutuallyAuthenticated; }
        }

        public DateTime LastModified
        {
            get { return _httpWebResponse.LastModified; }
        }

        public string Method
        {
            get { return _httpWebResponse.Method; }
        }

        public Version ProtocolVersion
        {
            get { return _httpWebResponse.ProtocolVersion; }
        }

        public Uri ResponseUri
        {
            get { return _httpWebResponse.ResponseUri; }
        }

        public string Server
        {
            get { return _httpWebResponse.Server; }
        }

        public HttpStatusCode StatusCode
        {
            get { return _httpWebResponse.StatusCode; }
        }

        public string StatusDescription
        {
            get { return _httpWebResponse.StatusDescription; }
        }

        public bool SupportsHeaders
        {
            get { return _httpWebResponse.SupportsHeaders; }
        }

        public void Close()
        {
            _httpWebResponse.Close();
        }

        public string GetResponseHeader(string headerName)
        {
            return _httpWebResponse.GetResponseHeader(headerName);
        }

        public Stream GetResponseStream()
        {
            return _httpWebResponse.GetResponseStream();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _httpWebResponse.Dispose();
            _disposed = true;
        }
    }
}
