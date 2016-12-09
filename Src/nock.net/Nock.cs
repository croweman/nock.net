using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

namespace Nock.net
{
    public sealed class nock
    {

        private static readonly object LockObject = new object();
        private static Recorder _recorder = new Recorder();

        internal static readonly List<NockedRequest> NockedRequests = new List<NockedRequest>();
        internal static bool Testing;

        private NockedRequest _nockedRequest;
        private bool _built;

        private readonly string _url;
        private string _path;
        private Method _method = Method.NotSet;
        private string _body;
        private Action<string> _logger;

        private HeaderMatcher _headerMatcher = HeaderMatcher.None;
        private NameValueCollection _requestHeaders = new NameValueCollection();
        private Func<NameValueCollection, bool> _requestHeadersMatcher;

        private BodyMatcher _bodyMatcher = BodyMatcher.None;
        private Func<string, bool> _bodyMatcherString;

        private object _bodyMatcherFunc;
        private Type _bodyMatcherType;

        private QueryMatcher _queryMatcher = QueryMatcher.None;
        private bool _queryResult;
        private NameValueCollection _query;
        private Func<QueryDetails, bool> _queryFunc;

        private Func<RequestDetails, WebResponse> _responseCreator;

        private static Listener _listener;
        private static WebProxy _webProxy;

        private static bool _createWebProxy = true;
        private static bool _setDefaultCredentials = true;
        private static int? _requestTimeoutInMilliseconds = null;

        public static Recorder Recorder
        {
            get
            {
                return _recorder;
            }
        }

        public static int? RequestTimeoutInMilliseconds
        {
            get
            {
                return _requestTimeoutInMilliseconds;
            }
            set
            {
                _requestTimeoutInMilliseconds = value;
            }
        }

        public static bool CreateWebProxy
        {
            get
            {
                return _createWebProxy;
            }
            set
            {
                _createWebProxy = value;
            }
        }

        public static bool SetDefaultCredentials
        {
            get
            {
                return _setDefaultCredentials;
            }
            set
            {
                _setDefaultCredentials = value;
            }
        }

        public nock(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Url must be defined");

            if (url.EndsWith("/"))
                throw new ArgumentException("The url must not end with a '/'");

            _url = url;

            lock (LockObject)
            {
                if (Testing)
                    return;

                Testing = true;
                _listener = new Listener(new string[] { "http://*:8080/" });

                _listener.Run();

                if (!CreateWebProxy)
                    return;

                _webProxy = new WebProxy("localhost", 8080);
                _webProxy.BypassProxyOnLocal = false;
                _webProxy.Credentials = CredentialCache.DefaultCredentials;
                WebRequest.DefaultWebProxy = _webProxy;
            }         
        }


        private nock SetMethod(string path, Method method, string body = null, Func<string, bool> bodyMatcherString = null, Type bodyMatcherType = null, object bodyMatcherFunc = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path must be defined");

            if (!path.StartsWith("/"))
                throw new ArgumentException("Path must start with a '/'");

            if (_method != Method.NotSet)
                throw new Exception("The method of the nock has already been set!");
                
            _path = path;
            _method = method;

            _body = body;
            _bodyMatcherString = bodyMatcherString;
            _bodyMatcherType = bodyMatcherType;
            _bodyMatcherFunc = bodyMatcherFunc;

            if (_body != null)
            {
                _bodyMatcher = BodyMatcher.Body;
            }
            else if (_bodyMatcherString != null)
            {
                IsCorrectlyReferencedFunction(_bodyMatcherString);
                _bodyMatcher = BodyMatcher.StringFunc;
            }
            else if (_bodyMatcherType != null)
            {
                IsCorrectlyReferencedFunction(_bodyMatcherFunc);
                _bodyMatcher = BodyMatcher.TypedFunc;
            }

            return this;
        }

        public nock Get(string path)
        {
            return SetMethod(path, Method.GET);
        }

        public nock Get(string path, string body = null)
        {
            return SetMethod(path, Method.GET, body);
        }

        public nock Post(string path)
        {
            return SetMethod(path, Method.POST);
        }

        public nock Post(string path, string body = null)
        {
            return SetMethod(path, Method.POST, body);
        }

        public nock Post(string path, Func<string, bool> bodyMatcher)
        {
            return SetMethod(path, Method.POST, null, bodyMatcher);
        }

        public nock Post<T>(string path, Func<T, bool> bodyMatcher)
        {
            return SetMethod(path, Method.POST, null, null, typeof(T), bodyMatcher);
        }

        public nock Put(string path)
        {
            return SetMethod(path, Method.PUT);
        }

        public nock Put(string path, string body = null)
        {
            return SetMethod(path, Method.PUT, body);
        }

        public nock Put(string path, Func<string, bool> bodyMatcher)
        {
            return SetMethod(path, Method.PUT, null, bodyMatcher);
        }

        public nock Put<T>(string path, Func<T, bool> bodyMatcher)
        {
            return SetMethod(path, Method.PUT, null, null, typeof(T), bodyMatcher);
        }

        public nock Delete(string path)
        {
            return SetMethod(path, Method.DELETE);
        }
        public nock Delete(string path, string body = null)
        {
            return SetMethod(path, Method.DELETE, body);
        }

        public nock Head(string path)
        {
            return SetMethod(path, Method.HEAD);
        }

        public nock Patch(string path)
        {
            return SetMethod(path, Method.PATCH);
        }

        public nock Patch(string path, string body = null)
        {
            return SetMethod(path, Method.PATCH, body);
        }

        public nock Patch(string path, Func<string, bool> bodyMatcher)
        {
            return SetMethod(path, Method.PATCH, null, bodyMatcher);
        }

        public nock Patch<T>(string path, Func<T, bool> bodyMatcher)
        {
            return SetMethod(path, Method.PATCH, null, null, typeof(T), bodyMatcher);
        }

        public nock Merge(string path)
        {
            return SetMethod(path, Method.MERGE);
        }

        public nock Merge(string path, string body = null)
        {
            return SetMethod(path, Method.MERGE, body);
        }

        public nock Merge(string path, Func<string, bool> bodyMatcher)
        {
            return SetMethod(path, Method.MERGE, null, bodyMatcher);
        }

        public nock Merge<T>(string path, Func<T, bool> bodyMatcher)
        {
            return SetMethod(path, Method.MERGE, null, null, typeof(T), bodyMatcher);
        }

        public nock ContentType(string contentType)
        {
            return MatchHeader("Content-Type", contentType);
        }

        public nock UserAgent(string userAgent)
        {
            return MatchHeader("User-Agent", userAgent);
        }

        public nock Referer(string referrer)
        {
            return MatchHeader("Referer", referrer);
        }

        public nock MatchHeader(string headerName, string headerValue)
        {
            if (string.IsNullOrEmpty(headerName))
                throw new ArgumentException("Header name must be defined");

            if (headerValue == null)
                throw new ArgumentException("Header value must be defined");

            if (_headerMatcher == HeaderMatcher.None)
                _headerMatcher = HeaderMatcher.Match;

            _requestHeaders.Add(headerName, headerValue);
            return this;
        }

        public nock MatchHeaders(NameValueCollection headers, bool exact = false)
        {
            if (headers == null)
                throw new ArgumentException("Request headers must be defined");

            if (exact)
                _headerMatcher = HeaderMatcher.ExactMatch;
            else
                _headerMatcher = HeaderMatcher.Match;

            _requestHeaders = headers;
            return this;
        }

        public nock MatchHeaders(Func<NameValueCollection, bool> requestHeadersMatcher)
        {
            if (requestHeadersMatcher == null)
                throw new ArgumentException("Request headers matcher function must be defined");

            IsCorrectlyReferencedFunction(requestHeadersMatcher);

            _headerMatcher = HeaderMatcher.Match;
            _requestHeadersMatcher = requestHeadersMatcher;
            return this;
        }

        public nock Query(bool result)
        {
            _queryMatcher = QueryMatcher.Bool;
            _queryResult = result;
            return this;
        }

        public nock Query(NameValueCollection queryParameters, bool exactMatch = false)
        {
            _queryMatcher = exactMatch == true ? QueryMatcher.NameValueExact : QueryMatcher.NameValue;
            _query = queryParameters;
            return this;
        }

        public nock Query(Func<QueryDetails, bool> queryMatcher)
        {
            _queryMatcher = QueryMatcher.Func;
            _queryFunc = queryMatcher;
            return this;
        }

        public nock Reply(HttpStatusCode statusCode, Func<RequestDetails, WebResponse> responseCreator)
        {
            if (responseCreator == null)
                throw new ArgumentException("Response creator function is invalid");

            IsCorrectlyReferencedFunction(responseCreator);

            _responseCreator = responseCreator;

            return Reply(statusCode, string.Empty, new NameValueCollection());
        }

        public nock Reply(HttpStatusCode statusCode, string response, NameValueCollection headers = null)
        {
            if (_built)
                throw new Exception("The nock has already been built");

            if (string.IsNullOrEmpty(_path))
                throw new ArgumentException("Path must be defined");

            if (headers == null)
                headers = new NameValueCollection();

            if (response == null)
                response = string.Empty;

            _nockedRequest = new NockedRequest(_url);
            _nockedRequest.Path = _path;
            _nockedRequest.Response = response;
            _nockedRequest.StatusCode = statusCode;
            _nockedRequest.Method = _method;
            _nockedRequest.Body = _body;
            _nockedRequest.BodyMatcher = _bodyMatcher;
            _nockedRequest.BodyMatcherString = _bodyMatcherString;
            _nockedRequest.BodyMatcherFunc = _bodyMatcherFunc;
            _nockedRequest.BodyMatcherType = _bodyMatcherType;
            _nockedRequest.ResponseHeaders = headers;
            _nockedRequest.HeaderMatcher = _headerMatcher;
            _nockedRequest.RequestHeaders = _requestHeaders;
            _nockedRequest.RequestHeaderMatcher = _requestHeadersMatcher;
            _nockedRequest.ResponseCreator = _responseCreator;

            _nockedRequest.Query = _query;
            _nockedRequest.QueryFunc = _queryFunc;
            _nockedRequest.QueryMatcher = _queryMatcher;
            _nockedRequest.QueryResult = _queryResult;

            _nockedRequest.Times = 1;
            _nockedRequest.Logger = _logger;
            NockedRequests.Add(_nockedRequest);
            _built = true;

            return this;
        }

        public nock Times(int numberOfTimes)
        {
            if (!_built)
                throw new Exception("You have not called a valid action method e.g. Get, Post and the Reply method");

            if (numberOfTimes < 2)
                throw new ArgumentException("Number of times must be greater than 1");

            _nockedRequest.Times = numberOfTimes;
            return this;
        }

        public nock Log(Action<string> logger)
        {
            if (logger == null)
                throw new ArgumentException("A logger must be defined");

            IsCorrectlyReferencedFunction(logger);

            _logger = logger;

            return this;
        }

        public bool Done()
        {
            return _nockedRequest.IsDone && _nockedRequest.Times == 0;
        }

        public static void ClearAll()
        {
            NockedRequests.Clear();
        }

        public static void RemoveInterceptor(nock nock)
        {
            NockedRequests.Remove(nock._nockedRequest);
        }

        public static void Stop()
        {
            lock (LockObject)
            {
                if (Testing)
                {
                    if (_listener != null)
                        _listener.Stop();

                    Testing = false;
                }
            }
        }

        private void IsCorrectlyReferencedFunction(object function)
        {
            var property = function.GetType().GetProperty("Method");
            var value = property.GetValue(function);
            var methodInfo = (System.Reflection.MethodInfo)value;

            if (methodInfo.ToString().Contains("<"))
                return;

            if (!methodInfo.IsStatic)
                throw new ArgumentException("The defined function is not static!");
        }

    }
}
