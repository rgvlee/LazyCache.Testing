# LazyCache.Testing.Moq/LazyCache.Testing.NSubstitute
__*Moq and NSubstitute mocking libraries for LazyCache*__

If you want to mock the LazyCache caching service using Moq or NSubstitute rather than use the built-in fake provided by LazyCache, you're in the right place. I've got you covered.

## Hang on a minute... but but why? LazyCache already provides a test class to do this?
Yes, LazyCache does provide a ```MockCachingService``` for unit testing. It works as advertised for most cases __however__ it doesn't work for all.

If you're using Get\<T> you're out of luck. You'll get nulls.

LazyCache.Testing.Moq and LazyCache.Testing.NSubstitute are the have your cake and eat it too solution. They provide:
- Out of the box functionality if you are only using GetOrAdd and GetOrAddAsync. No explicit set up required, it just works.
- If you do need support for other methods e.g., Get\<T>, use the included fluent extension method to do so.
- Access to all good stuff that Moq provides such as Moq ```Verify```, NSubstitute ```Received``` etc. 

## Resources
- [Source repository](https://github.com/rgvlee/LazyCache.Testing)
- [NuGet](https://www.nuget.org/packages/LazyCache.Testing.Moq/)

## The disclaimer
I have built these libraries for fun (well sort of, I needed the Moq implementation for work). The mocking doesn't extend to the CacheProvider so if you're hitting that explicitly then you're in for a world of nulls. I might add it in a later release but in general I think accessing the CacheProvider should be avoided. If you find these libraries useful and something is missing, not working as you'd expect or you need additional behaviour mocked flick me a message and I'll see what I can do.

## Fluent interface
Let's be honest, there isn't much set up to do except for some tricky bits to do with the no explicit set up case. That being said I've still provided a fluent interface for the cache entry set up for ease of use.

# Moq
## Basic usage
- Get a mocked lazy cache
- Consume

```
[Test]
public void MinimumViableInterface_Guid_ReturnsExpectedResult() {
    var cacheKey = "SomethingInTheCache";
    var expectedResult = Guid.NewGuid().ToString();

    var mockedCache = MockFactory.CreateMockedCachingService();
            
    var actualResult = mockedCache.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

    Assert.AreSame(expectedResult, actualResult);
}
```

## But I want the cache mock
No problem. Use the mock helper to create the mock. At this point it's a Mock\<IAppCache> for you to specify additional set ups, assert verify method invocations etc.

```
 [Test]
public void GetOrAddWithNoSetUp_TestObject_ReturnsExpectedResult() {
    var cacheKey = "SomethingInTheCache";
    var expectedResult = new TestObject();

    var cacheMock = MockFactory.CreateCachingServiceMock();
    var mockedCache = cacheMock.Object;

    var actualResult = mockedCache.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

    Assert.Multiple(() => {
        Assert.AreSame(expectedResult, actualResult);
        cacheMock.Verify(m => m.GetOrAdd(Arg.Any<string>(), Arg.Any<Func<ICacheEntry, TestObject>>()), Times.Once);
    });
}
```

## Let's get explicit
If you want to explicitly specify a cache entry set up, use the fluent extension method.

```
[Test]
public void GetOrAddWithSetUp_Guid_ReturnsExpectedResult() {
    var cacheKey = "SomethingInTheCache";
    var expectedResult = Guid.NewGuid().ToString();

    var cacheMock = MockFactory.CreateCachingServiceMock();
    cacheMock.SetUpCacheEntry(cacheKey, expectedResult);
    var mockedCache = cacheMock.Object;

    var actualResult = mockedCache.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

    Assert.AreSame(expectedResult, actualResult);
}
```

## I'm using Get\<T>, what do I need to do?
You'll need to use the explicit set up as described above - the mock needs to know what to return.

## Async? Please tell me you support the async methods.
The survey says, yes the async methods are supported. You're welcome.

# NSubstitute
I'm not going to go deep into NSubstitute usage as, well, it is the same except for one thing. For Moq, the MockFactory.CreateCachingServiceMock() method returns a Mock\<T> containing the mocked object (Mock\<T>.Object). For NSubstitute you don't need to do that.

## Basic usage
- Get a mocked lazy cache
- Consume
```
[Test]
public void MinimumViableInterface_Guid_ReturnsExpectedResult() {
    var cacheKey = "SomethingInTheCache";
    var expectedResult = Guid.NewGuid().ToString();

    var mockedCache = MockFactory.CreateCachingServiceMock();

    var actualResult = mockedCache.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

    Assert.AreSame(expectedResult, actualResult);
}
```

## Explicit cache entry set up
Same same just with the explicit ```SetUpCacheEntry``` to set up the specified cache entry.
```
[Test]
public void GetOrAddWithSetUp_Guid_ReturnsExpectedResult() {
    var cacheKey = "SomethingInTheCache";
    var expectedResult = Guid.NewGuid().ToString();

    var cacheMock = MockFactory.CreateCachingServiceMock();
    cacheMock.SetUpCacheEntry(cacheKey, expectedResult);
    
    var actualResult = cacheMock.GetOrAdd(cacheKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

    Assert.AreSame(expectedResult, actualResult);
}
```