using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Nock.net.Tests
{
    [TestFixture]
    public class AcceptanceTests
    {

        [SetUp]
        public void SetUp()
        {
            Nocker.ClearAll();
        }

        [Test]
        public void CallingDoneOnANockReturnsFalseIfTheNockResponseWasNotUsed()
        {
            var nock = new Nocker("https://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body");

            var request = HttpWebRequest.CreateRequest("https://domain-name.com/api/v2/action/");
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";

            Assert.That(nock.Done(), Is.False); 
        }

        [Test]
        public void CallingDoneOnANockReturnsTrueIfTheNockResponseWasUsed()
        {
            var nock = new Nocker("https://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body");

            var request = HttpWebRequest.CreateRequest("https://domain-name.com/api/v2/action/");
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";

            var response = request.GetResponse();

            Assert.That(nock.Done(), Is.True);
        }

        [TestCase(HttpStatusCode.OK, "Added", Status.OK)]
        [TestCase(HttpStatusCode.OK, "User not allowed", Status.Forbidden)]
        [TestCase(HttpStatusCode.OK, "User could not be found", Status.NotFound)]
        [TestCase(HttpStatusCode.ServiceUnavailable, "Something went wrong", Status.Error)]
        [TestCase(null, "WebException", Status.Error)]
        [Test]
        public void NockingAResponseCorrectlyReturnsRelevantResponses(HttpStatusCode? statusCode, string resultMessage, Status expectedStatus)
        {
            var responseJson = string.Format("{{ result: \"{0}\" }}", resultMessage);

            if (resultMessage == "WebException")
            {
                var testHttpWebResponse = new CustomHttpWebResponse();
                testHttpWebResponse.Headers.Add("Status-Code", "403");

                new Nocker("https://domain-name.com")
                    .ContentType("application/json; encoding='utf-8'")
                    .Post("/api/v2/action/")
                    .Reply(new WebException("This is a web exception", null, WebExceptionStatus.UnknownError, testHttpWebResponse));
            }
            else
            {
                new Nocker("https://domain-name.com")
                    .ContentType("application/json; encoding='utf-8'")
                    .Post("/api/v2/action/")
                    .Reply(HttpStatusCode.OK, responseJson);                
            }

            var postData = PostDataToAnEndpointAndProcessTheResponse();

            Assert.That(postData.Status, Is.EqualTo(expectedStatus));

            if (resultMessage == "WebException")
            {
                Assert.That(postData.ErrorMessage, Is.EqualTo("An error occurred: This is a web exception. Http status code: 403"));
            }

        }

        public PostResult PostDataToAnEndpointAndProcessTheResponse()
        {
            var postResult = new PostResult
            {
                Status = Status.OK
            };

            var postData =
                "{" +
                    "Action: \"AddFunds\"," +
                    "FirstName: \"Joe\"," +
                    "Surname: \"Bloggs\"" +
                    "Amount: 50.95" + 
                "}";

            var bytes = Encoding.UTF8.GetBytes(postData);

            var request = HttpWebRequest.CreateRequest("https://domain-name.com/api/v2/action/");
            request.ContentType = "application/json; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            IHttpWebResponse response = null;

            try
            {
                response = request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var body = string.Empty;

                    using (var reader = new StreamReader(response.GetResponseStream(), true))
                    {
                        body = reader.ReadToEnd();
                    }

                    var model = JsonConvert.DeserializeObject<ResponseModel>(body);

                    switch (model.Result)
                    {
                        case "Added":
                            postResult.Status = Status.OK;
                            break;
                        case "User not allowed":
                            postResult.Status = Status.Forbidden;
                            break;
                        case "User could not be found":
                            postResult.Status = Status.NotFound;
                            break;
                        default:
                            postResult.Status = Status.Error;
                            break;
                    }
                }
                else
                    postResult.Status = Status.Error;
   
            }
            catch (WebException ex)
            {
                postResult.Status = Status.Error;

                var statusCode = "Unknown";

                if (ex.Response != null)
                    statusCode = ex.Response.Headers["Status-Code"];

                postResult.ErrorMessage = string.Format("An error occurred: {0}. Http status code: {1}", ex.Message, statusCode);
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }

            return postResult;
        }

        public enum Status
        {
            OK,
            Forbidden,
            NotFound,
            Error
        }

        public class ResponseModel
        {
            public string Result { get; set; }
        }

        public class CustomHttpWebResponse : WebResponse
        {
            private readonly WebHeaderCollection _headers = new WebHeaderCollection();

            public override WebHeaderCollection Headers
            {
                get
                {
                    return _headers;
                }
            }
        }

        public class PostResult
        {
            public Status Status { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
