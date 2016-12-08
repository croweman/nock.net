namespace Nock.net
{
    internal enum Method
    {
        GET,
        POST,
        PUT,
        DELETE,
        HEAD,
        PATCH,
        MERGE,
        NotSet
    }

    internal enum HeaderMatcher
    {
        None,
        Match,
        ExactMatch
    }

    internal enum BodyMatcher
    {
        None,
        Body,
        StringFunc,
        TypedFunc
    }

    internal enum QueryMatcher
    {
        None,
        NameValue,
        NameValueExact,
        Func,
        Bool
    }
}
