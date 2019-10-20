# LazyCache.Testing
__*Moq and NSubstitute mocking libraries for LazyCache*__

If you want to mock the LazyCache caching service using Moq or NSubstitute you're in the right place. I've got you covered.

## Hang on a minute... but but why? LazyCache already provides a test class to do this?
Yes, LazyCache does provide a ```MockCachingService``` for unit testing. It works as advertised for some cases __however__ it doesn't work for all.

If the SUT is only using Get\<T> you're out of luck. Add\<T> followed by a Get\<T> won't do anything either.

LazyCache.Testing.Moq and LazyCache.Testing.NSubstitute are the have your cake and eat it too solution. They are a cross between a mocked caching service and an in-memory provider. Some of the features include:
- If the SUT populates the cache using Add\<T>, GetOrAdd\<T> or GetOrAddAsync\<T> no explicit set up is required, it just works. Create the mock and consume.
- Add and remove invocations actually do something (e.g., Add will add a new cache entry or update an existing cache entry regardless of whether you have explicitly set it up or not).
- The ability to provide explicit set ups if you want (using SetUpCacheEntry\<T>).
- Access to all of the good stuff that these great mocking libraries provide such as Moq ```Verify```, NSubstitute ```Received``` etc. 

## Resources
- [Source repository](https://github.com/rgvlee/LazyCache.Testing)
- [LazyCache.Testing.Moq - NuGet](https://www.nuget.org/packages/LazyCache.Testing.Moq/)
- [LazyCache.Testing.NSubstitute - NuGet](https://www.nuget.org/packages/LazyCache.Testing.NSubstitute/)

## The disclaimer
I have built these libraries for fun (well sort of, I needed the Moq implementation for work). The mocking doesn't extend to the CacheProvider so if you're hitting that explicitly then you're in for a world of nulls. I might add it in a later release but in general I think accessing the CacheProvider should be avoided. If you find these libraries useful and something is missing, not working as you'd expect or you need additional behaviour mocked flick me a message and I'll see what I can do.

# Moq
## Basic usage
- Create a mocked caching service
- Consume

```
[Test]
public virtual void MinimumViableInterface_Guid_ReturnsExpectedResult()
{
    var cacheEntryKey = "SomethingInTheCache";
    var expectedResult = Guid.NewGuid().ToString();

    var mockedCache = Create.MockedCachingService();

    var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

    Assert.AreEqual(expectedResult, actualResult);
}
```

## But I want the cache mock
No problem. Use Mock.Get(mockedCache) to get the cache mock.

## I'm using Get\<T>, what do I need to do?
The mock needs to know what to return. You'll need to either:
- Populate the cache using Add\<T>, GetOrAdd\<T> or GetOrAddAsync\<T>; or
- Use the explicit set up as described below.

## Let's get explicit
If you want to explicitly set up a cache entry use the following extension method:

```
public virtual void GetOrAddWithSetUp_Guid_ReturnsExpectedResult()
{
    var cacheEntryKey = "SomethingInTheCache";
    var expectedResult = Guid.NewGuid().ToString();

    var mockedCache = Create.MockedCachingService();
    mockedCache.SetUpCacheEntry(cacheEntryKey, expectedResult);

    var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

    Assert.AreEqual(expectedResult, actualResult);
}
```

## Async? Please tell me you support the async methods.
The survey says, yes the async methods are supported. You're welcome.

# NSubstitute
It works the same/has the same interface. The only difference is if you want to cache mock. For Moq you need to invoke Mock.Get(mockedCache) to get it. For NSubstitute you don't need to do this.