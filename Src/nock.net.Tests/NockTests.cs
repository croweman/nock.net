using System;
using System.Linq;
using System.Net;
using NUnit.Framework;

namespace Nock.net.Tests
{
    [TestFixture]
    public class NockTests
    {

        [SetUp]
        public void SetUp()
        {
            Nock.ClearAll();
        }

        #region Object creation tests

        [TestCase(null, "Url must be defined")]
        [TestCase("", "Url must be defined")]
        [TestCase("http://google.co.uk/", "The url must not end with a '/'")]
        [Test]
        public void WhenCreatingANockAnExceptionWillBeThrownIfUrlIsInvalid(string url, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock(url));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        #endregion Object creation tests

        #region Path and method setting tests

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void GetWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Get(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void PostWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Post(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void PutWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Put(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void DeleteWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Delete(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void HeadWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Head(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void PatchWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Patch(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void MergeWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Merge(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        #endregion Path and method setting tests

        #region Content type tests

        [TestCase(null)]
        [TestCase("")]
        [Test]
        public void ContentTypeWillThrowAnExceptionIsContentTypeIsInvalid(string contentType)
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Get("/").ContentType(contentType));
            Assert.That(exception.Message, Is.EqualTo("Content type must be defined"));
        }

        #endregion Content type tests

        #region Reply method tests

        [Test]
        public void ReplyWillThrowAnExceptionIfPathHasNotBeenDefined()
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Reply(HttpStatusCode.OK, ""));
            Assert.That(exception.Message, Is.EqualTo("Path must be defined"));
        }

        [Test]
        public void ReplyWillThrowAnExceptionIfNockHasAlreadyBeenBuilt()
        {
            var nock = new Nock("http://www.google.co.uk")
                .Get("/")
                .Reply(HttpStatusCode.OK, "");

            var exception = Assert.Catch<Exception>(() => nock.Reply(HttpStatusCode.OK, ""));
            Assert.That(exception.Message, Is.EqualTo("The nock has already been built"));
        }

        [Test]
        public void ReplyWithExceptionWillThrowAnExceptionIfExceptionIsNotDefined()
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Reply<WebException>(null));
            Assert.That(exception.Message, Is.EqualTo("The exception must be defined"));
        }

        [Test]
        public void ReplyWithExceptionWillThrowAnExceptionIfNockHasAlreadyBeenBuilt()
        {
            var nock = new Nock("http://www.google.co.uk")
                .Get("/")
                .Reply(HttpStatusCode.OK, "");

            var exception = Assert.Catch<Exception>(() => nock.Reply<WebException>(null));
            Assert.That(exception.Message, Is.EqualTo("The nock has already been built"));
        }

        [Test]
        public void ReplyWithExceptionWillThrowAnExceptionIfPathIsNotDefined()
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Reply<WebException>(new WebException()));
            Assert.That(exception.Message, Is.EqualTo("Path must be defined"));
        }

        [Test]
        public void ReplyWithTestNockHttpWebResponseWillThrowAnExceptionIfTestNockHttpWebResponseIsNotDefined()
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Reply(null));
            Assert.That(exception.Message, Is.EqualTo("The nock response must be defined"));
        }

        [Test]
        public void ReplyWithTestNockHttpWebResponseWillThrowAnExceptionIfNockHasAlreadyBeenBuilt()
        {
            var nock = new Nock("http://www.google.co.uk")
                .Get("/")
                .Reply(HttpStatusCode.OK, "");

            var exception = Assert.Catch<Exception>(() => nock.Reply(null));
            Assert.That(exception.Message, Is.EqualTo("The nock has already been built"));
        }

        [Test]
        public void ReplyWithTestNockHttpWebResponseWillThrowAnExceptionIfPathIsNotDefined()
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Reply(new TestHttpWebResponse(string.Empty)));
            Assert.That(exception.Message, Is.EqualTo("Path must be defined"));
        }

        #endregion Reply method tests 

        #region Times method tests

        [Test]
        public void TimesWillThrowAnExceptionIfNotHasNotBeenBuilt()
        {
            var exception = Assert.Catch<Exception>(() => new Nock("http://www.google.co.uk").Times(2));
            Assert.That(exception.Message, Is.EqualTo("You have not called a valid action method e.g. get, post and the Reply method"));
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [Test]
        public void TimesWillThrowAnExceptionIfTimesValueIsInvalid(int numberOfTimes)
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Get("/").Reply(HttpStatusCode.OK, "").Times(numberOfTimes));
            Assert.That(exception.Message, Is.EqualTo("Number of times must be greater than 1"));
        }

        #endregion Times method tests

        #region Request headers tests

        [Test]
        public void RequestHeadersWillThrowAnExceptionIfRequestHeadersIsNull()
        {
            var exception = Assert.Catch<ArgumentException>(() => new Nock("http://www.google.co.uk").Get("/").RequestHeaders(null));
            Assert.That(exception.Message, Is.EqualTo("Request headers must be defined"));
        }

        #endregion Request headers tests

        #region General valid nock building tests

        [Test]
        public void NockingAGetResponseCorrectlyBuildsResponseDetailObject()
        {
            var headers = new WebHeaderCollection { { "Content-Type", "application/json" } };
            var requestHeaders = new WebHeaderCollection { { "Content-Type", "application/xml" } };

            new Nock("http://www.google.co.uk")
                .Get("/test", "The body")
                .RequestHeaders(requestHeaders)
                .ContentType("application/json")
                .Reply(HttpStatusCode.Created, "<blah2>", headers);

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/test"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.GET));
            Assert.That(responseDetail.Body, Is.EqualTo("The body"));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(responseDetail.ContentType, Is.EqualTo("application/json"));
            Assert.That(responseDetail.Response, Is.EqualTo("<blah2>"));
            Assert.That(responseDetail.Headers.Count, Is.EqualTo(1));
            Assert.That(responseDetail.Headers.AllKeys[0], Is.EqualTo("Content-Type"));
            Assert.That(responseDetail.Headers["Content-Type"], Is.EqualTo("application/json"));
            Assert.That(responseDetail.RequestHeaders.Count, Is.EqualTo(1));
            Assert.That(responseDetail.RequestHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
            Assert.That(responseDetail.RequestHeaders["Content-Type"], Is.EqualTo("application/xml"));
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Null);
        }

        [Test]
        public void NockingAGetResponseCorrectlyBuildsResponseDetailObjectWhenNoContentTypeOrHeadersAreDefined()
        {
            new Nock("http://www.google.co.uk")
                .Get("/test")
                .Reply(HttpStatusCode.Created, "<blah2>");

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/test"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.GET));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(responseDetail.ContentType, Is.Null);
            Assert.That(responseDetail.Response, Is.EqualTo("<blah2>"));
            Assert.That(responseDetail.Headers.Count, Is.EqualTo(0));
            Assert.That(responseDetail.RequestHeaders.Count, Is.EqualTo(0));
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Null);
        }

        [Test]
        public void NockingAPostResponseCorrectlyBuildsResponseDetailObject()
        {
            var headers = new WebHeaderCollection { { "Content-Type", "application/json" } };

            new Nock("http://www.google.co.uk")
                .Post("/test", "The body")
                .Reply(HttpStatusCode.NotFound, "<blah>", headers);

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/test"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.POST));
            Assert.That(responseDetail.Body, Is.EqualTo("The body"));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(responseDetail.Response, Is.EqualTo("<blah>"));
            Assert.That(responseDetail.Headers.Count, Is.EqualTo(1));

            Assert.That(responseDetail.Headers.AllKeys[0], Is.EqualTo("Content-Type"));
            Assert.That(responseDetail.Headers["Content-Type"], Is.EqualTo("application/json"));
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Null);
        }

        [Test]
        public void NockingAPutResponseCorrectlyBuildsResponseDetailObject()
        {
            var headers = new WebHeaderCollection { { "x-Custom", "asdfe" } };

            new Nock("http://www.anothergoogle.co.uk")
                .Put("/testing", "The body")
                .Reply(HttpStatusCode.Accepted, "<blah>", headers);

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.anothergoogle.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/testing"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.PUT));
            Assert.That(responseDetail.Body, Is.EqualTo("The body"));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
            Assert.That(responseDetail.Response, Is.EqualTo("<blah>"));
            Assert.That(responseDetail.Headers.Count, Is.EqualTo(1));

            Assert.That(responseDetail.Headers.AllKeys[0], Is.EqualTo("x-Custom"));
            Assert.That(responseDetail.Headers["x-custom"], Is.EqualTo("asdfe"));
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Null);
        }

        [Test]
        public void NockingADeleteResponseCorrectlyBuildsResponseDetailObject()
        {
            var headers = new WebHeaderCollection { { "Content-Type", "application/json" } };

            new Nock("http://www.google.co.uk")
                .Delete("/test", "the body")
                .Reply(HttpStatusCode.Created, "<blah2>", headers);

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/test"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.DELETE));
            Assert.That(responseDetail.Body, Is.EqualTo("the body"));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(responseDetail.Response, Is.EqualTo("<blah2>"));
            Assert.That(responseDetail.Headers.Count, Is.EqualTo(1));

            Assert.That(responseDetail.Headers.AllKeys[0], Is.EqualTo("Content-Type"));
            Assert.That(responseDetail.Headers["Content-Type"], Is.EqualTo("application/json"));
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Null);
        }

        [Test]
        public void NockingAHeadResponseCorrectlyBuildsResponseDetailObject()
        {
            var headers = new WebHeaderCollection { { "Content-Type", "application/json" } };

            new Nock("http://www.google.co.uk")
                .Head("/test")
                .Reply(HttpStatusCode.Created, "<blah2>", headers);

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/test"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.HEAD));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(responseDetail.Response, Is.EqualTo("<blah2>"));
            Assert.That(responseDetail.Headers.Count, Is.EqualTo(1));

            Assert.That(responseDetail.Headers.AllKeys[0], Is.EqualTo("Content-Type"));
            Assert.That(responseDetail.Headers["Content-Type"], Is.EqualTo("application/json"));
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Null);
        }

        [Test]
        public void NockingAPatchResponseCorrectlyBuildsResponseDetailObject()
        {
            var headers = new WebHeaderCollection { { "Content-Type", "application/json" } };

            new Nock("http://www.google.co.uk")
                .Patch("/test")
                .Reply(HttpStatusCode.Created, "<blah2>", headers);

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/test"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.PATCH));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(responseDetail.Response, Is.EqualTo("<blah2>"));
            Assert.That(responseDetail.Headers.Count, Is.EqualTo(1));

            Assert.That(responseDetail.Headers.AllKeys[0], Is.EqualTo("Content-Type"));
            Assert.That(responseDetail.Headers["Content-Type"], Is.EqualTo("application/json"));
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Null);
        }

        [Test]
        public void NockingAMergeResponseCorrectlyBuildsResponseDetailObject()
        {
            var headers = new WebHeaderCollection { { "Content-Type", "application/xml" } };

            new Nock("http://www.google.co.uk")
                .Merge("/test")
                .Reply(HttpStatusCode.Created, "<blah2>", headers);

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/test"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.MERGE));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(responseDetail.Response, Is.EqualTo("<blah2>"));
            Assert.That(responseDetail.Headers.Count, Is.EqualTo(1));

            Assert.That(responseDetail.Headers.AllKeys[0], Is.EqualTo("Content-Type"));
            Assert.That(responseDetail.Headers["Content-Type"], Is.EqualTo("application/xml"));
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Null);
        }

        [Test]
        public void NockingAGetResponseWhichRepliesWithAStandardResponseCorrectlyBuildsResponseDetailObject()
        {
            var headers = new WebHeaderCollection { { "Content-Type", "application/json" } };

            new Nock("http://www.google.co.uk")
                .Get("/test")
                .ContentType("application/json")
                .Reply(HttpStatusCode.Created, "<blah2>", headers);

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/test"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.GET));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(responseDetail.ContentType, Is.EqualTo("application/json"));
            Assert.That(responseDetail.Response, Is.EqualTo("<blah2>"));
            Assert.That(responseDetail.Headers.Count, Is.EqualTo(1));

            Assert.That(responseDetail.Headers.AllKeys[0], Is.EqualTo("Content-Type"));
            Assert.That(responseDetail.Headers["Content-Type"], Is.EqualTo("application/json"));
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Null);
        }

        [Test]
        public void NockingAGetResponseWhichRepliesWithAnExceptionCorrectlyBuildsResponseDetailObject()
        {
            var webException = new WebException("This is an exception");

            new Nock("http://www.google.co.uk")
                .Get("/test")
                .ContentType("application/xml")
                .Reply<WebException>(webException)
                .Times(2);

            Assert.That(Nock.ResponseDetails.Count, Is.EqualTo(2));


            foreach (var responseDetail in Nock.ResponseDetails)
            {
                Assert.That(responseDetail, Is.Not.Null);

                Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
                Assert.That(responseDetail.Path, Is.EqualTo("/test"));
                Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.GET));
                Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseDetail.ContentType, Is.EqualTo("application/xml"));
                Assert.That(responseDetail.Response, Is.EqualTo(string.Empty));
                Assert.That(responseDetail.Headers, Is.Null);
                Assert.That(responseDetail.Exception, Is.Not.Null);
                Assert.That(responseDetail.Exception.Message, Is.EqualTo(webException.Message));
                Assert.That(responseDetail.TestHttpWebResponse, Is.Null);
            }

        }

        [Test]
        public void NockingAGetResponseWhichRepliesWithATestNockHttpWebResponseCorrectlyBuildsResponseDetailObject()
        {
            var testNockHttpWebResponse = new TestHttpWebResponse("This is the response body");

            new Nock("http://www.google.co.uk")
                .Get("/test")
                .ContentType("application/txt")
                .Reply(testNockHttpWebResponse);

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/test"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.GET));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseDetail.ContentType, Is.EqualTo("application/txt"));
            Assert.That(responseDetail.Response, Is.EqualTo(string.Empty));
            Assert.That(responseDetail.Headers, Is.Null);
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Not.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.EqualTo(testNockHttpWebResponse));
        }

        [Test]
        public void NockingAGetResponseWhichRepliesWithATestNockHttpWebResponseCorrectlyBuildsResponseDetailObjectTwoTimes()
        {
            var testNockHttpWebResponse = new TestHttpWebResponse("This is the response body");

            new Nock("http://www.google.co.uk")
                .Get("/test")
                .ContentType("application/txt")
                .Reply(testNockHttpWebResponse);

            var responseDetail = Nock.ResponseDetails.Last();
            Assert.That(responseDetail, Is.Not.Null);

            Assert.That(responseDetail.Url, Is.EqualTo("http://www.google.co.uk"));
            Assert.That(responseDetail.Path, Is.EqualTo("/test"));
            Assert.That(responseDetail.Method, Is.EqualTo(Nock.Method.GET));
            Assert.That(responseDetail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseDetail.ContentType, Is.EqualTo("application/txt"));
            Assert.That(responseDetail.Response, Is.EqualTo(string.Empty));
            Assert.That(responseDetail.Headers, Is.Null);
            Assert.That(responseDetail.Exception, Is.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.Not.Null);
            Assert.That(responseDetail.TestHttpWebResponse, Is.EqualTo(testNockHttpWebResponse));
        }

        #endregion General valid nock building tests

    }
}
