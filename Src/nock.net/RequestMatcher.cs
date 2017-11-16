using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace Nock.net
{
    internal class RequestMatcher
    {

        private static readonly object LockObject = new object();

        public static NockMatch FindNockedWebResponse(NockHttpWebRequest request)
        {
            NockMatch nockMatch = new NockMatch();
            
            if (!nock.Testing || nock.NockedRequests.Count == 0)
                return nockMatch;

            try
            {
                var reader = new StreamReader(request.InputStream, true);
                var requestedBody = reader.ReadToEnd();
                nockMatch.RequestedBody = requestedBody;
            }
            catch (Exception) { }

            lock (LockObject)
            {
                foreach (var nockedRequest in nock.NockedRequests)
                {
                    var checkUrlResult = CheckUrl(request, nockedRequest);
                    var checkMethodResult = CheckMethod(request, nockedRequest);
                    var checkHeadersResult = CheckHeaders(request, nockedRequest);
                    var checkQueryResult = CheckQuery(request, nockedRequest);
                    var checkBodyResult = CheckBody(request, nockedRequest, nockMatch.RequestedBody);

                    if (!checkUrlResult || !checkMethodResult || !checkHeadersResult || !checkQueryResult || !checkBodyResult)
                    {
                        Log(nockedRequest, "Nock has not been matched :S");
                        continue;
                    }

                    Log(nockedRequest, "Nock has been matched :)");
                    nockedRequest.Times--;
                    Log(nockedRequest, string.Format("Nocked request Times has been decremented to: {0}", nockedRequest.Times));

                    if (nockedRequest.Times < 1)
                    {
                        nockedRequest.IsDone = true;
                        nock.NockedRequests.Remove(nockedRequest);
                    }

                    nockMatch.NockedRequest = nockedRequest;
                    return nockMatch;
                }
            }

            return nockMatch;
        }

        private static bool CheckUrl(NockHttpWebRequest request, NockedRequest nockedRequest)
        {
            var requestUrl = request.RequestUri;
            var nockUrl = (nockedRequest.Url + nockedRequest.Path);

            Log(nockedRequest, string.Format("Trying to match requested url '{0}' against nocked url '{1}'", requestUrl, nockUrl));
            var match = UrlMatcher.IsMatch(request, nockedRequest);
            Log(nockedRequest, "Url match: " + match);
            return match;
        }

        private static bool CheckMethod(NockHttpWebRequest request, NockedRequest nockedRequest)
        {
            Log(nockedRequest, string.Format("Trying to match requested method '{0}' against nocked method '{1}'", request.Method, nockedRequest.Method.ToString()));
            var match = (string.Equals(nockedRequest.Method.ToString(), request.Method, StringComparison.OrdinalIgnoreCase));
            Log(nockedRequest, "Method match: " + match);
            return match;
        }

        private static bool CheckHeaders(NockHttpWebRequest request, NockedRequest nockedRequest)
        {
            if (nockedRequest.HeaderMatcher == HeaderMatcher.None)
                return true;

            if (!RequestHeadersMatch(nockedRequest, nockedRequest.RequestHeaders, request.Headers))
                return false;

            if (nockedRequest.HeaderMatcher == HeaderMatcher.ExactMatch && nockedRequest.RequestHeaders.Count != request.Headers.Count)
            {
                Log(nockedRequest, "Exact header match is enabled and the number of nocked request headers and actual request headers did not match");
                return false;
            }

            if (nockedRequest.RequestHeaderMatcher != null)
            {
                try
                {
                    var match = nockedRequest.RequestHeaderMatcher(request.Headers);
                    Log(nockedRequest, "Custom header matcher result: " + match);
                    return match;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Nock.net: An error occurred while trying to check the request headers: " + ex.Message);
                    Log(nockedRequest, "An error occurred while trying to check the request headers: " + ex.Message);
                    Log(nockedRequest, "Custom header matcher result: false");
                    return false;
                }
            }

            Log(nockedRequest, "Header matcher result: true");
            return true;
        }

        private static bool CheckQuery(NockHttpWebRequest request, NockedRequest nockedRequest)
        {
            if (request.Query.Count == 0 && nockedRequest.QueryMatcher == QueryMatcher.None)
                return true;

            if (request.Query.Count > 0 && !request.RequestUri.Contains("?") && nockedRequest.QueryMatcher == QueryMatcher.None)
            {
                Log(nockedRequest, "A query string has been specified in the request url, but query matcher has not been defined");
                Log(nockedRequest, "Query matcher result: false");
                return false;
            }

            if (nockedRequest.QueryMatcher == QueryMatcher.None)
                return true;

            if (nockedRequest.QueryMatcher == QueryMatcher.NameValue || nockedRequest.QueryMatcher == QueryMatcher.NameValueExact)
            {
                if (!QueryStringMatch(nockedRequest, nockedRequest.Query, request.Query))
                    return false;

                if (nockedRequest.QueryMatcher == QueryMatcher.NameValueExact && nockedRequest.Query.Count != request.Query.Count)
                {
                    Log(nockedRequest, "Exact query match is enabled and the number of nocked request query keys and actual request query keys did not match");
                    return false;
                }
            }

            if (nockedRequest.QueryMatcher == QueryMatcher.Bool)
            {
                Log(nockedRequest, "Query matcher result: " + nockedRequest.QueryResult);
                return nockedRequest.QueryResult;
            }

            if (nockedRequest.QueryFunc != null)
            {
                try
                {
                    var url = request.RequestUri;

                    if (url.Contains("?"))
                    {
                        url = url.Substring(0, url.IndexOf("?"));
                    }

                    var match = nockedRequest.QueryFunc(new QueryDetails(url, request.Query));
                    Log(nockedRequest, "Query matcher result: " + match);
                    return match;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Nock.net: An error occurred while trying to check the query: " + ex.Message);
                    Log(nockedRequest, "An error occurred while trying to check the query: " + ex.Message);
                    Log(nockedRequest, "Custom query matcher result: false");
                    return false;
                }
            }

            Log(nockedRequest, "Query matcher result: true");
            return true;
        }

        private static bool CheckBody(NockHttpWebRequest request, NockedRequest nockedRequest, string body)
        {
            if (nockedRequest.BodyMatcher != BodyMatcher.None)
            {
                switch (nockedRequest.BodyMatcher)
                {
                    case BodyMatcher.Body:
                        if (!string.Equals(nockedRequest.Body, body))
                        {
                            Log(nockedRequest, string.Format("The requested body '{0}' does not match the nocked request body '{1}'", body, nockedRequest.Body));
                            return false;
                        }
                        break;
                    case BodyMatcher.StringFunc:
                        try
                        {
                            var match = nockedRequest.BodyMatcherString(body);
                            Log(nockedRequest, "Custom body matcher result: " + match);
                            return match;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Nock.net: An error occurred while trying to check the request body: " + ex.Message);
                            Log(nockedRequest, "An error occurred while trying to check the request body: " + ex.Message);
                            Log(nockedRequest, "Body matcher result: false");
                            return false;
                        }
                    case BodyMatcher.TypedFunc:
                        var property = nockedRequest.BodyMatcherFunc.GetType().GetProperty("Method");
                        var value = property.GetValue(nockedRequest.BodyMatcherFunc);
                        var methodInfo = (System.Reflection.MethodInfo)value;
                        object obj = null;

                        try
                        {
                            obj = JsonConvert.DeserializeObject(body, nockedRequest.BodyMatcherType);
                        }
                        catch (Exception ex)
                        {
                            obj = null;
                            Console.WriteLine("Nock.net: The response body could not be deserialized into type: " + nockedRequest.BodyMatcherType.Name + ". " + ex.Message);
                            Log(nockedRequest, "Nock.net: The response body could not be deserialized into type: " + nockedRequest.BodyMatcherType.Name + ". " + ex.Message);
                        }

                        bool success = false;

                        try
                        {
                            success = (bool)methodInfo.Invoke(null, new object[] { obj });
                        }
                        catch (Exception ex)
                        {
                            obj = null;
                            Console.WriteLine("Nock.net: An error occurred while trying to check the request body: " + ex.Message);
                            Log(nockedRequest, "An error occurred while trying to check the request body: " + ex.Message);
                            Log(nockedRequest, "Body matcher result: false");
                        }

                        if (!success)
                        {
                            Log(nockedRequest, "Body matcher result: false");
                            return false;
                        }
                        break;
                }
            }

            Log(nockedRequest, "Body matcher result: true");
            return true;
        }

        private static bool RequestHeadersMatch(NockedRequest nockedRequest, NameValueCollection requiredRequestHeaders, NameValueCollection requestHeaders)
        {
            var matched = true;

            foreach (var requiredKey in requiredRequestHeaders.AllKeys)
            {
                var requiredValue = requiredRequestHeaders[requiredKey];
                Log(nockedRequest, string.Format("Trying to match header '{0}'", requiredKey));

                var matchingKey = requestHeaders.AllKeys.FirstOrDefault(x => string.Equals(requiredKey, x));

                if (string.IsNullOrEmpty(matchingKey))
                {
                    Log(nockedRequest, string.Format("Header '{0}' could not be found", requiredKey));
                    matched = false;
                    break;
                }

                if (!string.Equals(requiredValue, requestHeaders[requiredKey]))
                {
                    Log(nockedRequest, string.Format("Request header value '{0}' did not match nocked request value '{1}'.", requestHeaders[requiredKey], requiredValue));
                    matched = false;
                    break;
                }
                else
                {
                    Log(nockedRequest, string.Format("Request header value '{0}' matched nocked request value '{1}'.", requestHeaders[requiredKey], requiredValue));
                }
            }

            Log(nockedRequest, "Request headers matched: " + matched);
            return matched;
        }

        private static bool QueryStringMatch(NockedRequest nockedRequest, NameValueCollection requiredQueryHeaders, NameValueCollection queryHeaders)
        {
            var matched = true;

            foreach (var requiredKey in requiredQueryHeaders.AllKeys)
            {
                var requiredValue = requiredQueryHeaders[requiredKey];
                Log(nockedRequest, string.Format("Trying to match query '{0}'", requiredKey));

                var matchingKey = queryHeaders.AllKeys.FirstOrDefault(x => string.Equals(requiredKey, x));

                if (string.IsNullOrEmpty(matchingKey))
                {
                    Log(nockedRequest, string.Format("Query '{0}' could not be found", requiredKey));
                    matched = false;
                    break;
                }

                if (!string.Equals(requiredValue, queryHeaders[requiredKey]))
                {
                    Log(nockedRequest, string.Format("Query header value '{0}' did not match nocked request value '{1}'.", queryHeaders[requiredKey], requiredValue));
                    matched = false;
                    break;
                }
                else
                {
                    Log(nockedRequest, string.Format("Query header value '{0}' matched nocked request value '{1}'.", queryHeaders[requiredKey], requiredValue));
                }
            }

            Log(nockedRequest, "Query headers matched: " + matched);
            return matched;
        }

        private static void Log(NockedRequest nockedRequest, string message)
        {
            if (nockedRequest.Logger == null)
                return;

            try
            {
                nockedRequest.Logger("Nock.net: " + message);
            }
            catch (Exception)
            {

            }
        }
    }
}
