using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Catness.Services.EntityFramework;

public class GuildService
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public GuildService(
        IDbContextFactory<CatnessDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
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
    }

    public async Task<Guild?> GetGuild(ulong guildId)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Guilds
            .AsNoTracking()
            .FirstOrDefaultAsync(guild => guild.GuildId == guildId);
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
}