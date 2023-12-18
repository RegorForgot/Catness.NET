using Catness.Persistence;
using Catness.Persistence.Models;
using Catness.Services.EntityFramework;
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
    private readonly ChannelService _channelService;

    public UserHandler(
        IMemoryCache cache,
        IDbContextFactory<CatnessDbContext> dbContextFactory,
        GuildService guildService,
        UserService userService,
        ChannelService channelService)
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
        _guildService = guildService;
        _userService = userService;
        _channelService = channelService;
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

        string key = GetUserCooldownCacheKey(message.Author.Id, guildChannel.GuildId);

        bool cacheValue = _cache.TryGetValue(key, out _);
        if (cacheValue)
        {
            return;
        }

        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        Guild guild = await _guildService.GetOrAddGuild(guildChannel.GuildId);
        User user = await _userService.GetOrAddUser(message.Author.Id);
        GuildChannel? channel = await _channelService.GetChannel(guildChannel.Id);

        if (!user.LevellingEnabled || !guild.LevellingEnabled || (channel is not null && !channel.LevellingEnabled))
        {
            return;
        }

        GuildUser guildUser = await _guildService.GetOrAddUserToGuild(user, guild);

        if (guildUser.IsLevelBlacklisted)
        {
            return;
        }

        guildUser.Experience++;
        await _guildService.UpdateGuildUser(guildUser);
        _cache.Set(key, new object(), TimeSpan.FromSeconds(5));
    }

    private static string GetUserCooldownCacheKey(ulong userId, ulong guildId)
    {
        return $"message-{guildId}-{userId}";
    }
}