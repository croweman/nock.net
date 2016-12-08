
using NUnit.Framework;
using Nock.net;

[SetUpFixture]
public class Setup
{
    [OneTimeSetUp]
    public void Configure()
    {   
        nock.RequestTimeoutInMilliseconds = 1000;
    }
    [OneTimeTearDown]
    public void StopNock()
    {
        nock.Stop();
    }
}