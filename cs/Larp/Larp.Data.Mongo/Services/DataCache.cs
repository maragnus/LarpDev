﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Larp.Data.Mongo.Services;

public class LarpDataCache : MemoryCache
{
    public LarpDataCache(IOptions<MemoryCacheOptions> optionsAccessor) : base(optionsAccessor)
    {
    }

    public LarpDataCache(IOptions<MemoryCacheOptions> optionsAccessor, ILoggerFactory loggerFactory) : base(optionsAccessor, loggerFactory)
    {
    }
}