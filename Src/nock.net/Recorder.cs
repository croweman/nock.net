using System;
using System.Net;
using System.Text;

namespace Nock.net
{

    public class Recorder
    {
        private StringBuilder _data = new StringBuilder();
        private bool _recording = false;
        private bool _output = false;

        public bool IsRecording
        {
            get
            {
                return _recording;
            }
        }

        public void Record(bool output = true)
        {
            _recording = true;
            _output = output;
        }

        public string GetRecording()
        {
            return _data.ToString();
        }

        public void Stop()
        {
            _recording = false;
        }

        public void Clear()
        {
            _data.Clear();
        }

        internal void RecordRequest(HttpListenerRequest request)
        {
            var requestDetails = new StringBuilder();

            var url = request.RawUrl;
            var path = url;

            var index = url.IndexOf("://");

            if (index != -1)
            {
                index = index + 3;

                if (url.Substring(index).Contains("/"))
                    index += url.Substring(index).IndexOf("/");

                url = url.Substring(0, index);
            }

            if (path.Contains("://"))
                path = path.Substring(path.IndexOf("://") + 3);

            if (path.Contains("/"))
                path = path.Substring(path.IndexOf("/"));

            if (string.IsNullOrEmpty(path))
                path = "/";

            var method = request.HttpMethod.ToLower();
            method = method.Substring(0, 1).ToUpper() + method.Substring(1);

            requestDetails.AppendLine();
            requestDetails.AppendLine(string.Format("var nockedRequest = new nock(\"{0}\")", url));
            requestDetails.AppendLine(string.Format("\t.{0}(\"{1}\")", method, path));

            if (request.Headers != null && request.Headers.Count > 0)
            { 
                var headersToIgnore = new string[] { "host", "proxy-connection", "content-length", "expect" };

                foreach (string key in request.Headers.AllKeys)
                {
                    if (Array.IndexOf(headersToIgnore, key.ToLower()) != -1)
                        continue;

                    requestDetails.AppendLine(string.Format("\t.MatchHeader(\"{0}\", \"{1}\")", key, request.Headers[key]));
                }
            }

            if (request.QueryString != null && request.QueryString.Count > 0)
            {
                requestDetails.AppendLine(string.Format("\t.Query(true)"));
            }

            requestDetails.AppendLine(string.Format("\t.Reply(HttpStatusCode.OK, \"Response Body\");"));
            _data.Append(requestDetails.ToString());

            if (_output)
            {
                Console.WriteLine(requestDetails.ToString());
            }
        }
    }
}
