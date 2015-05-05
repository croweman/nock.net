using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Nock.net
{
    public sealed class HttpWebRequest : IHttpWebRequest
    {
        private readonly System.Net.HttpWebRequest _httpWebRequest;
        internal string Body;

        private CustomMemoryStream _requestStream;

        public HttpWebRequest(System.Net.HttpWebRequest httpWebRequest)
        {
            _httpWebRequest = httpWebRequest;
        }

        public static HttpWebRequest CreateRequest(string url)
        {
            return new HttpWebRequest((System.Net.HttpWebRequest)WebRequest.Create(url));
        }

        public static HttpWebRequest CreateRequest(Uri uri)
        {
            return new HttpWebRequest((System.Net.HttpWebRequest)WebRequest.Create(uri));
        }

        public string Accept
        {
            get { return _httpWebRequest.Accept; }
            set { _httpWebRequest.Accept = value; }
        }

        public Uri Address
        {
            get { return _httpWebRequest.Address; }
        }

        public bool AllowAutoRedirect
        {
            get { return _httpWebRequest.AllowAutoRedirect; }
            set { _httpWebRequest.AllowAutoRedirect = value; }
        }

        public bool AllowReadStreamBuffering
        {
            get { return _httpWebRequest.AllowReadStreamBuffering; }
            set { _httpWebRequest.AllowReadStreamBuffering = value; }
        }

        public bool AllowWriteStreamBuffering
        {
            get { return _httpWebRequest.AllowWriteStreamBuffering; }
            set { _httpWebRequest.AllowWriteStreamBuffering = value; }
        }

        public DecompressionMethods AutomaticDecompression
        {
            get { return _httpWebRequest.AutomaticDecompression; }
            set { _httpWebRequest.AutomaticDecompression = value; }
        }

        public X509CertificateCollection ClientCertificates
        {
            get { return _httpWebRequest.ClientCertificates; }
            set { _httpWebRequest.ClientCertificates = value; }
        }

        public string Connection
        {
            get { return _httpWebRequest.Connection; }
            set { _httpWebRequest.Connection = value; }
        }

        public string ConnectionGroupName
        {
            get { return _httpWebRequest.ConnectionGroupName; }
            set { _httpWebRequest.ConnectionGroupName = value; }
        }

        public long ContentLength
        {
            get { return _httpWebRequest.ContentLength; }
            set { _httpWebRequest.ContentLength = value; }
        }

        public string ContentType
        {
            get { return _httpWebRequest.ContentType; }
            set { _httpWebRequest.ContentType = value; }
        }

        public HttpContinueDelegate ContinueDelegate
        {
            get { return _httpWebRequest.ContinueDelegate; }
            set { _httpWebRequest.ContinueDelegate = value; }
        }

        public int ContinueTimeout
        {
            get { return _httpWebRequest.ContinueTimeout; }
            set { _httpWebRequest.ContinueTimeout = value; }
        }

        public CookieContainer CookieContainer
        {
            get { return _httpWebRequest.CookieContainer; }
            set { _httpWebRequest.CookieContainer = value; }
        }

        public ICredentials Credentials
        {
            get { return _httpWebRequest.Credentials; }
            set { _httpWebRequest.Credentials = value; }
        }

        public DateTime Date
        {
            get { return _httpWebRequest.Date; }
            set { _httpWebRequest.Date = value; }
        }

        public string Expect
        {
            get { return _httpWebRequest.Expect; }
            set { _httpWebRequest.Expect = value; }
        }

        public bool HaveResponse
        {
            get { return _httpWebRequest.HaveResponse; }
        }

        public WebHeaderCollection Headers
        {
            get { return _httpWebRequest.Headers; }
            set { _httpWebRequest.Headers = value; }
        }

        public string Host
        {
            get { return _httpWebRequest.Host; }
            set { _httpWebRequest.Host = value; }
        }

        public DateTime IfModifiedSince 
        {
            get { return _httpWebRequest.IfModifiedSince; }
            set { _httpWebRequest.IfModifiedSince = value; }
        }

        public bool KeepAlive
        {
            get { return _httpWebRequest.KeepAlive; }
            set { _httpWebRequest.KeepAlive = value; }
        }

        public int MaximumAutomaticRedirections
        {
            get { return _httpWebRequest.MaximumAutomaticRedirections; }
            set { _httpWebRequest.MaximumAutomaticRedirections = value; }
        }

        public int MaximumResponseHeadersLength
        {
            get { return _httpWebRequest.MaximumResponseHeadersLength; }
            set { _httpWebRequest.MaximumResponseHeadersLength = value; }
        }

        public string MediaType
        {
            get { return _httpWebRequest.MediaType; }
            set { _httpWebRequest.MediaType = value; }
        }

        public string Method
        {
            get { return _httpWebRequest.Method; }
            set { _httpWebRequest.Method = value; }
        }

        public bool Pipelined
        {
            get { return _httpWebRequest.Pipelined; }
            set { _httpWebRequest.Pipelined = value; }
        }

        public bool PreAuthenticate
        {
            get { return _httpWebRequest.PreAuthenticate; }
            set { _httpWebRequest.PreAuthenticate = value; }
        }

        public Version ProtocolVersion
        {
            get { return _httpWebRequest.ProtocolVersion; }
            set { _httpWebRequest.ProtocolVersion = value; }
        }

        public IWebProxy Proxy
        {
            get { return _httpWebRequest.Proxy; }
            set { _httpWebRequest.Proxy = value; }
        }

        public int ReadWriteTimeout
        {
            get { return _httpWebRequest.ReadWriteTimeout; }
            set { _httpWebRequest.ReadWriteTimeout = value; }
        }

        public string Referer
        {
            get { return _httpWebRequest.Referer; }
            set { _httpWebRequest.Referer = value; }
        }

        public Uri RequestUri
        {
            get { return _httpWebRequest.RequestUri; }
        }

        public bool SendChunked
        {
            get { return _httpWebRequest.SendChunked; }
            set { _httpWebRequest.SendChunked = value; }
        }

        public RemoteCertificateValidationCallback ServerCertificateValidationCallback
        {
            get { return _httpWebRequest.ServerCertificateValidationCallback; }
            set { _httpWebRequest.ServerCertificateValidationCallback = value; }
        }

        public ServicePoint ServicePoint
        {
            get { return _httpWebRequest.ServicePoint; }
        }

        public bool SupportsCookieContainer
        {
            get { return _httpWebRequest.SupportsCookieContainer; }
        }

        public int Timeout
        {
            get { return _httpWebRequest.Timeout; }
            set { _httpWebRequest.Timeout = value; }
        }

        public string TransferEncoding
        {
            get { return _httpWebRequest.TransferEncoding; }
            set { _httpWebRequest.TransferEncoding = value; }
        }

        public bool UnsafeAuthenticatedConnectionSharing
        {
            get { return _httpWebRequest.UnsafeAuthenticatedConnectionSharing; }
            set { _httpWebRequest.UnsafeAuthenticatedConnectionSharing = value; }
        }

        public bool UseDefaultCredentials
        {
            get { return _httpWebRequest.UseDefaultCredentials; }
            set { _httpWebRequest.UseDefaultCredentials = value; }
        }

        public string UserAgent
        {
            get { return _httpWebRequest.UserAgent; }
            set { _httpWebRequest.UserAgent = value; }
        }

        public void Abort()
        {
            _httpWebRequest.Abort();
        }

        public void AddRange(int range)
        {
            _httpWebRequest.AddRange(range);
        }

        public void AddRange(long range)
        {
            _httpWebRequest.AddRange(range);
        }

        public void AddRange(int @from, int to)
        {
            _httpWebRequest.AddRange(from, to);
        }

        public void AddRange(long @from, long to)
        {
            _httpWebRequest.AddRange(from, to);
        }

        public void AddRange(string rangeSpecifier, int range)
        {
            _httpWebRequest.AddRange(rangeSpecifier, range);
        }

        public void AddRange(string rangeSpecifier, long range)
        {
            _httpWebRequest.AddRange(rangeSpecifier, range);
        }

        public void AddRange(string rangeSpecifier, int @from, int to)
        {
            _httpWebRequest.AddRange(rangeSpecifier, from, to);
        }

        public void AddRange(string rangeSpecifier, long @from, long to)
        {
            _httpWebRequest.AddRange(rangeSpecifier, from, to);
        }

        public IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            return _httpWebRequest.BeginGetRequestStream(callback, state);
        }

        public IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return _httpWebRequest.BeginGetResponse(callback, state);
        }

        public Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return _httpWebRequest.EndGetRequestStream(asyncResult);
        }

        public Stream EndGetRequestStream(IAsyncResult asyncResult, out TransportContext context)
        {
            return _httpWebRequest.EndGetRequestStream(asyncResult, out context);
        }

        public IHttpWebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            return new HttpWebResponse((System.Net.HttpWebResponse)_httpWebRequest.EndGetResponse(asyncResult));
        }

        public Stream GetRequestStream()
        {
            var response = ResponseHelper.FindTestHttpWebResponse(this, false, true);

            if (response == null)
                return _httpWebRequest.GetRequestStream();

            return _requestStream ?? (_requestStream = new CustomMemoryStream(SetRequestBody));
        }

        public Stream GetRequestStream(out TransportContext context)
        {
            return _httpWebRequest.GetRequestStream(out context);
        }

        public IHttpWebResponse GetResponse()
        {
            var responseDetail = ResponseHelper.FindTestHttpWebResponse(this);

            if (responseDetail != null)
                return ResponseHelper.BuildNockHttpWebResponse(responseDetail);

            return new HttpWebResponse((System.Net.HttpWebResponse)_httpWebRequest.GetResponse());
        }

        internal void SetRequestBody(string body)
        {
            Body = body;
        }

    }
}
