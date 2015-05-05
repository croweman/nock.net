# Nock.net

Nock.net is an HTTP mocking library for .Net

Nock.net can be used to aid in testing modules that perform HTTP requests in isolation.

## Table of contents

**[Install](#install)**  
**[Use](#use)**  
***[Specifying headers](#specifying-headers)***
**[Expectations](#expectations)**  
**[How does it work?](#how-does-it-work)**  
**[License](#license)**  


## Install

Either reference the Nock.net assembly or Install from nuget.

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

### Specifying headers

```

```c#

Nock.ClearAll();

```

Returning specific test responses

## Expectations

isDone

## How does it work?

The Nock assembly provides wrapper objects over the standard System.Net HttpWebResponse and HttpWebRequest objects.

When Nocks have been created in your tests then relevant TestHttpWebResponse objects will be returned.

## License

(The MIT License)

Copyright (c) 2011-2015 Pedro Teixeira. http://about.me/pedroteixeira

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
