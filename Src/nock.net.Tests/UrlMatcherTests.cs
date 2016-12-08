using NUnit.Framework;

namespace Nock.net.Tests
{
    [TestFixture]
    public class UrlMatcherTests
    {
        [Test]
        [TestCase(true, "http://www.cheese.com/", "http://www.cheese.com", "/")]
        [TestCase(false, "http://www.cheese.com/a", "http://www.cheese.com", "/")]
        [TestCase(true, "http://www.cheese.com/asdf", "http://www.cheese.com", "/asdf")]
        [TestCase(true, "http://www.cheese.com/sdffasdf/asdf", "http://www.cheese.com", "/*")]
        [TestCase(true, "http://www.cheese.com/blah/asdf", "http://www.cheese.com", "/*/asdf")]
        [TestCase(true, "http://www.cheese.com/blah/asdfs", "http://www.cheese.com", "/*/asdfs")]
        [TestCase(true, "http://www.cheese.com/some/chips/fish/beans/peas", "http://www.cheese.com", "/*/fish/*/peas")]
        [TestCase(false, "http://www.cheese.com/some/chips/fish2/beans/peas", "http://www.cheese.com", "/*/fish/*/peas")]
        [TestCase(true, "http://www.cheese.com/fish?tags=asdf", "http://www.cheese.com", "/fish?tags=*")]
        [TestCase(false, "http://www.cheese.com/fish?tags=*", "http://www.cheese.com", "/fish/tags=*")]
        [TestCase(true, "http://www.cheese.com/fish?tags=*", "http://www.cheese.com", "/fish?tags=*")]
        [TestCase(true, "http://www.cheese.com/fish/?tags=*", "http://www.cheese.com", "/fish/?tags=*")]
        [TestCase(true, "http://www.cheese.com/fish?tags=one,two&peas=1", "http://www.cheese.com", "/fish?tags=*&peas=1")]
        [TestCase(false, "http://www.cheese.com/fish?tags=one,two&peas=2", "http://www.cheese.com", "/fish?tags=*&peas=1")]
        [TestCase(true, "http://www.domain-name.com/one/?location=true", "http://www.domain-name.com", "/*/?location=true")]
        public void TestUrlMatching(bool expectation, string requestUrl, string nockedRequestUrl, string nockedRequestPath)
        {

            var webRequest = new NockHttpWebRequest() { RequestUri = requestUrl };
            var nockedRequest = new NockedRequest(nockedRequestUrl) { Path = nockedRequestPath };

            var result = UrlMatcher.IsMatch(webRequest, nockedRequest);
            Assert.That(result, Is.EqualTo(expectation));
        }
    }
}
