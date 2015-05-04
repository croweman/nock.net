using System;

namespace nock.net
{
    internal class ResponseHelper
    {
        private static readonly object LockObject = new object();

        public static ResponseDetail FindTestHttpWebResponse(HttpWebRequest request, bool remove = true)
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

                    if (remove)
                        Nock.ResponseDetails.Remove(responseDetail);

                    return responseDetail;
                }
            }

            return null;
        }

        public static TestHttpWebResponse BuildNockHttpWebResponse(ResponseDetail responseDetail)
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
    }
}
