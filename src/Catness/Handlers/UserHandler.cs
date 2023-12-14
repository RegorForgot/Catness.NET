using Catness.Persistence;
using Catness.Persistence.Models;
using Catness.Services.EFServices;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Handlers;

public class UserHandler
{
    private readonly IMemoryCache _cache;
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;
    private readonly GuildService _guildService;
    private readonly UserService _userService;

    public UserHandler(
        IMemoryCache cache,
        IDbContextFactory<CatnessDbContext> dbContextFactory,
        GuildService guildService,
        UserService userService)
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
        _guildService = guildService;
        _userService = userService;
    }

    public async Task HandleLevellingAsync(SocketMessage message)
    {
        if (message.Author.IsBot ||
            message.Type == MessageType.GuildMemberJoin ||
            message.Type == MessageType.UserPremiumGuildSubscription ||
            message.Channel is not IGuildChannel guildChannel)
        {
            return;
        }

        string key = GetUserCooldownCacheKey(message.Author.Id);

        bool cacheValue = _cache.TryGetValue(key, out object? cooldown);
        if (cacheValue)
        {
            return;
        }

        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        Guild? guild = await _guildService.GetOrAddGuild(guildChannel.GuildId);
        User? user = await _userService.GetUser(message.Author.Id);

        if (user is null || guild is null)
        {
            return;
        }
        
        if (!user.LevellingEnabled || !guild.LevellingEnabled)
        {
            return;
        }

        GuildUser guildUser = await _guildService.GetOrAddUserToGuild(user, guild);

        if (guildUser.IsLevelBlacklisted)
        {
            return;
        }

        user.Experience++;
        await _userService.UpdateUser(user);
    }

    public static string GetUserCooldownCacheKey(ulong userId)
    {
        return $"message-{userId}";
    }
}