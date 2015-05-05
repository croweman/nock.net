# Nock.net

Nock.net is an HTTP mocking library for .Net

Nock.net can be used to aid in testing modules that perform HTTP requests in isolation.

Nock.net.HttpWebRequest and Nock.net.IHttpWebResponse objects need to be used instead of the Standard System.Net HttpWebRequest and HttpWebResponse.

It came to life because of nock in the node js world.

## Table of contents

- [Install](#install)  
- [Use](#use)  
  - [Specifying request body](#specifying-request-body)  
  - [Replying with exceptions](#replying-with-exceptions)
  - [Replying with more detailed responses](#Replying-with-more-detailed-responses)
  - [Specifying headers](#specifying-headers)
    - [Specifying request headers](#specifying-request-headers)
    - [Specifying reply headers](#specifying-reply-headers)
  - [Specifying content type](#specifying-content-type)
  - [Repeat response n times](#repeat-response-n-times)
  - [Real world example](#real-world-example)
- [Expectations](#expectations)  
- [Restoring](#restoring)  
- [How does it work?](#how-does-it-work)  
- [License](#license)  


## Install

Either reference the Nock.net assembly or Install from [nuget](https://www.nuget.org/packages/Nock.net/).

## Use

In your test you can setup a mocking object like the following:

```c#
using Nock.net;

[Test]
public void Test()
{
    var nock = new Nock("http://domain.com")
        .Get("/users/1")
        .Reply(HttpStatusCode.OK, "{ value: 5 }");
}
```

You would then need implementation logic that would Create a Nock.net.HttpWebRequest and retrieve a response like this:

```#
Nock.net.HttpWebRequest request = Nock.net.HttpWebRequest.CreateRequest("https://domain.com/users/1");
request.Method = "GET";

Nock.net.IHttpWebResponse response = request.GetResponse();;
```

Nock.net.HttpWebRequest and Nock.netHttpWebResponse objects are wrappers over the System.Net.HttpWebResponse and System.Net.HttpWebRequest objects.

### Specifying request body

You can specify the request body to be matched as the second argument to the Get, Post, Put or Delete specifications like this:

```c#
var nock = new Nock("http://domain.com")
    .Get("/users/1", "{ add: \"1 + 4\" }")
    .Reply(HttpStatusCode.OK, "{ value: 5 }");
```

If no request body is defined on the Nock then the body will not be used for matching

### Replying with exceptions

You can reply with an exception like this:

```c#
var nock = new Nock("http://domain.com")
    .Get("/users/1")
    .Reply(new WebException("An unexpected exception occurred"));
```

### Replying with more detailed responses

```c#
var response = new TestHttpWebResponse("The body")
{
    StatusCode = HttpStatusCode.Created,
    ContentType = "application/json",
    CharacterSet = "blah"
};

var nock = new Nock("http://domain.com")
    .Get("/users/1")
    .Reply(response);
```

### Specifying headers

Header field names are case-insensitive

### Specifying request headers

You can specify the request headers to be matched against like this:

```c#
var webHeaders = new WebHeaderCollection { { "x-custom", "value" } };

var nock = new Nock("http://domain.com")
   .Get("/users/1")
   .RequestHeaders(webHeaders)
   .Reply(HttpStatusCode.OK, "{ value: 5 }");
```

If no request headers are defined on the Nock then the request headers will not be used for matching

### Specifying reply headers

You can specify the reply headers like this:

```c#
var reponseHeaders = new WebHeaderCollection { { "x-custom", "value" } };

var nock = new Nock("http://domain.com")
   .Get("/users/1")
   .Reply(HttpStatusCode.OK, "{ value: 5 }", responseHeaders);
```

### Specifying content type

If a content type is defined on a Nock then the request content type will be used for matching

```c#
var nock = new Nock("http://domain.com")
   .Get("/users/1")
   .ContentType("application/json")
   .Reply(HttpStatusCode.OK, "{ value: 5 }")
   .Times(3);
```

### Repeat response n times

You are able to specify the number of times to repeat the same response.

```c#
var nock = new Nock("http://domain.com")
   .Get("/users/1")
   .Reply(HttpStatusCode.OK, "{ value: 5 }")
   .Times(3);
```

### Restoring

You can remove any previously unused nocks like this:

```c#
Nock.ClearAll();
```

## Expectations

You can determine whether a nock was called like this:

```c#
var nock = new Nock("http://domain.com")
   .Get("/users/1")
   .Reply(HttpStatusCode.OK, "{ value: 5 }")

....

Assert.That(nock.Done(), Is.True);
```

### Real world example

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
      new Nock("https://domain-name.com")
         .ContentType("application/json; encoding='utf-8'")
         .Post("/api/v2/action/")
         .Reply(new WebException("This is a web exception"));
   }
   else
   {
      new Nock("https://domain-name.com")
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
   catch (Exception)
   {
      status = Status.Error;
   }
   finally
   {
      if (response != null)
         response.Dispose();
   }

   return status;
}
```
## How does it work?

The Nock assembly provides wrapper objects over the standard System.Net HttpWebResponse and HttpWebRequest objects.

When Nocks have been created in your tests then relevant Nock.net.TestHttpWebResponse objects will be returned, these objects implement the Nock.net.IHttpWebResponse interface.

## License

(The MIT License)

Copyright (c) 2015  Lee Crowe

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
