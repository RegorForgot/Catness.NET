using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Services.EntityFramework;

public class GuildService : CachedEFService
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public GuildService(
        IDbContextFactory<CatnessDbContext> dbContextFactory,
        IMemoryCache memoryCache) : base(memoryCache)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public bool TryGetGuildFromCache(ulong guildId, out Guild? guild)
    {
        return _memoryCache.TryGetValue(GetGuildCacheKey(guildId), out guild);
    }
    
    public void AddGuildToCache(Guild guild)
    {
        _memoryCache.Set(GetGuildCacheKey(guild.GuildId), guild, TimeSpan.FromMinutes(5));
    }

    public async Task<Guild> GetOrAddGuild(ulong guildId)
    {
        Guild? guild = await GetGuild(guildId);

        if (guild is null)
        {
            guild = new Guild
            {
                GuildId = guildId
            };

            await AddGuild(guild);
        }

        return guild;
    }

    public async Task AddGuild(Guild guild)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        await context.Guilds.AddAsync(guild);
        await context.SaveChangesAsync();
        AddGuildToCache(guild);
    }

    public async Task<Guild?> GetGuild(ulong guildId)
    {
        if (TryGetGuildFromCache(guildId, out Guild? guild))
        {
            return guild;
        }
        
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        Guild? dbGuild = await context.Guilds
            .AsNoTracking()
            .FirstOrDefaultAsync(guild => guild.GuildId == guildId);
        
        if (dbGuild is not null)
        {
            AddGuildToCache(dbGuild);
        }

        return dbGuild;
    }

    public async Task<GuildUser> GetOrAddUserToGuild(User user, Guild guild)
    {
        GuildUser? guildUser = await GetUserInGuild(user.UserId, guild.GuildId);

        if (guildUser is null)
        {
            guildUser = new GuildUser
            {
                GuildId = guild.GuildId,
                UserId = user.UserId
            };
            await AddUserToGuild(guildUser);
        }

        return guildUser;
    }

    public async Task<GuildUser?> GetUserInGuild(ulong userId, ulong guildId)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        return await context.GuildUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(gu => gu.UserId == userId && gu.GuildId == guildId);
    }

    public async Task AddUserToGuild(GuildUser guildUser)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        await context.GuildUsers.AddAsync(guildUser);
        await context.SaveChangesAsync();
    }

    public async Task UpdateGuildUser(GuildUser guildUser)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        context.GuildUsers.Update(guildUser);
        await context.SaveChangesAsync();
    }

    public async Task<List<GuildUser>> GetUsersSortedLevel(ulong guildId)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        return await context.GuildUsers
            .Where(g => g.GuildId == guildId)
            .Where(g => !g.IsLevelBlacklisted)
            .OrderByDescending(g => g.Level)
            .ThenByDescending(g => g.Experience)
            .ToListAsync();
    }
    
    private static string GetGuildCacheKey(ulong guildId)
    {
        return $"guild-{guildId}";
    }
}