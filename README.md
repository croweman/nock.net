# Nock.net

Nock.net is an HTTP mocking and expectations library for .Net

Nock.net can be used to aid in testing modules that perform HTTP requests in isolation.

For instance, if a module performs HTTP requests to a CouchDB server or makes HTTP requests to the Amazon API, you can test that module in isolation.

It came to life because of nock in the node js world.


## Table of contents

- [Install](#install)  
- [How does it work?](#how-does-it-work) 
- [Use](#use)  
  - [Specifying request body](#specifying-request-body)  
  - [Replying with exceptions](#replying-with-exceptions)
  - [Replying with more detailed responses](#replying-with-more-detailed-responses)
  - [Specifying headers](#specifying-headers)
    - [Specifying request headers](#specifying-request-headers)
    - [Specifying reply headers](#specifying-reply-headers)
  - [Specifying content type](#specifying-content-type)
  - [Repeat response n times](#repeat-response-n-times)
  - [Real world example](#real-world-example)
- [Expectations](#expectations)  
- [Restoring](#restoring)  
- [License](#license)  


## Install<a name="install"></a>

Either reference the Nock.net assembly or Install from [nuget](https://www.nuget.org/packages/Nock.net/).

!! The test runner and CI build agent will need to run with ADMINISTRATOR privileges as the tests will create a web proxy for web request on localhost:8080 !!

## How does it work?<a name="how-does-it-work"></a>

Once a nocked request has been created a web proxy will be created that routes all requests within the running application through to http://localhost:8080.

Nock.net creates a listener that listens to all web requests and tries to find a nocked response.  If a matching nocked response is found this is returned to the caller otherwise it tries to send the request out to the outside world.  If the request then fails a 417 response is returned.

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

### Specifying request body<a name="specifying-request-body"></a>

You can specify the request body to be matched as the second argument to the Get, Post, Put or Delete specifications like this:

```c#
var nock = new nock("http://domain.com")
    .Get("/users/1", "{ add: \"1 + 4\" }")
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

### Real world example<a name="real-world-example"><a/>

Below is a real world usage sample with test and implementation code.

```c#
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
      
      new nock("https://domain-name.com")
         .ContentType("application/json; encoding='utf-8'")
         .Post("/api/v2/action/")
         .Reply(new WebException("This is a web exception", null, WebExceptionStatus.UnknownError, testHttpWebResponse));
   }
   else
   {
      new nock("https://domain-name.com")
         .ContentType("application/json; encoding='utf-8'")
         .Post("/api/v2/action/")
         .Reply(HttpStatusCode.OK, responseJson);                
   }

   var status = PostDataToAnEndpointAndProcessTheResponse();

   Assert.That(status, Is.EqualTo(expectedStatus));

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

public Status PostDataToAnEndpointAndProcessTheResponse()
{
   var status = Status.OK;

   var postData =
      "{" +
         "Action: \"AddFunds\"," +
         "FirstName: \"Joe\"," +
         "Surname: \"Bloggs\"" +
         "Amount: 50.95" + 
      "}";

   var bytes = Encoding.UTF8.GetBytes(postData);

   var request = Nock.net.HttpWebRequest.CreateRequest("https://domain-name.com/api/v2/action/");
   request.ContentType = "application/json; encoding='utf-8'";
   request.ContentLength = bytes.Length;
   request.Method = "POST";

   using (var requestStream = request.GetRequestStream())
   {
      requestStream.Write(bytes, 0, bytes.Length);
      requestStream.Close();
   }

   Nock.net.IHttpWebResponse response = null;

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
               status = Status.OK;
               break;
            case "User not allowed":
               status = Status.Forbidden;
               break;
            case "User could not be found":
               status = Status.NotFound;
               break;
            default:
               status = Status.Error;
               break;
         }
      }
      else
         status = Status.Error;
   }
   catch (WebException ex)
   {
      var errorMessage = string.Format("An error occurred: {0}, Http status code: {1}", ex.Message, ex.Response.Headers["Status-Code"]);
      status = Status.Error;
   }
   finally
   {
      if (response != null)
         response.Dispose();
   }

   return status;
}

public class CustomHttpWebResponse : WebResponse
{
   private readonly NameValueCollection _headers = new NameValueCollection();

   public override WebHeaderCollection Headers
   {
      get
      {
        return _headers;
      }
   }
}
```

## License<a name="license"></a>

(The MIT License)

Copyright (c) 2015  Lee Crowe

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
