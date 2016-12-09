# Nock.net

Nock.net is an HTTP mocking and expectations library for .Net

Nock.net can be used to aid in testing modules that perform HTTP requests in isolation.

For instance, if a module performs HTTP requests to a CouchDB server or makes HTTP requests to the Amazon API, you can test that module in isolation.

It came to life because of nock in the node js world.

## Table of contents

- [Install](#install)  
- [How does it work?](#how-does-it-work) 
- [Use](#use)  
  - [READ THIS! - About interceptors](#about-interceptors)
  - [Specifying request body](#specifying-request-body)  
  - [Replying with exceptions](#replying-with-exceptions)
  - [Replying with more detailed responses](#replying-with-more-detailed-responses)
  - [Specifying headers](#specifying-headers)
    - [Specifying request headers](#specifying-request-headers)
    - [Specifying reply headers](#specifying-reply-headers)
  - [Specifying request query string](#specifying-request-query-string)
  - [Specifying content type](#specifying-content-type)
  - [Repeat response n times](#repeat-response-n-times)
  - [Wildcard URL](#wildcard)
  - [Real world example](#real-world-example)
- [Expectations](#expectations)  
- [Restoring](#restoring)  
- [Logging](#logging) 
- [Recording](#recording) 
- [Request timeout in milliseconds](#request-timeout) 
- [Set default credentials](#set-default-credentials)
- [Test setup and teardown](#setup-teardown) 
- [License](#license)  


## Install<a name="install"></a>

Either reference the Nock.net assembly or Install from [nuget](https://www.nuget.org/packages/Nock.net/).

!! The test runner and CI build agent will need to run with ADMINISTRATOR privileges as the tests will create a web proxy for web request on *:8080 !!

## How does it work?<a name="how-does-it-work"></a>

Once a nocked request has been created a web proxy will be created as a default on all .Net WebRequest objects that routes all requests within the running application through to http://localhost:8080.

Nock.net creates a listener that listens to all web requests on *:8080 and tries to find a nocked response.  If a matching nocked response is found this is returned to the caller otherwise it tries to send the request out to the outside world.  If the request then fails a 417 response is returned.

This proxy will be set as a default for all web requests.  If the running application is not using ADMINISTRATOR privileges or is setting alternative proxies on web requests the library will not work! 

## Use<a name="use"></a>

In your test you can setup a mocking object like the following:

```c#
using Nock.net;

[Test]
public void Test()
{
    var nock = new nock("http://domain.com")
        .Get("/users/1")
        .Reply(HttpStatusCode.OK, "{ value: 5 }");
}
```

You would then need implementation logic that would Create a HttpWebRequest and retrieve a response like this:

```#
var request = WebRequest.Create("http://domain-name.com/users/1") as HttpWebRequest;
request.Method = "GET";
var response = request.GetResponse();
```

### READ THIS! - About interceptors<a name="about-interceptors"></a>

When you setup an interceptor for an URL and that interceptor is used, it is removed from the interceptor list. This means that you can intercept 2 or more calls to the same URL and return different things on each of them. It also means that you must setup one interceptor for each request you are going to have, otherwise nock will throw an error because that URL was not present in the interceptor list.

However, an interceptor can be used more than once if n Times is defined.

!! The test runner and CI build agent will need to run with ADMINISTRATOR privileges as the tests will create a web proxy for web request on localhost:8080 !!

### Specifying request body<a name="specifying-request-body"></a>

You can specify the request body to be matched as the second argument to the Post, Put, Merge and Patch specifications like this:

```c#
var nock = new nock("http://domain.com")
    .Post("/users/1", "{ add: \"1 + 4\" }")
    .Reply(HttpStatusCode.OK, "{ value: 5 }");
```

Or you can define a custom static or inline delegate function to do custom body matching:

```c#
var nock = new nock("http://domain.com")
    .Post("/users/1", (body) => { return body.Contains("Hello"); })
    .Reply(HttpStatusCode.OK, "{ value: 5 }");
```

You can also deserialize the response into a typed object to do typed filtering

```c#
public class TestObj
{
    public string Action { get; set; }
}

var nock = new nock("http://domain.com")
    .Post<TestObj>("/api/v2/action/", (testObj) => { return testObj.Action == "Blah"; })
    .Reply(HttpStatusCode.OK, "{ value: 5 }");
```

If no request body is defined on the nock then the body will not be used for matching

### Replying with exceptions<a name="replying-with-exceptions"></a>

You can reply with an exception like this:

```c#
var nock = new nock("http://domain.com")
    .Get("/users/1")
    .Reply(HttpStatusCode.BadGatway, string.Empty);
```

### Replying with more detailed responses<a name="replying-with-more-detailed-responses"></a>

```c#
var nock = new nock("http://domain.com")
    .Get("/users/1")
    .Reply(HttpStatusCode.OK, "The body", new NameValueCollection { { "x-custom", "value" } });
```

You can also use a static delegate or inline function to define the response headers and body

```c#
var nock = new nock("http://domain.com")
    .Get("/users/1")
    .Reply(HttpStatusCode.OK, (requestDetails) =>
    {
        var headers = new NameValueCollection();
        headers.Add("crowe", "man");

		var body = string.Format("{0}-{1}-{2}-{3}", requestDetails.URL, requestDetails.Headers, requestDetails.Query, requestDetails.Body);

        return new WebResponse(headers, body);
    });
```

### Specifying headers<a name="specifying-headers"></a>

### Specifying request headers<a name="specifying-request-headers"></a>

You can specify the request headers to be matched against like this:

```c#
var webHeaders = new NameValueCollection { { "x-custom", "value" } };

var nock = new nock("http://domain.com")
   .Get("/users/1")
   .MatchHeaders(webHeaders)
   .Reply(HttpStatusCode.OK, "{ value: 5 }");
```

You can also match headers individually:

```c#
var webHeaders = new NameValueCollection { { "x-custom", "value" } };

var nock = new nock("http://domain.com")
   .Get("/users/1")
   .MatchHeader("x-custom", "value")
   .MatchHeader("x-custom-2", "value-2")
   .Reply(HttpStatusCode.OK, "{ value: 5 }");
```

Or you can define a custom static or inline delegate function to do custom filtering

```c#
var webHeaders = new NameValueCollection { { "x-custom", "value" } };

var nock = new nock("http://domain.com")
   .Get("/users/1")
   .MatchHeaders((headers) => { return headers["x-custom"] == "value"; })
   .Reply(HttpStatusCode.OK, "{ value: 5 }");
```

If no request headers are defined on the Nock then the request headers will not be used for matching

### Specifying reply headers<a name="specifying-reply-headers"></a>

You can specify the reply headers like this:

```c#
var reponseHeaders = new NameValueCollection { { "x-custom", "value" } };

var nock = new nock("http://domain.com")
   .Get("/users/1")
   .Reply(HttpStatusCode.OK, "{ value: 5 }", responseHeaders);
```
### Specifying request query string<a name="specifying-request-query-string"></a>

Nock understands query strings. Instead of placing the entire URL, you can specify the query part as an object:

If the url being requested has a query string but you are not concerned about it's values and still want to match the request in the following way:

```c#
var nock = new nock("http://domain.com")
    .Get("/users/1")
	.Query(true)
    .Reply(HttpStatusCode.OK, "Hello");
```

You can also match the query using a NameValueCollection:

```c#
var nock = new nock("http://domain.com")
    .Get("/users/1")
	.Query(new NameValueCollection { { "test", "1" } })
    .Reply(HttpStatusCode.OK, "Hello");
```

Or you could do custom matching using a static delegate or inline function

```c#
var nock = new nock("http://domain.com")
    .Get("/users/1")
	.Query((queryDetails) => { return queryDetails.Query["test"] == "2"; })
    .Reply(HttpStatusCode.OK, "Hello");
```

If no Query is defined on the nock and requested url contains a query string no match will be made.

### Specifying content type<a name="specifying-content-type"></a>

If a content type is defined on a Nock then the request content type will be used for matching

```c#
var nock = new nock("http://domain.com")
   .Get("/users/1")
   .ContentType("application/json")
   .Reply(HttpStatusCode.OK, "{ value: 5 }")
```

### Repeat response n times<a name="repeat-response-n-times"></a>

You are able to specify the number of times the same nock can be used.

```c#
var nock = new nock("http://domain.com")
   .Get("/users/1")
   .Reply(HttpStatusCode.OK, "{ value: 5 }")
   .Times(3);
```

### Wildcard URL<a name="wildcard"></a>

Wild card URL's can be used, basically asterix symbols can be used for values that may differ for each request

```c#
var nock = new nock("http://domain.com")
   .Get("/users/*/information")
   .Reply(HttpStatusCode.OK, "{ value: 5 }")
```

### Real world example<a name="real-world-example"></a>

Below is a real world usage sample with test and implementation code.

```c#
[TestCase("Added", Status.OK)]
[TestCase("User not allowed", Status.Forbidden)]
[TestCase("User could not be found", Status.NotFound)]
[TestCase("Something went wrong", Status.Error)]
[Test]
public void NockingAResponseCorrectlyReturnsRelevantResponses(string resultMessage, Status expectedStatus)
{
    var responseJson = string.Format("{{ result: \"{0}\" }}", resultMessage);

    new nock("http://domain-name.com")
        .ContentType("application/json; encoding='utf-8'")
        .Post("/api/v2/action/")
        .Reply(HttpStatusCode.OK, responseJson);

    var postResult = PostDataToAnEndpointAndProcessTheResponse();

    Assert.That(postResult.Status, Is.EqualTo(expectedStatus));
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

public class PostResult
{
    public Status Status { get; set; }
    public string ErrorMessage { get; set; }
}
```

### Restoring<a name="restoring"></a>

You can remove any previously unused nocks like this:

```c#
nock.ClearAll();
```

Or you can remove a specific nock like the following:

```c#
nock.RemoveInterceptor(nockedRequest);
```

## Expectations<a name="expectations"></a>

You can determine whether a nock was called like this:

Done returns true if the nock has been returned the required number of times.

```c#
var nock = new nock("http://domain.com")
   .Get("/users/1")
   .Reply(HttpStatusCode.OK, "{ value: 5 }")

....

Assert.That(nock.Done(), Is.True);
```

## Logging<a name="logging"></a>

Nock can log matches if you pass in a log function like this:

```c#
var nock = new nock("http://domain.com")
   .Get("/users/1")
   .Log(System.Console.WriteLine)
   .Reply(HttpStatusCode.OK, "{ value: 5 }")
```

## Recording<a name="recording"></a>

When determining what nocks to create an experimental recorder can be used to record and output example nocked requests could be created.

```c#
// By default all requests will be output to console, but you can turn this off with boolean argument.
nock.Recorder.Record();

// To get output of Recording
var output = nock.Recorder.GetRecording();

// To stop recording
nock.Recorder.Stop();
```

## Request timeout in milliseconds<a name="request-timeout"></a>

When requests are proxied through the nock listener if no nocked match is found the request will be forwarded on by the listener.

A default web timeout can be defined in the following manner.

```c#
nock.RequestTimeoutInMilliseconds = 10000;
```

## Set default credentials<a name="set-default-credentials"></a>

By default on all WebRequests default credentials will be set, this can be turned off like the following:

```c#
nock.SetDefaultCredentials = false;
```

## Test setup and teardown<a name="setup-teardown"></a>

It can be useful to configure nock before all tests run and cleanup (stop the listener) after all tests complete.

If nock is not stopped then the listener may hang around listening on *:8080

```c#
using NUnit.Framework;
using Nock.net;

[SetUpFixture]
public class Setup
{
    [OneTimeSetUp]
    public void Configure()
    {
        nock.RequestTimeoutInMilliseconds = 10000;
    }

    [OneTimeTearDown]
    public void StopNock()
    {
        nock.Stop();
    }
}
```

## License<a name="license"></a>

(The MIT License)

Copyright (c) 2015  Lee Crowe

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
