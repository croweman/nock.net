using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace Nock.net
{
    public sealed class Nock
    {
        internal static readonly List<ResponseDetail> ResponseDetails = new List<ResponseDetail>();
        internal static bool Testing;

        private ResponseDetail _responseDetail;
        private bool _built;

        private readonly string _url;
        private string _path;
        private Method _method;
        private string _contentType;
        private string _body;
        private WebHeaderCollection _requestHeaders = new WebHeaderCollection();

        public enum Method
        {
            GET,
            POST,
            PUT,
            DELETE,
            HEAD,
            PATCH,
            MERGE
        }

        public Nock(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Url must be defined");

            if (url.EndsWith("/"))
                throw new ArgumentException("The url must not end with a '/'");

            _url = url;
            Testing = true;
        }

        private Nock SetMethod(string path, Method method, string body = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path must be defined");

            if (!path.StartsWith("/"))
                throw new ArgumentException("Path must start with a '/'");
                
            _path = path;
            _method = method;
            _body = body;

            return this;
        }

        public Nock Get(string path, string body = null)
        {
            return SetMethod(path, Method.GET, body);
        }

        public Nock Post(string path, string body = null)
        {
            return SetMethod(path, Method.POST, body);
        }

        public Nock Put(string path, string body = null)
        {
            return SetMethod(path, Method.PUT, body);
        }

        public Nock Delete(string path, string body = null)
        {
            return SetMethod(path, Method.DELETE, body);
        }

        public Nock Head(string path)
        {
            return SetMethod(path, Method.HEAD);
        }

        public Nock Patch(string path)
        {
            return SetMethod(path, Method.PATCH);
        }

        public Nock Merge(string path)
        {
            return SetMethod(path, Method.MERGE);
        }

        public Nock ContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                throw new ArgumentException("Content type must be defined");

            _contentType = contentType;
            return this;
        }

        public Nock Reply(HttpStatusCode statusCode, string response, WebHeaderCollection headers = null)
        {
            if (_built)
                throw new Exception("The nock has already been built");

            if (string.IsNullOrEmpty(_path))
                throw new ArgumentException("Path must be defined");

            if (headers == null)
                headers = new WebHeaderCollection();

            if (response == null)
                response = string.Empty;

            _responseDetail = new ResponseDetail(_url, _path, response, statusCode, _method, headers, null, _contentType, null, _body, _requestHeaders);
             ResponseDetails.Add(_responseDetail);
            _built = true;

            return this;
        }

        public Nock Reply<T>(T exception) where T : Exception
        {
            if (_built)
                throw new Exception("The nock has already been built");

            if (exception == null)
                throw new ArgumentException("The exception must be defined");

            if (string.IsNullOrEmpty(_path))
                throw new ArgumentException("Path must be defined");

            _responseDetail = new ResponseDetail(_url, _path, "", HttpStatusCode.OK, _method, null, exception, _contentType, null, _body, _requestHeaders);
            ResponseDetails.Add(_responseDetail);
            _built = true;

            return this;
        }

        public Nock Reply(TestHttpWebResponse testHttpWebResponse)
        {
            if (_built)
                throw new Exception("The nock has already been built");

            if (testHttpWebResponse == null)
                throw new ArgumentException("The nock response must be defined");

            if (string.IsNullOrEmpty(_path))
                throw new ArgumentException("Path must be defined");

            _responseDetail = new ResponseDetail(_url, _path, "", HttpStatusCode.OK, _method, null, null, _contentType, testHttpWebResponse, _body, _requestHeaders);
            ResponseDetails.Add(_responseDetail);
            _built = true;

            return this;
        }

        public Nock RequestHeaders(WebHeaderCollection headers)
        {
            if (headers == null)
                throw new ArgumentException("Request headers must be defined");

            _requestHeaders = headers;
            return this;
        }

        public void Times(int numberOfTimes)
        {
            if (!_built)
                throw new Exception("You have not called a valid action method e.g. get, post and the Reply method");

            if (numberOfTimes < 2)
                throw new ArgumentException("Number of times must be greater than 1");

            for (var i = 1; i < numberOfTimes; i++)
            {
                ResponseDetails.Add(_responseDetail);
            }
        }

        public static void ClearAll()
        {
            ResponseDetails.Clear();
        }

    }
}
