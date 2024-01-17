using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Services.EntityFramework;

public class ChannelService : CachedEFService
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public ChannelService(
        IDbContextFactory<CatnessDbContext> dbContextFactory,
        IMemoryCache memoryCache) : base(memoryCache)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public bool TryGetChannelFromCache(ulong channelId, out GuildChannel? guildChannel)
    {
        return _memoryCache.TryGetValue(GetChannelCacheKey(channelId), out guildChannel);
    }
    
    public void AddChannelToCache(GuildChannel guildChannel)
    {
        _memoryCache.Set(GetChannelCacheKey(guildChannel.ChannelId), guildChannel, TimeSpan.FromMinutes(5));
    }

    public async Task<GuildChannel> GetOrAddChannel(ulong channelId, ulong guildId)
    {
        GuildChannel? channel = await GetChannel(channelId);

        if (channel is null)
        {
            channel = new GuildChannel
            {
                ChannelId = channelId,
                GuildId = guildId
            };

            await AddChannel(channel);
        }

        return channel;
    }

    public async Task<GuildChannel?> GetChannel(ulong channelId)
    {
        if (TryGetChannelFromCache(channelId, out GuildChannel? guildChannel))
        {
            return guildChannel;
        }
        
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        GuildChannel? dbGuildChannel = await context.GuildChannels
            .AsNoTracking()
            .FirstOrDefaultAsync(channel => channel.ChannelId == channelId);
        
        if (dbGuildChannel is not null)
        {
            AddChannelToCache(dbGuildChannel);
        }

        return dbGuildChannel;
    }

    public async Task AddChannel(GuildChannel channel)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        await context.GuildChannels.AddAsync(channel);
        await context.SaveChangesAsync();
        AddChannelToCache(channel);
    }

    public async Task UpdateChannel(GuildChannel channel)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        context.GuildChannels.Update(channel);
        await context.SaveChangesAsync();
        AddChannelToCache(channel);
    }
    
    private static string GetChannelCacheKey(ulong channelId)
    {
        return $"channel-{channelId}";
    }
}