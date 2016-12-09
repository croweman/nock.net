using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using System.Collections.Specialized;

namespace Nock.net.Tests
{
    public class Fish
    {
        public int Eyes { get; set; }
    }

    [TestFixture]
    public class NockTests
    {

        [SetUp]
        public void SetUp()
        {
            nock.ClearAll();
        }

        private bool FilterStringNonStatic(string body)
        {
            return true;
        }

        private static bool FilterStringStatic(string body)
        {
            return true;
        }

        private bool FilterTypedNonStatic(Fish fish)
        {
            return true;
        }

        private static bool FilterTypedStatic(Fish fish)
        {
            return true;
        }

        private bool FilterHeadersNonStatic(NameValueCollection headers)
        {
            return true;
        }

        private static bool FilterHeadersStatic(NameValueCollection headers)
        {
            return true;
        }

        #region Object creation tests

        [TestCase(null, "Url must be defined")]
        [TestCase("", "Url must be defined")]
        [TestCase("http://google.co.uk/", "The url must not end with a '/'")]
        [Test]
        public void WhenCreatingANockAnExceptionWillBeThrownIfUrlIsInvalid(string url, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock(url));
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
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Get(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void PostWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Post(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public void PostWillThrowAnExeptionIfStringTypeBodyMatcherFunctionIsDefinedWhichIsNotStatic()
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Post("/Test", FilterStringNonStatic));
            Assert.That(exception.Message, Is.EqualTo("The defined function is not static!"));
        }

        [Test]
        public void PostWillNotThrowAnExeptionIfStringTypeBodyMatcherFunctionIsDefinedWhichIsStatic()
        {
            var nock = new nock("http://www.google.co.uk").Post("/Test", FilterStringStatic);
        }

        [Test]
        public void PostWillThrowAnExeptionIfCustomTypeBodyMatcherFunctionIsDefinedWhichIsNotStatic()
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Post<Fish>("/Test", FilterTypedNonStatic));
            Assert.That(exception.Message, Is.EqualTo("The defined function is not static!"));
        }

        [Test]
        public void PostWillNotThrowAnExeptionIfCustomTypeBodyMatcherFunctionIsDefinedWhichIsStatic()
        {
            var nock = new nock("http://www.google.co.uk").Post<Fish>("/Test", FilterTypedStatic);
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void PutWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Put(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public void PutWillThrowAnExeptionIfStringTypeBodyMatcherFunctionIsDefinedWhichIsNotStatic()
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Put("/Test", FilterStringNonStatic));
            Assert.That(exception.Message, Is.EqualTo("The defined function is not static!"));
        }

        [Test]
        public void PutWillNotThrowAnExeptionIfStringTypeBodyMatcherFunctionIsDefinedWhichIsStatic()
        {
            var nock = new nock("http://www.google.co.uk").Put("/Test", FilterStringStatic);
        }

        [Test]
        public void PutWillThrowAnExeptionIfCustomTypeBodyMatcherFunctionIsDefinedWhichIsNotStatic()
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Put<Fish>("/Test", FilterTypedNonStatic));
            Assert.That(exception.Message, Is.EqualTo("The defined function is not static!"));
        }

        [Test]
        public void PutWillNotThrowAnExeptionIfCustomTypeBodyMatcherFunctionIsDefinedWhichIsStatic()
        {
            var nock = new nock("http://www.google.co.uk").Put<Fish>("/Test", FilterTypedStatic);
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void DeleteWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Delete(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void HeadWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Head(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void PatchWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Patch(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        [TestCase(null, "Path must be defined")]
        [TestCase("", "Path must be defined")]
        [TestCase("path", "Path must start with a '/'")]
        [Test]
        public void MergeWillThrowAnExceptionIsPathIsInvalid(string path, string exceptionMessage)
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Merge(path));
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage));
        }

        #endregion Path and method setting tests

        #region Content type tests

        [TestCase(null)]
        [Test]
        public void ContentTypeWillThrowAnExceptionIsContentTypeIsInvalid(string contentType)
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Get("/").ContentType(contentType));
            Assert.That(exception.Message, Is.EqualTo("Header value must be defined"));
        }

        #endregion Content type tests

        #region MatchHeader tests

        [TestCase(null, null, "Header name must be defined")]
        [TestCase("", null, "Header name must be defined")]
        [TestCase("Cheese", null, "Header value must be defined")]
        [Test]
        public void SetWillThrowAnExceptionIfSetIsInvalid(string headerName, string headerValue, string error)
        {
            var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Get("/").MatchHeader(headerName, headerValue));
            Assert.That(exception.Message, Is.EqualTo(error));
        }

        #endregion MatchHeader tests

        #region Query tests

        [Test]
        public void QueryFunctionCorrectlyCreatesBooleanMatch()
        {
            var nock = new nock("http://www.google.co.uk").Get("/").Query(true).Reply(HttpStatusCode.OK, "");
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));
            Assert.That(nock.NockedRequests[0].QueryMatcher, Is.EqualTo(QueryMatcher.Bool));
            Assert.That(nock.NockedRequests[0].QueryResult, Is.EqualTo(true));
        }

        [Test]
        public void QueryFunctionCorrectlyCreatesNameValueCollectionMatch()
        {
            var nvc = new NameValueCollection();

            var nock = new nock("http://www.google.co.uk").Get("/").Query(nvc).Reply(HttpStatusCode.OK, "");
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));
            Assert.That(nock.NockedRequests[0].QueryMatcher, Is.EqualTo(QueryMatcher.NameValue));
            Assert.That(nock.NockedRequests[0].Query, Is.EqualTo(nvc));
        }

        [Test]
        public void QueryFunctionCorrectlyCreatesNameValueCollectionMatchExact()
        {
            var nvc = new NameValueCollection();

            var nock = new nock("http://www.google.co.uk").Get("/").Query(nvc, true).Reply(HttpStatusCode.OK, "");
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));
            Assert.That(nock.NockedRequests[0].QueryMatcher, Is.EqualTo(QueryMatcher.NameValueExact));
            Assert.That(nock.NockedRequests[0].Query, Is.EqualTo(nvc));
        }

        [Test]
        public void QueryFunctionCorrectlyCreatesFuncMatch()
        {
            var nock = new nock("http://www.google.co.uk").Get("/").Query((queryDetails) => { return true; }).Reply(HttpStatusCode.OK, "");
            Assert.That(nock.NockedRequests.Count, Is.EqualTo(1));
            Assert.That(nock.NockedRequests[0].QueryMatcher, Is.EqualTo(QueryMatcher.Func));
            Assert.That(nock.NockedRequests[0].QueryFunc != null, Is.EqualTo(true));
        }

        #endregion Query tests

        #region Log tests

        public void NonStaticLogger(string toLog)
{

}

public static void StaticLogger(string toLog)
{

}

[Test]
public void LogWillThrowAnExceptionIfLoggerIsNull()
{
    var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Get("/").Log(null));
    Assert.That(exception.Message, Is.EqualTo("A logger must be defined"));
}

[Test]
public void LogWillThrowAnExceptionIfLoggerIsNonStatic()
{
    var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Get("/").Log(NonStaticLogger));
    Assert.That(exception.Message, Is.EqualTo("The defined function is not static!"));
}

[Test]
public void LogWillNotThrowAnExceptionIfLoggerIsStatic()
{
    new nock("http://www.google.co.uk").Get("/").Log(StaticLogger);
}

[Test]
public void LogWillNotThrowAnExceptionIfLoggerIsAnInlineDelegateAction()
{
    new nock("http://www.google.co.uk").Get("/").Log((value) => { });
}

#endregion Log tests

#region Reply method tests

[Test]
public void ReplyWillThrowAnExceptionIfPathHasNotBeenDefined()
{
    var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Reply(HttpStatusCode.OK, ""));
    Assert.That(exception.Message, Is.EqualTo("Path must be defined"));
}

[Test]
public void ReplyWillThrowAnExceptionIfNockHasAlreadyBeenBuilt()
{
    var nock = new nock("http://www.google.co.uk")
        .Get("/")
        .Reply(HttpStatusCode.OK, "");

    var exception = Assert.Catch<Exception>(() => nock.Reply(HttpStatusCode.OK, ""));
    Assert.That(exception.Message, Is.EqualTo("The nock has already been built"));
}

[Test]
public void ReplyWillThrowAnExceptionIfNullResponseCreatorIsDefined()
{
    Func<RequestDetails, WebResponse> blah = null;

    var exception = Assert.Catch<Exception>(() => new nock("http://www.google.co.uk").Get("/").Reply(HttpStatusCode.OK, blah));
    Assert.That(exception.Message, Is.EqualTo("Response creator function is invalid"));
}

private WebResponse Blah(RequestDetails requestDetails)
{
    return null;
}

[Test]
public void ReplyWillThrowAnExceptionIfNonStaticResponseCreatorIsDefined()
{
    var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Get("/").Reply(HttpStatusCode.OK, Blah));
    Assert.That(exception.Message, Is.EqualTo("The defined function is not static!"));
}

#endregion Reply method tests 

#region Times method tests

[Test]
public void TimesWillThrowAnExceptionIfNockHasNotBeenBuilt()
{
    var exception = Assert.Catch<Exception>(() => new nock("http://www.google.co.uk").Times(2));
    Assert.That(exception.Message, Is.EqualTo("You have not called a valid action method e.g. Get, Post and the Reply method"));
}

[TestCase(-1)]
[TestCase(0)]
[TestCase(1)]
[Test]
public void TimesWillThrowAnExceptionIfTimesValueIsInvalid(int numberOfTimes)
{
    var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Get("/").Reply(HttpStatusCode.OK, "").Times(numberOfTimes));
    Assert.That(exception.Message, Is.EqualTo("Number of times must be greater than 1"));
}

#endregion Times method tests

#region Request headers tests

[Test]
public void RequestHeadersWillThrowAnExceptionIfRequestHeadersIsNull()
{
    var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Get("/").MatchHeaders((WebHeaderCollection)null));
    Assert.That(exception.Message, Is.EqualTo("Request headers must be defined"));
}

[Test]
public void RequestHeadersWillThrowAnExceptionIfRequestHeadersMatchingFuncIsNotStatic()
{
    var exception = Assert.Catch<ArgumentException>(() => new nock("http://www.google.co.uk").Get("/").MatchHeaders(FilterHeadersNonStatic));
    Assert.That(exception.Message, Is.EqualTo("The defined function is not static!"));
}

[Test]
public void RequestHeadersWillNotThrowAnExceptionIfRequestHeadersMatchingFuncIsStatic()
{
    new nock("http://www.google.co.uk").Get("/").MatchHeaders(FilterHeadersStatic);
}

#endregion Request headers tests

#region General valid nock building tests

[Test]
public void NockingAGetResponseCorrectlyBuildsNockedRequestObject()
{
    var headers = new NameValueCollection { { "Content-Type", "application/json" } };
    var requestHeaders = new NameValueCollection { { "Content-Type", "application/xml" } };

    new nock("http://www.google.co.uk")
        .Get("/test", "The body")
        .MatchHeaders(requestHeaders)
        .Reply(HttpStatusCode.Created, "<blah2>", headers);

    var nockedRequest = nock.NockedRequests.Last();
    Assert.That(nockedRequest, Is.Not.Null);

    Assert.That(nockedRequest.Url, Is.EqualTo("http://www.google.co.uk"));
    Assert.That(nockedRequest.Path, Is.EqualTo("/test"));
    Assert.That(nockedRequest.Method, Is.EqualTo(Method.GET));
    Assert.That(nockedRequest.Body, Is.EqualTo("The body"));
    Assert.That(nockedRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    Assert.That(nockedRequest.Response, Is.EqualTo("<blah2>"));
    Assert.That(nockedRequest.ResponseHeaders.Count, Is.EqualTo(1));
    Assert.That(nockedRequest.ResponseHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
    Assert.That(nockedRequest.ResponseHeaders["Content-Type"], Is.EqualTo("application/json"));
    Assert.That(nockedRequest.RequestHeaders.Count, Is.EqualTo(1));
    Assert.That(nockedRequest.RequestHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
    Assert.That(nockedRequest.RequestHeaders["Content-Type"], Is.EqualTo("application/xml"));
}

[Test]
public void NockingAGetResponseCorrectlyBuildsNockedRequestObjectWhenNoContentTypeOrHeadersAreDefined()
{
    new nock("http://www.google.co.uk")
        .Get("/test")
        .Reply(HttpStatusCode.Created, "<blah2>");

    var nockedRequest = nock.NockedRequests.Last();
    Assert.That(nockedRequest, Is.Not.Null);

    Assert.That(nockedRequest.Url, Is.EqualTo("http://www.google.co.uk"));
    Assert.That(nockedRequest.Path, Is.EqualTo("/test"));
    Assert.That(nockedRequest.Method, Is.EqualTo(Method.GET));
    Assert.That(nockedRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    Assert.That(nockedRequest.Response, Is.EqualTo("<blah2>"));
    Assert.That(nockedRequest.ResponseHeaders.Count, Is.EqualTo(0));
    Assert.That(nockedRequest.RequestHeaders.Count, Is.EqualTo(0));
}

[Test]
public void NockingAGetResponseCorrectlyBuildsNockedRequestObjectWhenCustomHeadersAreSet()
{
    var headers = new NameValueCollection { { "Content-Type", "application/json" } };
    var requestHeaders = new NameValueCollection { { "Content-Type", "application/xml" } };

    new nock("http://www.google.co.uk")
        .Get("/test", "The body")
        .MatchHeaders(requestHeaders)
        .MatchHeader("cheese", "please")
        .Reply(HttpStatusCode.Created, "<blah2>", headers);

    var nockedRequest = nock.NockedRequests.Last();
    Assert.That(nockedRequest, Is.Not.Null);

    Assert.That(nockedRequest.Url, Is.EqualTo("http://www.google.co.uk"));
    Assert.That(nockedRequest.Path, Is.EqualTo("/test"));
    Assert.That(nockedRequest.Method, Is.EqualTo(Method.GET));
    Assert.That(nockedRequest.Body, Is.EqualTo("The body"));
    Assert.That(nockedRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    Assert.That(nockedRequest.Response, Is.EqualTo("<blah2>"));
    Assert.That(nockedRequest.ResponseHeaders.Count, Is.EqualTo(1));
    Assert.That(nockedRequest.ResponseHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
    Assert.That(nockedRequest.ResponseHeaders["Content-Type"], Is.EqualTo("application/json"));
    Assert.That(nockedRequest.RequestHeaders.Count, Is.EqualTo(2));
    Assert.That(nockedRequest.RequestHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
    Assert.That(nockedRequest.RequestHeaders["Content-Type"], Is.EqualTo("application/xml"));
    Assert.That(nockedRequest.RequestHeaders.AllKeys[1], Is.EqualTo("cheese"));
    Assert.That(nockedRequest.RequestHeaders["cheese"], Is.EqualTo("please"));
}

[Test]
public void NockingAPostResponseCorrectlyBuildsNockedRequestObject()
{
    var headers = new NameValueCollection { { "Content-Type", "application/json" } };

    new nock("http://www.google.co.uk")
        .Post("/test", "The body")
        .Reply(HttpStatusCode.NotFound, "<blah>", headers);

    var nockedRequest = nock.NockedRequests.Last();
    Assert.That(nockedRequest, Is.Not.Null);

    Assert.That(nockedRequest.Url, Is.EqualTo("http://www.google.co.uk"));
    Assert.That(nockedRequest.Path, Is.EqualTo("/test"));
    Assert.That(nockedRequest.Method, Is.EqualTo(Method.POST));
    Assert.That(nockedRequest.Body, Is.EqualTo("The body"));
    Assert.That(nockedRequest.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    Assert.That(nockedRequest.Response, Is.EqualTo("<blah>"));
    Assert.That(nockedRequest.ResponseHeaders.Count, Is.EqualTo(1));

    Assert.That(nockedRequest.ResponseHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
    Assert.That(nockedRequest.ResponseHeaders["Content-Type"], Is.EqualTo("application/json"));
}

[Test]
public void NockingAPutResponseCorrectlyBuildsNockedRequestObject()
{
    var headers = new NameValueCollection { { "x-Custom", "asdfe" } };

    new nock("http://www.anothergoogle.co.uk")
        .Put("/testing", "The body")
        .Reply(HttpStatusCode.Accepted, "<blah>", headers);

    var nockedRequest = nock.NockedRequests.Last();
    Assert.That(nockedRequest, Is.Not.Null);

    Assert.That(nockedRequest.Url, Is.EqualTo("http://www.anothergoogle.co.uk"));
    Assert.That(nockedRequest.Path, Is.EqualTo("/testing"));
    Assert.That(nockedRequest.Method, Is.EqualTo(Method.PUT));
    Assert.That(nockedRequest.Body, Is.EqualTo("The body"));
    Assert.That(nockedRequest.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
    Assert.That(nockedRequest.Response, Is.EqualTo("<blah>"));
    Assert.That(nockedRequest.ResponseHeaders.Count, Is.EqualTo(1));

    Assert.That(nockedRequest.ResponseHeaders.AllKeys[0], Is.EqualTo("x-Custom"));
    Assert.That(nockedRequest.ResponseHeaders["x-custom"], Is.EqualTo("asdfe"));
}

[Test]
public void NockingADeleteResponseCorrectlyBuildsNockedRequestObject()
{
    var headers = new NameValueCollection { { "Content-Type", "application/json" } };

    new nock("http://www.google.co.uk")
        .Delete("/test", "the body")
        .Reply(HttpStatusCode.Created, "<blah2>", headers);

    var nockedRequest = nock.NockedRequests.Last();
    Assert.That(nockedRequest, Is.Not.Null);

    Assert.That(nockedRequest.Url, Is.EqualTo("http://www.google.co.uk"));
    Assert.That(nockedRequest.Path, Is.EqualTo("/test"));
    Assert.That(nockedRequest.Method, Is.EqualTo(Method.DELETE));
    Assert.That(nockedRequest.Body, Is.EqualTo("the body"));
    Assert.That(nockedRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    Assert.That(nockedRequest.Response, Is.EqualTo("<blah2>"));
    Assert.That(nockedRequest.ResponseHeaders.Count, Is.EqualTo(1));

    Assert.That(nockedRequest.ResponseHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
    Assert.That(nockedRequest.ResponseHeaders["Content-Type"], Is.EqualTo("application/json"));
}

[Test]
public void NockingAHeadResponseCorrectlyBuildsNockedRequestObject()
{
    var headers = new NameValueCollection { { "Content-Type", "application/json" } };

    new nock("http://www.google.co.uk")
        .Head("/test")
        .Reply(HttpStatusCode.Created, "<blah2>", headers);

    var nockedRequest = nock.NockedRequests.Last();
    Assert.That(nockedRequest, Is.Not.Null);

    Assert.That(nockedRequest.Url, Is.EqualTo("http://www.google.co.uk"));
    Assert.That(nockedRequest.Path, Is.EqualTo("/test"));
    Assert.That(nockedRequest.Method, Is.EqualTo(Method.HEAD));
    Assert.That(nockedRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    Assert.That(nockedRequest.Response, Is.EqualTo("<blah2>"));
    Assert.That(nockedRequest.ResponseHeaders.Count, Is.EqualTo(1));

    Assert.That(nockedRequest.ResponseHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
    Assert.That(nockedRequest.ResponseHeaders["Content-Type"], Is.EqualTo("application/json"));
}

[Test]
public void NockingAPatchResponseCorrectlyBuildsNockedRequestObject()
{
    var headers = new NameValueCollection { { "Content-Type", "application/json" } };

    new nock("http://www.google.co.uk")
        .Patch("/test")
        .Reply(HttpStatusCode.Created, "<blah2>", headers);

    var nockedRequest = nock.NockedRequests.Last();
    Assert.That(nockedRequest, Is.Not.Null);

    Assert.That(nockedRequest.Url, Is.EqualTo("http://www.google.co.uk"));
    Assert.That(nockedRequest.Path, Is.EqualTo("/test"));
    Assert.That(nockedRequest.Method, Is.EqualTo(Method.PATCH));
    Assert.That(nockedRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    Assert.That(nockedRequest.Response, Is.EqualTo("<blah2>"));
    Assert.That(nockedRequest.ResponseHeaders.Count, Is.EqualTo(1));

    Assert.That(nockedRequest.ResponseHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
    Assert.That(nockedRequest.ResponseHeaders["Content-Type"], Is.EqualTo("application/json"));
}

[Test]
public void NockingAMergeResponseCorrectlyBuildsNockedRequestObject()
{
    var headers = new WebHeaderCollection { { "Content-Type", "application/xml" } };

    new nock("http://www.google.co.uk")
        .Merge("/test")
        .Reply(HttpStatusCode.Created, "<blah2>", headers);

    var nockedRequest = nock.NockedRequests.Last();
    Assert.That(nockedRequest, Is.Not.Null);

    Assert.That(nockedRequest.Url, Is.EqualTo("http://www.google.co.uk"));
    Assert.That(nockedRequest.Path, Is.EqualTo("/test"));
    Assert.That(nockedRequest.Method, Is.EqualTo(Method.MERGE));
    Assert.That(nockedRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    Assert.That(nockedRequest.Response, Is.EqualTo("<blah2>"));
    Assert.That(nockedRequest.ResponseHeaders.Count, Is.EqualTo(1));

    Assert.That(nockedRequest.ResponseHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
    Assert.That(nockedRequest.ResponseHeaders["Content-Type"], Is.EqualTo("application/xml"));
}

[Test]
public void NockingAGetResponseWhichRepliesWithAStandardResponseCorrectlyBuildsNockedRequestObject()
{
    var headers = new NameValueCollection { { "Content-Type", "application/json" } };

    new nock("http://www.google.co.uk")
        .Get("/test")
        .ContentType("application/json")
        .Reply(HttpStatusCode.Created, "<blah2>", headers);

    var nockedRequest = nock.NockedRequests.Last();
    Assert.That(nockedRequest, Is.Not.Null);

    Assert.That(nockedRequest.Url, Is.EqualTo("http://www.google.co.uk"));
    Assert.That(nockedRequest.Path, Is.EqualTo("/test"));
    Assert.That(nockedRequest.Method, Is.EqualTo(Method.GET));
    Assert.That(nockedRequest.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    Assert.That(nockedRequest.Response, Is.EqualTo("<blah2>"));
    Assert.That(nockedRequest.ResponseHeaders.Count, Is.EqualTo(1));

    Assert.That(nockedRequest.ResponseHeaders.AllKeys[0], Is.EqualTo("Content-Type"));
    Assert.That(nockedRequest.ResponseHeaders["Content-Type"], Is.EqualTo("application/json"));
}

#endregion General valid nock building tests

}
}
