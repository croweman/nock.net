using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Nock.net
{
    internal class Listener : IDisposable
    {
        private readonly HttpListener _listener = new HttpListener();
        private bool _disposed = false;

        public Listener(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _listener.Start();
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Nock.net: Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                Console.WriteLine("Nock.net: Url requested - " + ctx.Request.RawUrl);
                                NockHttpWebResponse webResponse = GetResponse(ctx.Request);

                                foreach (var header in webResponse.Headers.AllKeys)
                                {
                                    ctx.Response.Headers[header] = webResponse.Headers[header];
                                }
                                ctx.Response.StatusCode = webResponse.Status;

                                byte[] buf = Encoding.UTF8.GetBytes(webResponse.Body);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            finally
                            {
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { }
            });
        }

        private static TypeTo AddHeaders<TypeFrom, TypeTo>(TypeFrom headersFrom, HttpWebRequest webRequest) 
            where TypeFrom : NameValueCollection 
            where TypeTo : NameValueCollection, new()
        {
            TypeTo webHeaders = new TypeTo();

            foreach (var key in headersFrom.AllKeys)
            {
                var setValue = true;

                if (webHeaders is WebHeaderCollection && webRequest != null)
                {
                    WebHeaderCollection convertTo = webHeaders as WebHeaderCollection;

                    if (WebHeaderCollection.IsRestricted(key))
                    {
                        var currentKey = key.ToLower().Trim();
                        var lookup = new Tuple<string, string>[13];
                        lookup[0] = new Tuple<string, string>("accept", "Accept");
                        lookup[1] = new Tuple<string, string>("connection", "Connection");
                        lookup[2] = new Tuple<string, string>("content-length", "ContentLength");
                        lookup[3] = new Tuple<string, string>("content-type", "ContentType");
                        lookup[4] = new Tuple<string, string>("date", "Date");
                        lookup[5] = new Tuple<string, string>("expect", "Expect");
                        lookup[6] = new Tuple<string, string>("host", "Host");
                        lookup[7] = new Tuple<string, string>("if-modified-since", "IfModifiedSince");
                        lookup[8] = new Tuple<string, string>("range", "Range");
                        lookup[9] = new Tuple<string, string>("referrer", "Referrer");
                        lookup[10] = new Tuple<string, string>("transfer-encoding", "TransferEncoding");
                        lookup[11] = new Tuple<string, string>("user-agent", "UserAgent");
                        lookup[11] = new Tuple<string, string>("proxy-connection", "KeepAlive");

                        var match = lookup.FirstOrDefault(x => x.Item1 == currentKey);

                        if (match != null)
                        {
                            setValue = false;

                            try
                            {
                                if (headersFrom[key].ToLower() == "keep-alive")
                                {
                                    webRequest.GetType().InvokeMember("KeepAlive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder, webRequest, new object[] { true });
                                }
                                else
                                {
                                    webRequest.GetType().InvokeMember(match.Item2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder, webRequest, new object[] { headersFrom[key] });
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }
                
                if (setValue)
                {
                    try
                    {
                        if (Array.IndexOf(webHeaders.AllKeys, key) != -1)
                        {
                            webHeaders[key] = headersFrom[key];
                        }
                        else
                        {
                            webHeaders.Add(key, headersFrom[key]);
                        }
                    }
                    catch (Exception) { }
                }
            }

            return webHeaders;
        }

        private static NockHttpWebResponse GetResponse(HttpListenerRequest request)
        {
            if (nock.Recorder.IsRecording)
            {
                nock.Recorder.RecordRequest(request);
            }

            var headers = request.Headers;

            var nockedMatch = RequestMatcher.FindNockedWebResponse(new NockHttpWebRequest()
            {
                RequestUri = request.RawUrl,
                Headers = headers,
                Method = request.HttpMethod,
                InputStream = request.InputStream,
                Query = request.QueryString
            });

            NockHttpWebResponse response = new NockHttpWebResponse();
            response.Headers = new NameValueCollection();
            response.Status = 500;
            response.Body = "";

            if (nockedMatch.NockedRequest != null)
            {
                var nock = nockedMatch.NockedRequest;

                response.Status = (int)nock.StatusCode;

                if (nock.ResponseCreator != null)
                {
                    try
                    {
                        var url = request.RawUrl;

                        if (url.Contains("?"))
                        {
                            url = url.Substring(0, url.IndexOf("?"));
                        }

                        var requestDetails = new RequestDetails(url, headers, request.QueryString, nockedMatch.RequestedBody);
                        var createdResponse = nock.ResponseCreator(requestDetails);

                        response.Body = createdResponse.ResponseBody ?? string.Empty;
                        response.Headers = createdResponse.ResponseHeaders;
                    }
                    catch (Exception ex)
                    {
                        var body = string.Format("Nock.net: An error occured when making a request to '{0}', Exception: {1}, response creator delegate failed, now returning a Expectation failed status code.", request.RawUrl, ex.Message);
                        Console.WriteLine(body);
                        response.Status = (int)HttpStatusCode.ExpectationFailed;
                        byte[] buf = Encoding.UTF8.GetBytes(body);
                        response.Body = body;
                    }
                }
                else
                {
                    response.Body = nock.Response ?? string.Empty;
                    response.Headers = AddHeaders<NameValueCollection, WebHeaderCollection>(nock.ResponseHeaders, null);
                }
            }
            else
            {
                HttpWebRequest webRequest = WebRequest.Create(request.Url) as HttpWebRequest;
                webRequest.Proxy = null;

                webRequest.Headers = AddHeaders<NameValueCollection, WebHeaderCollection>(request.Headers, webRequest);

                webRequest.Method = request.HttpMethod;

                if (nock.RequestTimeoutInMilliseconds.HasValue)
                {
                    webRequest.Timeout = nock.RequestTimeoutInMilliseconds.Value;
                }

                if (!string.IsNullOrEmpty(nockedMatch.RequestedBody))
                {
                    try
                    {
                        var bytes = Encoding.UTF8.GetBytes(nockedMatch.RequestedBody);

                        using (var requestStream = webRequest.GetRequestStream())
                        {
                            requestStream.Write(bytes, 0, bytes.Length);
                            requestStream.Close();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                HttpWebResponse webResponse;

                try
                {
                    webResponse = webRequest.GetResponse() as HttpWebResponse;
                    var reader = new StreamReader(webResponse.GetResponseStream(), true);
                    var responseBody = reader.ReadToEnd();
                    response.Status = (int)webResponse.StatusCode;
                    response.Headers = AddHeaders<NameValueCollection, WebHeaderCollection>(webResponse.Headers, null);
                    response.Body = responseBody;
                }
                catch (Exception ex)
                {
                    var body = string.Format("Nock.net: An error occured when making a request to '{0}', Exception: {1}, now returning a Expectation failed status code.", request.RawUrl, ex.Message);
                    Console.WriteLine(body);
                    response.Status = (int)HttpStatusCode.ExpectationFailed;
                    byte[] buf = Encoding.UTF8.GetBytes(body);
                    response.Body = body;
                }
            }

            return response;
        }

        public void Stop()
        {
            try
            {
                if (_listener.IsListening)
                {
                    _listener.Stop();
                    _listener.Close();
                }
            }
            catch { }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Stop();
        }
    }
}