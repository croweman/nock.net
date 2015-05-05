using System;
using System.IO;
using System.Net;

namespace Nock.net
{
    public class TestHttpWebResponse : IHttpWebResponse, IDisposable
    {
        private readonly string _responseBody;        
        private MemoryStream _stream;
        private StreamWriter _writer;
        private bool _disposed;

        public TestHttpWebResponse(string responseBody)
        {
            _responseBody = responseBody;
            ContentLength = responseBody.Length;
            StatusCode = HttpStatusCode.OK;
        }

        public virtual string CharacterSet { get; set; }
        
        public virtual string ContentEncoding { get; set; }
        
        public virtual long ContentLength { get; set; }
        
        public virtual string ContentType { get; set; }
        
        public virtual CookieCollection Cookies { get; set; }
        
        public virtual WebHeaderCollection Headers { get; set; }
        
        public virtual bool IsMutuallyAuthenticated { get; set; }
        
        public virtual DateTime LastModified { get; set; }
        
        public virtual string Method { get; set; }
        
        public virtual Version ProtocolVersion { get; set; }
        
        public virtual Uri ResponseUri { get; set; }
        
        public virtual string Server { get; set; }
        
        public virtual HttpStatusCode StatusCode { get; set; }
        
        public virtual string StatusDescription { get; set; }
        
        public virtual bool SupportsHeaders { get; set; }
        
        public virtual void Close()
        {
            if (_stream != null)
                _stream.Close();

            if (_writer != null)
                _writer.Close();
        }

        public virtual string GetResponseHeader(string headerName)
        {
            return Headers[headerName] ?? string.Empty;
        }

        public virtual Stream GetResponseStream()
        {
            _stream = new MemoryStream();
            _writer = new StreamWriter(_stream);
            _writer.Write(_responseBody);
            _writer.Flush();
            _stream.Position = 0;
            return _stream;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (_stream != null)
                _stream.Dispose();

            if (_writer != null)
                _writer.Dispose();

            _disposed = true;
        }
    }
}
