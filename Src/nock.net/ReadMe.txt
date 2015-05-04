Should be able to set content type in nock

when actually returned nocked response, should check url, method, and content type if defined on request??

should set headers on response etc?????????

what about specifying errors with error messages to throw, type and message

future versions,
response cookies? more stuff?

Need to test or implement many of the properties and methods etc like get stream blah blah in test version

TestNockWebRequest : INockHttpWebREquest
TestNockWebResponse : INockHttpWebResponse

release build


test for number of times, times 2 but loop three have counter expect 2 to be successful and 1 to fail

if times 1, what happens, if you do times 2 does that do 2 or 3 in total????

TestNockHttpWebResponse

static clear all, test it

reply testNockWebResponse

all methods in test one should be virtual so could be overriden, constructure should take response string
what about content length etc etc?

retrieve/ifind method which finds by method, content type etc should lock object blah blah the whole method


should I keep them called INock and NockHttp, maybe rename???

Url and Uri overload on create

test
populate assembly info properly
github and nuget package

git ignore, bin obj, etc, packages?

Test getresponse header?

Need to test the real objects too!!!!!!!!! don't just test nocks'

** TEST New reply variation
Test without setting a content type and confirm it's empty 
TEST THAT ReponseDetail is built correclty in all variations of reply!!!!!!!!

# Test the responses that are built are correct in all variations, test content type multiple nocks and diff responses, headers
etc returned, blah blah blah blah
# 3 reply variations
get response header afunctino

test multiple times etc, or with no nocks and exception an error to occur because can't connnect ot non existing domain

integration based test etc

test FindTestHttpWebResponse remove false overload

documentation mention disposable etc, check npm nock
show working examples in real code, then the test code around it with nocks etc
exception example, custom and lightweight blah blah
alernative to dependency injection
does not currently work with async method calls or transport context etc
contenttype, times, reply variations, method etc
clear all method


internal sealed, private etc

reading stream etc etc

if (real example code wrap in usings etc)

integration

// test times 2, maybe 2 times, but for loop of 2 except 2 to be valid 1 to fail




        // exception, blah and custom
        // test no match if method differs on request
        // test if no method defined etc
        // test not found if url or content type differs
        // assert variations on content type etc, assert removed
        // test content type defined in response
        // test with non matching paths etc
        // test times 2, maybe 2 times, but for loop of 2 except 2 to be valid 1 to fail

        // test read response etc