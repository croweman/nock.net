using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;
using System.Collections.Specialized;

namespace Nock.net.Tests
{
    [TestFixture]
    public class RequestMatcherTests
    {
        [SetUp]
        public void SetUp()
        {
            nock.ClearAll();
            nock.Stop();
        }

        [Test]
        public void FindNockedWebResponseReturnsANullObjectIfThereAreNoResponseDetailsDefined()
        {
            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path";

            var match = RequestMatcher.FindNockedWebResponse(request);

            Assert.That(match.NockedRequest, Is.Null);
        }

        [Test]
        public void FindNockedWebResponseReturnsAValidStandardTestHttpWebResponseObject()
        {
            var headers = new NameValueCollection { { "x-custom", "custom-value" } };

            new nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Reply(HttpStatusCode.Created, "The response", headers);

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/";
            request.InputStream = new MemoryStream();
            request.Method = "GET";

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);

            var response = nockMatch.NockedRequest;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.ResponseHeaders.Count, Is.EqualTo(1));
            Assert.That(response.ResponseHeaders["x-custom"], Is.EqualTo("custom-value"));

            Assert.That(response.Response, Is.EqualTo("The response"));

            Assert.That(nock.NockedRequests.Count, Is.EqualTo(0));
        }


        [Test]
        public void FindNockedWebResponseCorrectlyReturnsRelevantResponseWhenContentTypeDiffers()
        {
            var xmlResponse = "<somexml />";
            var jsonResponse = "{ a:\"\"}";

            new nock("http://www.nock-fake-domain.co.uk")
                .ContentType("application/xml")
                .Get("/path/")
                .Reply(HttpStatusCode.OK, xmlResponse);

            new nock("http://www.nock-fake-domain.co.uk")
                .ContentType("application/json")
                .Get("/path/")
                .Reply(HttpStatusCode.OK, jsonResponse);

            Assert.That(nock.NockedRequests.Count, Is.EqualTo(2));

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/";
            request.InputStream = new MemoryStream();
            request.Method = "GET";
            //request.ContentType = "application/json";
            request.Headers.Add("Content-Type", "application/json");

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response.Response, Is.EqualTo(jsonResponse));
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));

            //request.ContentType = "application/xml";
            request.Headers["Content-Type"] = "application/xml";

            nockMatch = RequestMatcher.FindNockedWebResponse(request);
            response = nockMatch.NockedRequest;

            Assert.That(response.Response, Is.EqualTo(xmlResponse));
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindNockedWebResponseCorrectlyReturnsRelevantResponseWhenMethodDiffers()
        {
            var getResponse = "<somexml />";
            var postResponse = "{ a:\"\"}";

            new nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Reply(HttpStatusCode.OK, getResponse);

            new nock("http://www.nock-fake-domain.co.uk")
                .Post("/path/")
                .Reply(HttpStatusCode.OK, postResponse);

            Assert.That(nock.NockedRequests.Count, Is.EqualTo(2));

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/";
            request.InputStream = new MemoryStream();
            request.Method = "POST";

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response.Response, Is.EqualTo(postResponse));
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));

            request.Method = "GET";

            nockMatch = RequestMatcher.FindNockedWebResponse(request);
            response = nockMatch.NockedRequest;

            Assert.That(response.Response, Is.EqualTo(getResponse));
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindNockedWebResponseReturnsNullIfBodyInRequestAndNockDiffers()
        {
            new nock("http://www.nock-fake-domain.co.uk")
                .Post("/path/", "nock body")
                .Reply(HttpStatusCode.OK, "I am a test response");

            var postData = "Request body";
            var bytes = Encoding.UTF8.GetBytes(postData);

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/";
            request.Method = "POST";

            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            request.InputStream = ms;

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response, Is.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));
        }

        [Test]
        public void FindNockedWebResponseReturnsAValidStandardTestHttpWebResponseObjectIfBodyInRequestAndNockAreEqual()
        {
            new nock("http://www.nock-fake-domain.co.uk")
                .Post("/path/", "Request body")
                .Reply(HttpStatusCode.OK, "I am a test response");

            var postData = "Request body";
            var bytes = Encoding.UTF8.GetBytes(postData);

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/";
            request.Method = "POST";

            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            request.InputStream = ms;

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response, Is.Not.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindNockedWebResponseReturnsNullIfRequestAndNockRequestHeadersDoNotMatch()
        {
            var requestHeaders = new NameValueCollection
            {
                { "x-custom", "val" }
            };

            new nock("http://www.nock-fake-domain.co.uk")
                .Post("/path/")
                .MatchHeaders(requestHeaders)
                .Reply(HttpStatusCode.OK, "I am a test response");

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/";
            request.Method = "POST";
            //request.ContentType = "application/xml";
            request.InputStream = new MemoryStream();
            request.Headers = new WebHeaderCollection();

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response, Is.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));
        }

        [Test]
        public void FindNockedWebResponseReturnsNullIfRequestAndNockRequestHeadersDoNotMatch2()
        {        
            var requestHeaders = new NameValueCollection
            {
                { "x-custom", "val" }
            };

            new nock("http://www.nock-fake-domain.co.uk")
                .Post("/path/")
                .MatchHeaders(requestHeaders)
                .Reply(HttpStatusCode.OK, "I am a test response");

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/";
            request.Method = "POST";
            //request.ContentType = "application/xml";
            request.InputStream = new MemoryStream();
            request.Headers = new NameValueCollection();
            request.Headers.Add("x-custom", "val2");

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response, Is.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));
        }

        [Test]
        public void FindNockedWebResponseReturnsAValidStandardWebResponseObjectIfRequestAndNockRequestHeadersMatch()
        {
            var requestHeaders = new NameValueCollection
            {
                { "x-custom", "val" }
            };

            new nock("http://www.nock-fake-domain.co.uk")
                .ContentType("application/xml")
                .Post("/path/")
                .MatchHeaders(requestHeaders)
                .Reply(HttpStatusCode.OK, "I am a test response");

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/";
            request.Method = "POST";
            //request.ContentType = "application/xml";
            request.InputStream = new MemoryStream();
            request.Headers = new NameValueCollection();
            request.Headers.Add("x-custom", "val");
            request.Headers.Add("Content-Type", "application/xml");

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response, Is.Not.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindNockedWebResponseCorrectlyFiltersBasedOnQueryBool()
        {
            new nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Query(false)
                .Log(System.Console.WriteLine)
                .Reply(HttpStatusCode.OK, "I am a test response");

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/?test=1";
            request.Method = "GET";
            request.InputStream = new MemoryStream();
            request.Headers = new NameValueCollection();
            request.Query = new NameValueCollection { { "test", "1" } };

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response, Is.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));

            nock.ClearAll();

            new nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Query(true)
                .Reply(HttpStatusCode.OK, "I am a test response");

            request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/?test=1";
            request.Method = "GET";
            request.InputStream = new MemoryStream();
            request.Headers = new NameValueCollection();
            request.Query = new NameValueCollection { { "test", "1" } };

            nockMatch = RequestMatcher.FindNockedWebResponse(request);
            response = nockMatch.NockedRequest;

            Assert.That(response, Is.Not.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindNockedWebResponseCorrectlyFiltersBasedOnQueryFunc()
        {
            new nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Query((queryDetails) =>
                {
                    return queryDetails.Query["test"] == "2";
                })
                .Reply(HttpStatusCode.OK, "I am a test response");

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/?test=1";
            request.Method = "GET";
            request.InputStream = new MemoryStream();
            request.Headers = new NameValueCollection();
            request.Query = new NameValueCollection { { "test", "1" } };

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response, Is.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));

            nock.ClearAll();

            new nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Query((queryDetails) =>
                {
                    return queryDetails.Query["test"] == "2";
                })
                .Reply(HttpStatusCode.OK, "I am a test response");

            request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/?test=2";
            request.Method = "GET";
            request.InputStream = new MemoryStream();
            request.Headers = new NameValueCollection();
            request.Query = new NameValueCollection { { "test", "2" } };

            nockMatch = RequestMatcher.FindNockedWebResponse(request);
            response = nockMatch.NockedRequest;

            Assert.That(response, Is.Not.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindNockedWebResponseCorrectlyFiltersBasedOnQueryNameValue()
        {
            var nvc = new NameValueCollection();
            nvc.Add("test", "2");

            new nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Query(nvc)
                .Reply(HttpStatusCode.OK, "I am a test response");

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/?test=1";
            request.Method = "GET";
            request.InputStream = new MemoryStream();
            request.Headers = new NameValueCollection();
            request.Query = new NameValueCollection { { "test", "1" } };

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response, Is.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));

            nock.ClearAll();

            new nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Query(nvc)
                .Reply(HttpStatusCode.OK, "I am a test response");

            request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/?test=2";
            request.Method = "GET";
            request.InputStream = new MemoryStream();
            request.Headers = new NameValueCollection();
            request.Query = new NameValueCollection { { "test", "2" } };

            nockMatch = RequestMatcher.FindNockedWebResponse(request);
            response = nockMatch.NockedRequest;

            Assert.That(response, Is.Not.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(0));
        }

        [Test]
        public void FindNockedWebResponseCorrectlyFiltersBasedOnQueryNameValueExact()
        {
            var nvc = new NameValueCollection();
            nvc.Add("test", "1");

            new nock("http://www.nock-fake-domain.co.uk")
                .Get("/path/")
                .Query(nvc, true)
                .Reply(HttpStatusCode.OK, "I am a test response");

            var request = new NockHttpWebRequest();
            request.RequestUri = "http://www.nock-fake-domain.co.uk/path/?test=1&test2=2";
            request.Method = "GET";
            request.InputStream = new MemoryStream();
            request.Headers = new NameValueCollection();
            request.Query = new NameValueCollection { { "test", "1" }, { "test2", "2" } };

            var nockMatch = RequestMatcher.FindNockedWebResponse(request);
            var response = nockMatch.NockedRequest;

            Assert.That(response, Is.Null);
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));
        }

        /*
         * TODO: adsf
         * Header matches, with set etc and functions, test with tuple stuff, as they will not be availabe in standard request headers, again when matching headers, should the whole request be an argument
         * when doing custom matching?
         * body matching all variations
         * dynamic response building function stuff?
         */


    }
}
