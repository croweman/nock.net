
reqheaders!!, must have ones that are defined

tests around request headers, nock built building response details, building tests building responses etc and finding tests


specificying headers to match on
specify post data to match on


You can specify the request body to be matched as the second argument to the get, post, put or delete specifications like this:

All tests around the above, acceptance tests too


finding for request

 public static string RequestBody()
    {
        var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
        bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        var bodyText = bodyStream.ReadToEnd();
        return bodyText;
    }


future versions,
response cookies? more stuff?



release build
populate assembly info properly
github and nuget package



documentation mention disposable etc, check npm nock
show working examples in real code, then the test code around it with nocks etc
exception example, custom and lightweight blah blah
alernative to dependency injection
does not currently work with async method calls or transport context etc
contenttype, times, reply variations, method etc
clear all method