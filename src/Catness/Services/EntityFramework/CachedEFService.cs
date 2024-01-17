using Microsoft.Extensions.Caching.Memory;

namespace Catness.Services.EntityFramework;

public abstract class CachedEFService 
{
    protected readonly IMemoryCache _memoryCache;
    
    public CachedEFService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
}