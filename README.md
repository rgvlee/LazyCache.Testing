# LazyCache.Testing

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/0a917d1ef2b04dfea0e8a99b941dd42b)](https://www.codacy.com/manual/rgvlee/LazyCache.Testing?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=rgvlee/LazyCache.Testing&amp;utm_campaign=Badge_Grade) [![Codacy Badge](https://api.codacy.com/project/badge/Coverage/0a917d1ef2b04dfea0e8a99b941dd42b)](https://www.codacy.com/manual/rgvlee/LazyCache.Testing?utm_source=github.com&utm_medium=referral&utm_content=rgvlee/LazyCache.Testing&utm_campaign=Badge_Coverage)

## Overview

LazyCache.Testing is a mocking library that creates LazyCache IAppCache system mocks. It's easy to use (usually just a single line of code) with implementations for both Moq and NSubstitute.

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

This creates a mocked `IAppCache`. If your SUT populates the cache you're done. If it doesn't, or you like your arrange to be verbose, populate it as if you were using the real thing:

```c#
var cacheEntryKey = "SomethingInTheCache";
var expectedResult = Guid.NewGuid().ToString();

var mockedCache = Create.MockedCachingService();
mockedCache.Add(cacheEntryKey, expectedResult);

var actualResult = mockedCache.Get<string>(cacheEntryKey);

Assert.AreEqual(expectedResult, actualResult);
```

The Moq implementation of `Create.MockedCachingService()` returns the mocked caching service. If you need the mock itself (e.g., to verify an invocation) use `Mock.Get(mockedCache)`:

```c#
var cacheEntryKey = "SomethingInTheCache";
var expectedResult = Guid.NewGuid().ToString();

var mockedCache = Create.MockedCachingService();

var actualResult = mockedCache.GetOrAdd(cacheEntryKey, () => expectedResult, DateTimeOffset.Now.AddMinutes(30));

var cacheMock = Mock.Get(mockedCache);
cacheMock.Verify(x => x.GetOrAdd(cacheEntryKey, It.IsAny<Func<ICacheEntry, string>>()), Times.Once);
```

With regard to verifying invocations, all members of the `IAppCache` interface except `CacheProvider` are mocked.

## Limitations

-   The CacheProvider is not mocked (in general I think accessing the CacheProvider itself should be avoided; raise an issue if you think it should be supported)
