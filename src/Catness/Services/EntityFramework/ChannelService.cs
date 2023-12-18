using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Catness.Services.EntityFramework;

public class ChannelService
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public ChannelService(
        IDbContextFactory<CatnessDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
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
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        return await context.GuildChannels
            .AsNoTracking()
            .FirstOrDefaultAsync(channel => channel.ChannelId == channelId);
    }

    public async Task AddChannel(GuildChannel channel)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        await context.GuildChannels.AddAsync(channel);
        await context.SaveChangesAsync();
    }

    public async Task UpdateChannel(GuildChannel channel)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        context.GuildChannels.Update(channel);
        await context.SaveChangesAsync();
    }
}