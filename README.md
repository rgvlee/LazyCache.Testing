# LazyCache.Testing

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/0a917d1ef2b04dfea0e8a99b941dd42b)](https://www.codacy.com/manual/rgvlee/LazyCache.Testing?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=rgvlee/LazyCache.Testing&amp;utm_campaign=Badge_Grade) [![Codacy Badge](https://api.codacy.com/project/badge/Coverage/0a917d1ef2b04dfea0e8a99b941dd42b)](https://www.codacy.com/manual/rgvlee/LazyCache.Testing?utm_source=github.com&utm_medium=referral&utm_content=rgvlee/LazyCache.Testing&utm_campaign=Badge_Coverage)

__*A functional system mock of the LazyCache caching service using Moq and NSubstitute*__

## Overview

LazyCache ships with a MockCachingService which begs the question - why should you use LazyCache.Testing?

The MockCachingService is limited. Add\<T\> is an empty implementation. Get\<T\> will return default(T). It doesn't work for all caching service usages. I needed a better implementation and I am sure others do as well.

The intent of this library is to provide a mock that __will__ work for all caching service usages. Some of the features include:

-   Easy to use - create a functional system mock with a single line of code
-   Implicit or explicit cache entry set up, it's up to you
-   Access to all of the good stuff that these great mocking libraries provide (e.g., Moq Verify)

## Resources

-   [Source repository](https://github.com/rgvlee/LazyCache.Testing/)
-   [LazyCache.Testing.Moq - NuGet](https://www.nuget.org/packages/LazyCache.Testing.Moq/)
-   [LazyCache.Testing.NSubstitute - NuGet](https://www.nuget.org/packages/LazyCache.Testing.NSubstitute/)

## Usage

Start by creating a mocked caching service using `Create.MockedCachingService()`:

```c#
var cacheEntryKey = "SomethingInTheCache";
var expectedResult = Guid.NewGuid().ToString();

var mockedCache = Create.MockedCachingService();

var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

Assert.AreEqual(expectedResult, actualResult);
```

Provided your SUT populates the cache that'd be all you need to do. If it doesn't, or you like your arrange to be verbose, use `SetUpCacheEntry<T>` to set up a cache entry:

```c#
var cacheEntryKey = "SomethingInTheCache";
var expectedResult = Guid.NewGuid().ToString();

var mockedCache = Create.MockedCachingService();
mockedCache.SetUpCacheEntry(cacheEntryKey, expectedResult);

var actualResult = mockedCache.Get<string>(cacheEntryKey);

Assert.AreEqual(expectedResult, actualResult);
```

The Moq implementation of `Create.MockedCachingService()` returns the mocked caching service. If you need the mock (e.g., to verify an invocation) use `Mock.Get(mockedCache)`:

```c#
var cacheEntryKey = "SomethingInTheCache";
var expectedResult = Guid.NewGuid().ToString();

var mockedCache = Create.MockedCachingService();

var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

var cacheMock = Mock.Get(mockedCache);
cacheMock.Verify(x => x.GetOrAdd(cacheEntryKey, It.IsAny<Func<ICacheEntry, string>>()), Times.Once);
```

## Limitations

-   The CacheProvider is not mocked (in general I think accessing the CacheProvider itself should be avoided; raise an issue if you think it should be supported)