using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Nock.net
{
    internal class ResponseHelper
    {
        private static readonly object LockObject = new object();

        public static ResponseDetail FindTestHttpWebResponse(HttpWebRequest request, bool remove = true, bool findingForRequest = false)
        {
            if (!Nock.Testing || Nock.ResponseDetails.Count == 0)
                return null;

            lock (LockObject)
            {
                var url = request.RequestUri.ToString();
                var method = request.Method.ToUpper();
                var contentType = request.ContentType;

                foreach (var responseDetail in Nock.ResponseDetails)
                {
                    var fullUrl = responseDetail.Url + responseDetail.Path;

                    if (!string.Equals(fullUrl, url, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!string.Equals(responseDetail.Method.ToString(), method, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!string.IsNullOrWhiteSpace(responseDetail.ContentType) && !string.Equals(responseDetail.ContentType, contentType, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!RequestHeadersMatch(responseDetail.RequestHeaders, request.Headers))
                        continue;

                    if (!findingForRequest && !string.IsNullOrWhiteSpace(responseDetail.Body))
                    {
                        var body = request.Body;

                        if (!string.IsNullOrWhiteSpace(body) && !string.Equals(responseDetail.Body, body))
                            continue;
                    }

                    if (remove)
                    {
                        responseDetail.IsDone = true;
                        Nock.ResponseDetails.Remove(responseDetail);
                    }

                    return responseDetail;
                }
            }

            return null;
        }

        public static TestHttpWebResponse FindAndBuildTestHttpWebResponse(HttpWebRequest request)
        {
            var responseDetail = FindTestHttpWebResponse(request);
            return responseDetail != null ? BuildNockHttpWebResponse(responseDetail) : null;
        }

        internal static TestHttpWebResponse BuildNockHttpWebResponse(ResponseDetail responseDetail)
        {
            if (responseDetail.TestHttpWebResponse != null)
                return responseDetail.TestHttpWebResponse;

            if (responseDetail.Exception != null)
                throw responseDetail.Exception;

            var response = new TestHttpWebResponse(responseDetail.Response)
            {
                StatusCode = responseDetail.StatusCode,
                ContentType = responseDetail.ContentType,
                Headers = responseDetail.Headers
            };

            return response;
        }

        private static bool RequestHeadersMatch(WebHeaderCollection requiredRequestHeaders, WebHeaderCollection requestHeaders)
        {
            var matched = true;

            foreach (var requiredKey in requiredRequestHeaders.AllKeys)
            {
                var requiredValue = requiredRequestHeaders[requiredKey];

                var matchingKey = requestHeaders.AllKeys.FirstOrDefault(x => string.Equals(requiredKey, x, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrWhiteSpace(matchingKey) || !string.Equals(requiredValue, requestHeaders[requiredKey]))
                {
                    matched = false;
                    break;
                }
            }

            return matched;
        }
    }
}
