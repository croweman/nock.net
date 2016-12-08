using System;
using System.Text.RegularExpressions;

namespace Nock.net
{
    internal class UrlMatcher
    {
        public static bool IsMatch(NockHttpWebRequest webRequest, NockedRequest nockedRequest)
        {
            var requestUrl = webRequest.RequestUri;
            var nockUrl = (nockedRequest.Url + nockedRequest.Path);

            if (nockUrl.Contains("*"))
            {
                if (string.Equals(nockUrl, requestUrl))
                    return true;

                var regEx = nockUrl
                    .Replace("/", "\\/")
                    .Replace(".", "\\.")
                    .Replace("*", "[^ ]*")
                    .Replace("?", "\\?");

                try
                {
                    return Regex.IsMatch(requestUrl, "^" + regEx + "$");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Nock.net: An error occurred while trying to match the url based on wildcards: " + ex.Message);
                }

                return false;
            }
            else
            {
                return string.Equals(nockUrl, requestUrl);
            }
        }
    }
}
