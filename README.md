# LazyCache.Testing
__*Moq and NSubstitute mocking libraries for LazyCache*__

## The cache experience
LazyCache.Testing.Moq and LazyCache.Testing.NSubstitute are more than just a few mock set ups. They mock the LazyCache caching service as a whole.

Some of the features include:
- If the SUT populates the cache using `Add<T>`, `GetOrAdd<T>` or `GetOrAddAsync<T>` no explicit set up is required; it just works. Create the mock and consume.
- Changes to the caching service do something (e.g., `Add` will add a new cache entry or update an existing cache entry regardless of whether you have explicitly set it up or not).
- The ability to provide explicit set ups if you want (using `SetUpCacheEntry<T>`).
- Access to all of the good stuff that these great mocking libraries provide such as Moq ```Verify```, NSubstitute ```Received``` etc. 

## But why? Doesn't LazyCache provide a test class to do this?
Yes, LazyCache does provide a `MockCachingService` for unit testing. It works as advertised for some cases __however__ it doesn't work for all. If the SUT is only using `Get<T>` you're out of luck. `Add<T>` followed by a `Get<T>` won't do anything either.

LazyCache.Testing.Moq and LazyCache.Testing.NSubstitute solve that problem by mocking this functionality, providing a system mock that is functionally equivalent to the real thing.

## Resources
- [Source repository](https://github.com/rgvlee/LazyCache.Testing)
- [LazyCache.Testing.Moq - NuGet](https://www.nuget.org/packages/LazyCache.Testing.Moq/)
- [LazyCache.Testing.NSubstitute - NuGet](https://www.nuget.org/packages/LazyCache.Testing.NSubstitute/)

## The disclaimer
I have built these libraries for fun (well sort of, I needed the Moq implementation for work). The mocking doesn't extend to the `CacheProvider` so if you're hitting that explicitly then you're in for a world of nulls. I might add it in a later release but in general I think accessing the `CacheProvider` should be avoided.

If you find these libraries useful and something is missing, not working as you'd expect, or you need additional behaviour mocked, flick me a message and I'll see what I can do.

# Moq
## Basic usage
- Create a mocked caching service
- Consume

``` C#
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

## But I want the caching service mock!
No problem. Use `Mock.Get(mockedCache)` to get the caching service mock.

## My SUT invokes the caching service `Get<T>` method, is there anything specific I need to do?
The mock needs to know what to return. You'll need to either:
- Populate the cache using `Add<T>`, `GetOrAdd<T>` or `GetOrAddAsync<T>`; or
- Use the explicit set up as described below.

## Explicit caching service cache entry set up
If you want to manually set up a cache entry use the following extension method:

``` C#
[Test]
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

## Are the async methods supported?
Yes.

# NSubstitute
It works the same/has the same interface. The only difference is if you want the caching service mock. For Moq you need to invoke `Mock.Get(mockedCache)` to get it. For NSubstitute you don't need to do this.