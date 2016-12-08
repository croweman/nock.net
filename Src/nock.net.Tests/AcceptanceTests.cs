using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Specialized;

namespace Nock.net.Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        [OneTimeTearDown]
        public void TearDown()
        {
            nock.Stop();
        }

        [SetUp]
        public void SetUp()
        {
           nock.ClearAll();
        }

        [Test]
        public void CallingDoneOnANockReturnsFalseIfTheNockResponseWasNotUsed()
        {
            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action2/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";

            System.Net.WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException err)
            {
                Assert.AreEqual(err.Message, "The remote server returned an error: (417) Expectation Failed.");
            }

            Assert.That(nock.Done(), Is.False);
        }

        [Test]
        public void CallingDoneOnANockReturnsTrueIfTheNockResponseWasUsed()
        {

            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Get("/api/v2/action/")
                .Log(Console.WriteLine)
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";
            System.Net.WebResponse response = request.GetResponse();

            Assert.That(nock.Done(), Is.True);
        }

        [Test]
        public void CallingDoneOnANockReturnsTrueIfTheNockResponseWasUsedWhenUsingWebClient()
        {

            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body");


            var client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json; encoding='utf-8'";
            var body = client.DownloadString("http://domain-name.com/api/v2/action/");

            Assert.That(nock.Done(), Is.True);
            Assert.That(body, Is.EqualTo("The body"));
        }

        [Test]
        public void WhenANockHasBeenDefinedForOneRequestAndTwoRequestsAreMadeTheSecondWillFail()
        {

            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";
            System.Net.WebResponse response = request.GetResponse();

            Assert.That(nock.Done(), Is.True);

            var errorMessage = "";
            try
            {
                request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
                request.ContentType = "application/json; encoding='utf-8'";
                request.Method = "GET";
                response = request.GetResponse();
            }
            catch (WebException ex)
            {
                errorMessage = ex.Message;
            }

            Assert.That(errorMessage, Is.EqualTo("The remote server returned an error: (417) Expectation Failed."));
        }

        [Test]
        public void WhenANockHasBeenDefinedForTwoRequestsAndTwoRequestsAreMadeBothWillPass()
        {

            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body")
                .Times(2);

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";
            System.Net.WebResponse response = request.GetResponse();
            var bodyOne = ReadResponseBody(response);

            request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";
            response = request.GetResponse();
            var bodyTwo = ReadResponseBody(response);

            Assert.That(nock.Done(), Is.True);
            Assert.That(bodyOne, Is.EqualTo("The body"));
            Assert.That(bodyTwo, Is.EqualTo("The body"));
        }

        [Test]
        public void WhenANockHasBeenDefinedWithWildcardUrlForTwoRequestsAndTwoRequestsAreMadeBothWillPass()
        {

            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Get("/*/?location=true")
                .Reply(HttpStatusCode.OK, (url, headers, body) =>
                {
                    url = url.Substring(0, url.IndexOf("?"));
                    var responseHeaders = new NameValueCollection();
                    responseHeaders.Add("Location", url);

                    return new WebResponse(responseHeaders, "{ \"src\": \"" + url + "\" }");
                })
                .Times(2);

            var request = WebRequest.Create("http://domain-name.com/one/?location=true") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";
            System.Net.WebResponse response = request.GetResponse();
            var bodyOne = ReadResponseBody(response);
            Assert.That(nock.Done(), Is.False);
            Assert.That(bodyOne, Is.EqualTo("{ \"src\": \"http://domain-name.com/one/\" }"));
            Assert.That(response.Headers["Location"], Is.EqualTo("http://domain-name.com/one/"));


            request = WebRequest.Create("http://domain-name.com/two/?location=true") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";
            response = request.GetResponse();
            var bodyTwo = ReadResponseBody(response);

            Assert.That(nock.Done(), Is.True);
            Assert.That(bodyTwo, Is.EqualTo("{ \"src\": \"http://domain-name.com/two/\" }"));
            Assert.That(response.Headers["Location"], Is.EqualTo("http://domain-name.com/two/"));
        }

        [Test]
        public void WhenANockHasBeenDefinedForTwoRequestsButOnlyOneRequestHasMadeCallingDoneWillReturnFalse()
        {
            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body")
                .Times(2);

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";
            System.Net.WebResponse response = request.GetResponse();

            Assert.That(nock.Done(), Is.False);
        }

        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnStringHeaderFilters()
        {
            var headers = new NameValueCollection();
            headers.Add("fish", "chips");
            headers.Add("peas", "beans");

            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .MatchHeaders(headers)
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Headers.Add("peas", "beans");
            request.Headers.Add("fish", "chips");
            request.Method = "GET";
            System.Net.WebResponse response = request.GetResponse();
        
            Assert.That(nock.Done(), Is.True);
        }

        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnFunctionHeaderFiltersIfFunctionReturnsTrue()
        {
            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .MatchHeaders((headers) => { return headers["fish"] == "chips"; })
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Headers.Add("fish", "chips");
            request.Method = "GET";
            System.Net.WebResponse response = request.GetResponse();

            Assert.That(nock.Done(), Is.True);
        }

        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnFunctionHeaderFiltersIfFunctionReturnsFalse()
        {
            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .MatchHeaders((headers) => { return headers["fish"] == "gravy"; })
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Headers.Add("fish", "chips");
            request.Method = "GET";

            Assert.That(nock.Done(), Is.False);

            var errorMessage = "";
            try
            {
                request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
                request.ContentType = "application/json; encoding='utf-8'";
                request.Method = "GET";
                var response = request.GetResponse();
            }
            catch (WebException ex)
            {
                errorMessage = ex.Message;
            }

            Assert.That(errorMessage, Is.EqualTo("The remote server returned an error: (417) Expectation Failed."));
        }

        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnFunctionHeaderFiltersThatThrowAnException()
        {
            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .MatchHeaders(headers => { throw new WebException("Oh no"); })
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Headers.Add("fish", "chips");
            request.Method = "GET";

            Assert.That(nock.Done(), Is.False);

            var errorMessage = "";
            try
            {
                request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
                request.ContentType = "application/json; encoding='utf-8'";
                request.Method = "GET";
                var response = request.GetResponse();
            }
            catch (WebException ex)
            {
                errorMessage = ex.Message;
            }

            Assert.That(errorMessage, Is.EqualTo("The remote server returned an error: (417) Expectation Failed."));
        }

        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnStringHeaderFiltersExactMatch()
        {
            var headers = new NameValueCollection();
            headers.Add("fish", "chips");
            headers.Add("peas", "beans");

            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .MatchHeaders(headers, true)
                .Get("/api/v2/action/")
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Headers.Add("peas", "beans");
            request.Headers.Add("fish", "chips");
            request.Method = "GET";

            var errorMessage = "";
            try
            {
                var response = request.GetResponse();
            }
            catch (WebException ex)
            {
                errorMessage = ex.Message;
            }

            Assert.That(nock.Done(), Is.False);
            Assert.That(errorMessage, Is.EqualTo("The remote server returned an error: (417) Expectation Failed."));
        }


        public class TestObj
        {
            public string Action { get; set; }
            public string FirstName { get; set; }
            public string Surname { get; set; }
            public decimal Amount { get; set; }
        }

        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnStringBodyFilters()
        {
            var postData =
                "{" +
                    "Action: \"AddFunds\"," +
                    "FirstName: \"Joe\"," +
                    "Surname: \"Bloggs\"" +
                    "Amount: 50.95" +
                "}";

            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Post("/api/v2/action/", postData)
                .Reply(HttpStatusCode.OK, "The body");

            var bytes = Encoding.UTF8.GetBytes(postData);

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            System.Net.WebResponse response = request.GetResponse();

            Assert.That(nock.Done(), Is.True);
        }

        private static bool BodyMatcherTyped(TestObj testObj)
        {
            return testObj.Action == "AddFunds" &&
                testObj.FirstName == "Joe" &&
                testObj.Surname == "Bloggs" &&
                testObj.Amount == (decimal)50.95;
        }

        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnTypedFunctionBodyFilters()
        {

            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Post<TestObj>("/api/v2/action/", BodyMatcherTyped)
                .Reply(HttpStatusCode.OK, "The body");

            var postData = "{\"Action\":\"AddFunds\",\"FirstName\":\"Joe\",\"Surname\":\"Bloggs\",\"Amount\":50.95}";
            var bytes = Encoding.UTF8.GetBytes(postData);

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            System.Net.WebResponse response = request.GetResponse();

            Assert.That(nock.Done(), Is.True);
        }

        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnFunctionBodyFiltersIfFunctionReturnsTrue()
        {
            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Post("/api/v2/action/", (body) => { return true; })
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Headers.Add("fish", "chips");
            request.Method = "POST";

            var postData = "{\"Action\":\"AddFunds\",\"FirstName\":\"Joe\",\"Surname\":\"Bloggs\",\"Amount\":50.95}";
            var bytes = Encoding.UTF8.GetBytes(postData);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            System.Net.WebResponse response = request.GetResponse();

            Assert.That(nock.Done(), Is.True);
        }

        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnFunctionBodyFiltersIfFunctionReturnsFalse()
        {
            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Post("/api/v2/action/", (body) => { return false; })
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Headers.Add("fish", "chips");
            request.Method = "POST";

            var postData = "{\"Action\":\"AddFunds\",\"FirstName\":\"Joe\",\"Surname\":\"Bloggs\",\"Amount\":50.95}";
            var bytes = Encoding.UTF8.GetBytes(postData);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            Assert.That(nock.Done(), Is.False);

            var errorMessage = "";
            try
            {
                request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
                request.ContentType = "application/json; encoding='utf-8'";
                request.Method = "GET";
                var response = request.GetResponse();
            }
            catch (WebException ex)
            {
                errorMessage = ex.Message;
            }

            Assert.That(errorMessage, Is.EqualTo("The remote server returned an error: (417) Expectation Failed."));
        }


        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnResponseCreatorFunction()
        {
            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .MatchHeader("cheese", "gravy")
                .Post("/api/v2/action/")
                .Reply(HttpStatusCode.OK, (requestUrl, requestHeaders, requestBody) =>
                {
                    var headers = new NameValueCollection();
                    headers.Add("crowe", "man");

                    return new WebResponse(headers, "yum yum");
                });

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Headers.Add("cheese", "gravy");
            request.Method = "POST";

            var postData = "{\"Action\":\"AddFunds\",\"FirstName\":\"Joe\",\"Surname\":\"Bloggs\",\"Amount\":50.95}";
            var bytes = Encoding.UTF8.GetBytes(postData);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            System.Net.WebResponse response = request.GetResponse();
            var bodyOne = ReadResponseBody(response);

            Assert.That(nock.Done(), Is.True);
            Assert.That(response.Headers["crowe"], Is.EqualTo("man"));
            Assert.That(bodyOne, Is.EqualTo("yum yum"));
        }

        [Test]
        public void NockedPostRequestsWorkCorrectlyWithBodyFilterAndCustomResponse()
        {
            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .MatchHeader("cheese", "gravy")
                .Post("/api/v2/action/", (body) => { return body.Contains("AddFunds"); })
                .Reply(HttpStatusCode.OK, (requestUrl, requestHeaders, requestBody) =>
                {
                    var firstName = requestBody.Substring(requestBody.IndexOf("FirstName") + 12);
                    firstName = firstName.Substring(0, firstName.IndexOf("\""));
                    var surname = requestBody.Substring(requestBody.IndexOf("Surname") + 10);
                    surname = surname.Substring(0, surname.IndexOf("\""));

                    var headers = new NameValueCollection();
                    headers.Add("firstname", firstName);

                    return new WebResponse(headers, surname);
                });

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Headers.Add("cheese", "gravy");
            request.Method = "POST";

            var postData = "{\"Action\":\"AddFunds\",\"FirstName\":\"Joe\",\"Surname\":\"Bloggs\",\"Amount\":50.95}";
            var bytes = Encoding.UTF8.GetBytes(postData);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            System.Net.WebResponse response = request.GetResponse();
            var bodyOne = ReadResponseBody(response);

            Assert.That(nock.Done(), Is.True);
            Assert.That(response.Headers["firstname"], Is.EqualTo("Joe"));
            Assert.That(bodyOne, Is.EqualTo("Bloggs"));
        }


        [Test]
        public void NockedResponseCorrectlyRespondsBasedOnFunctionBodyFiltersThatThrowAnException()
        {
            var nock = new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Post("/api/v2/action/", (body) => { throw new WebException("Oh no"); })
                .Reply(HttpStatusCode.OK, "The body");

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "POST";

            var postData = "{\"Action\":\"AddFunds\",\"FirstName\":\"Joe\",\"Surname\":\"Bloggs\",\"Amount\":50.95}";
            var bytes = Encoding.UTF8.GetBytes(postData);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            Assert.That(nock.Done(), Is.False);

            var errorMessage = "";
            try
            {
                request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
                request.ContentType = "application/json; encoding='utf-8'";
                request.Method = "GET";
                var response = request.GetResponse();
            }
            catch (WebException ex)
            {
                errorMessage = ex.Message;
            }

            Assert.That(errorMessage, Is.EqualTo("The remote server returned an error: (417) Expectation Failed."));
        }

        [Test]
        public void NockedResponsesCorrectlyRespondWhenContentTypeDiffers()
        {
            var xmlResponse = "<somexml />";
            var jsonResponse = "{ a:\"\"}";

            var nockOne = new nock("http://www.nock-fake-domain.co.uk")
                .ContentType("application/xml; encoding='utf-8'")
                .Get("/")
                .Reply(HttpStatusCode.OK, xmlResponse);

            var nockTwo = new nock("http://www.nock-fake-domain.co.uk")
                .ContentType("application/json; encoding='utf-8'")
                .Get("/")
                .Reply(HttpStatusCode.OK, jsonResponse);

            var request = WebRequest.Create("http://www.nock-fake-domain.co.uk") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "GET";

            System.Net.WebResponse response = request.GetResponse();
            var body = ReadResponseBody(response);

            Assert.That(nockOne.Done(), Is.False);
            Assert.That(nockTwo.Done(), Is.True);
            Assert.That(body, Is.EqualTo(jsonResponse));

            request = WebRequest.Create("http://www.nock-fake-domain.co.uk") as HttpWebRequest;
            request.ContentType = "application/xml; encoding='utf-8'";
            request.Method = "GET";

            response = request.GetResponse();
            body = ReadResponseBody(response);

            Assert.That(nockOne.Done(), Is.True);
            Assert.That(body, Is.EqualTo(xmlResponse));
        }

        [TestCase(HttpStatusCode.OK, "Added", Status.OK)]
        [TestCase(HttpStatusCode.OK, "User not allowed", Status.Forbidden)]
        [TestCase(HttpStatusCode.OK, "User could not be found", Status.NotFound)]
        [TestCase(HttpStatusCode.ServiceUnavailable, "Something went wrong", Status.Error)]
        [Test]
        public void NockingAResponseCorrectlyReturnsRelevantResponses(HttpStatusCode? statusCode, string resultMessage, Status expectedStatus)
        {
            var responseJson = string.Format("{{ result: \"{0}\" }}", resultMessage);

            new nock("http://domain-name.com")
                .ContentType("application/json; encoding='utf-8'")
                .Post("/api/v2/action/")
                .Reply(HttpStatusCode.OK, responseJson);

            var postData = PostDataToAnEndpointAndProcessTheResponse();

            Assert.That(postData.Status, Is.EqualTo(expectedStatus));
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

            var request = WebRequest.Create("http://domain-name.com/api/v2/action/") as HttpWebRequest;
            request.ContentType = "application/json; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            HttpWebResponse response = null;

            try
            {
                response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var body = ReadResponseBody(response);

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

        private string ReadResponseBody(System.Net.WebResponse response)
        {
            var body = string.Empty;

            using (var reader = new StreamReader(response.GetResponseStream(), true))
            {
                body = reader.ReadToEnd();
            }

            return body;
        }

        public class ResponseModel
        {
            public string Result { get; set; }
        }

        //public class CustomHttpWebResponse : System.Net.WebResponse
        //{
        //    private readonly NameValueCollection _headers = new NameValueCollection();

        //    public override WebHeaderCollection Headers
        //    {
        //        get
        //        {
        //            return _headers;
        //        }
        //    }
        //}

        public class PostResult
        {
            public Status Status { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
