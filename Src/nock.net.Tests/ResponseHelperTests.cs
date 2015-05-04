using System.IO;
using System.Net;
using NUnit.Framework;

namespace nock.net.Tests
{
    [TestFixture]
    public class ResponseHelperTests
    {
        [SetUp]
        public void SetUp()
        {
            Nock.ClearAll();
            Nock.Testing = false;
        }

        [Test]
        public void FindTestHttpWebResponseReturnsANullObjectIfThereAreNoResponseDetailsDefined()
        {
            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");

            var response = ResponseHelper.FindTestHttpWebResponse(request);

            Assert.That(response, Is.Null);
        }

        [Test]
        public void FindTestHttpWebResponseReturnsANullObjectIfResponseDetailsExistButTestingIsFalse()
        {
            Nock.ResponseDetails.Add(new ResponseDetail("http://www.nock-fake-domain.co.uk", "/path/", "Test", HttpStatusCode.OK, Nock.Method.GET, null, null, null, null));

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");

            var response = ResponseHelper.FindTestHttpWebResponse(request);

            Assert.That(response, Is.Null);
        }

        [Test]
        public void FindTestHttpWebResponseReturnsAValidStandardTestHttpWebResponseObject()
        {
            var headers = new WebHeaderCollection { { "x-custom", "custom-value" } };

            new Nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Reply(HttpStatusCode.Created, "The response", headers);

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");

            var response = ResponseHelper.BuildNockHttpWebResponse(ResponseHelper.FindTestHttpWebResponse(request));

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.ContentType, Is.Null);
            Assert.That(response.Headers.Count, Is.EqualTo(1));
            Assert.That(response.GetResponseHeader("x-custom"), Is.EqualTo("custom-value"));
            Assert.That(response.ContentLength, Is.EqualTo(12));

            string body;

            using (var reader = new StreamReader(response.GetResponseStream(), true))
            {
                body = reader.ReadToEnd();
            }

            Assert.That(body, Is.EqualTo("The response"));

            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindTestHttpWebResponseReturnsAValidStandardTestHttpWebResponseObjectButDoesNotRemoveFromResponseDetailsIfRemoveIsFalse()
        {
            var headers = new WebHeaderCollection { { "x-custom", "custom-value" } };

            new Nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Reply(HttpStatusCode.Created, "The response", headers);

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");

            var response = ResponseHelper.BuildNockHttpWebResponse(ResponseHelper.FindTestHttpWebResponse(request, false));

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.ContentType, Is.Null);
            Assert.That(response.Headers.Count, Is.EqualTo(1));
            Assert.That(response.GetResponseHeader("x-custom"), Is.EqualTo("custom-value"));
            Assert.That(response.ContentLength, Is.EqualTo(12));

            string body;

            using (var reader = new StreamReader(response.GetResponseStream(), true))
            {
                body = reader.ReadToEnd();
            }

            Assert.That(body, Is.EqualTo("The response"));

            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(1));
        }

        [Test]
        public void FindTestHttpWebResponseReturnsAValidCustomTestHttpWebResponseObject()
        {
            var headers = new WebHeaderCollection { { "x-custom2", "custom-value2" } };

            var testHttpWebResponse = new TestHttpWebResponse("I am a test response")
            {
                Headers = headers,
                ContentType = "application/json"
            };

            new Nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Reply(testHttpWebResponse);

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");

            var response = ResponseHelper.BuildNockHttpWebResponse(ResponseHelper.FindTestHttpWebResponse(request));

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(testHttpWebResponse));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.ContentType, Is.EqualTo("application/json"));
            Assert.That(response.Headers.Count, Is.EqualTo(1));
            Assert.That(response.GetResponseHeader("x-custom2"), Is.EqualTo("custom-value2"));
            Assert.That(response.ContentLength, Is.EqualTo(20));

            string body;

            using (var reader = new StreamReader(response.GetResponseStream(), true))
            {
                body = reader.ReadToEnd();
            }

            Assert.That(body, Is.EqualTo("I am a test response"));

            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindTestHttpWebResponseReturnsAValidCustomTestHttpWebResponseObjectWhenContentTypeDefinedOnRequestButNoContentTypeDefinedOnResponseDetail()
        {
            var testHttpWebResponse = new TestHttpWebResponse("I am a test response");

            new Nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Reply(testHttpWebResponse);

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");
            request.ContentType = "application/json";

            var response = ResponseHelper.BuildNockHttpWebResponse(ResponseHelper.FindTestHttpWebResponse(request));

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(testHttpWebResponse));
        }

        [Test]
        public void FindTestHttpWebResponseThrowsAnExceptionIsExceptionReplyIsDefined()
        {
            var webException = new WebException("oh no!");

            new Nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Reply(webException);

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");

            var exception = Assert.Throws<WebException>(() => ResponseHelper.BuildNockHttpWebResponse(ResponseHelper.FindTestHttpWebResponse(request)));
            
            Assert.That(exception.Message, Is.EqualTo("oh no!"));

            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindTestHttpWebResponseReturnsNullIfUrlMatchesButContentTypeDiffers()
        {
            var testHttpWebResponse = new TestHttpWebResponse("I am a test response");

            new Nock("http://www.nock-fake-domain.co.uk")
                .ContentType("application/json")
                .Get("/path/")
                .Reply(testHttpWebResponse);

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");
            request.ContentType = "application/xml";

            var response = ResponseHelper.FindTestHttpWebResponse(request);

            Assert.That(response, Is.Null);
            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(1));
        }

        [Test]
        public void FindTestHttpWebResponseReturnsNullIfUrlMatchesButMethodDiffers()
        {
            var testHttpWebResponse = new TestHttpWebResponse("I am a test response");

            new Nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Reply(testHttpWebResponse);

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");
            request.Method = "POST";

            var response = ResponseHelper.FindTestHttpWebResponse(request);

            Assert.That(response, Is.Null);
            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(1));
        }

        [Test]
        public void FindTestHttpWebResponseReturnsNullIfUrlDiffersAnExistingNock()
        {
            var testHttpWebResponse = new TestHttpWebResponse("I am a test response");

            new Nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Reply(testHttpWebResponse);

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path2/");

            var response = ResponseHelper.FindTestHttpWebResponse(request);

            Assert.That(response, Is.Null);
            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(1));
        }

        [Test]
        public void FindTestHttpWebResponseCorrectlyReturnsRelevantResponseWhenContentTypeDiffers()
        {
            var xmlResponse = new TestHttpWebResponse("<somexml />");
            var jsonResponse = new TestHttpWebResponse("{ a:\"\"}");

            new Nock("http://www.nock-fake-domain.co.uk")
                .ContentType("application/xml")
                .Get("/path/")
                .Reply(xmlResponse);

            new Nock("http://www.nock-fake-domain.co.uk")
                .ContentType("application/json")
                .Get("/path/")
                .Reply(jsonResponse);

            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(2));

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");
            request.Method = "Get";
            request.ContentType = "application/json";

            var response = ResponseHelper.BuildNockHttpWebResponse(ResponseHelper.FindTestHttpWebResponse(request));

            Assert.That(response, Is.EqualTo(jsonResponse));
            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(1));

            request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");
            request.Method = "Get";
            request.ContentType = "application/xml";

            response = ResponseHelper.BuildNockHttpWebResponse(ResponseHelper.FindTestHttpWebResponse(request));

            Assert.That(response, Is.EqualTo(xmlResponse));
            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindTestHttpWebResponseCorrectlyReturnsRelevantResponseWhenMethodDiffers()
        {
            var getResponse = new TestHttpWebResponse("<somexml />");
            var postResponse = new TestHttpWebResponse("{ a:\"\"}");

            new Nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Reply(getResponse);

            new Nock("http://www.nock-fake-domain.co.uk")
                .Post("/path/")
                .Reply(postResponse);

            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(2));

            var request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");
            request.Method = "POST";

            var response = ResponseHelper.BuildNockHttpWebResponse(ResponseHelper.FindTestHttpWebResponse(request));

            Assert.That(response, Is.EqualTo(postResponse));
            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(1));

            request = HttpWebRequest.CreateRequest("http://www.nock-fake-domain.co.uk/path/");
            request.Method = "GET";

            response = ResponseHelper.BuildNockHttpWebResponse(ResponseHelper.FindTestHttpWebResponse(request));

            Assert.That(response, Is.EqualTo(getResponse));
            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(0));
        }


    }
}
