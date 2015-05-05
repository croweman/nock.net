using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Nock.net
{
    public interface IHttpWebRequest
    {
        string Accept { get; set; }
        Uri Address { get; }
        bool AllowAutoRedirect { get; set; }
        bool AllowReadStreamBuffering { get; set; }
        bool AllowWriteStreamBuffering { get; set; }
        DecompressionMethods AutomaticDecompression { get; set; }
        X509CertificateCollection ClientCertificates { get; set; }
        string Connection { get; set; }
        string ConnectionGroupName { get; set; }
        long ContentLength { get; set; }
        string ContentType { get; set; }
        HttpContinueDelegate ContinueDelegate { get; set; }
        int ContinueTimeout { get; set; }
        CookieContainer CookieContainer { get; set; }
        ICredentials Credentials { get; set; }
        DateTime Date { get; set; }
        string Expect { get; set; }
        bool HaveResponse { get; }
        WebHeaderCollection Headers { get; set; }
        string Host { get; set; }
        DateTime IfModifiedSince { get; set; }
        bool KeepAlive { get; set; }
        int MaximumAutomaticRedirections { get; set; }
        int MaximumResponseHeadersLength { get; set; }
        string MediaType { get; set; }
        string Method { get; set; }
        bool Pipelined { get; set; }
        bool PreAuthenticate { get; set; }
        Version ProtocolVersion { get; set; }
        IWebProxy Proxy { get; set; }
        int ReadWriteTimeout { get; set; }
        string Referer { get; set; }
        Uri RequestUri { get; }
        bool SendChunked { get; set; }
        RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
        ServicePoint ServicePoint { get; }
        bool SupportsCookieContainer { get; }
        int Timeout { get; set; }
        string TransferEncoding { get; set; }
        bool UnsafeAuthenticatedConnectionSharing { get; set; }
        bool UseDefaultCredentials { get; set; }
        string UserAgent { get; set; }
        void Abort();
        void AddRange(int range);
        void AddRange(long range);
        void AddRange(int from, int to);
        void AddRange(long from, long to);
        void AddRange(string rangeSpecifier, int range);
        void AddRange(string rangeSpecifier, long range);
        void AddRange(string rangeSpecifier, int from, int to);
        void AddRange(string rangeSpecifier, long from, long to);
        IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state);
        IAsyncResult BeginGetResponse(AsyncCallback callback, object state);
        Stream EndGetRequestStream(IAsyncResult asyncResult);
        Stream EndGetRequestStream(IAsyncResult asyncResult, out TransportContext context);
        IHttpWebResponse EndGetResponse(IAsyncResult asyncResult);
        Stream GetRequestStream();
        Stream GetRequestStream(out TransportContext context);
        IHttpWebResponse GetResponse();
    }
}
