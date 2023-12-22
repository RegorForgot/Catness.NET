using Catness.Extensions;
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
    private readonly DiscordSocketClient _client;
    private readonly IMemoryCache _cache;
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;
    private readonly GuildService _guildService;
    private readonly UserService _userService;
    private readonly ChannelService _channelService;
    private readonly Random _random;

    public UserHandler(
        DiscordSocketClient client,
        IMemoryCache cache,
        IDbContextFactory<CatnessDbContext> dbContextFactory,
        GuildService guildService,
        UserService userService,
        ChannelService channelService)
    {
        _client = client;
        _cache = cache;
        _dbContextFactory = dbContextFactory;
        _guildService = guildService;
        _userService = userService;
        _channelService = channelService;
        _random = new Random();
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

        ulong nextExp = guildUser.Experience + (ulong)_random.Next(5, 20);

        ulong level = guildUser.Level;
        // quadratic for levelling curve
        ulong expToNextLevel = (5 * level * level) + (50 * level) + 100;

        if (nextExp >= expToNextLevel)
        {
            guildUser.Level++;
            guildUser.Experience = nextExp - expToNextLevel;
            Embed embed = new EmbedBuilder()
                .WithTitle("Level up!")
                .WithDescription($"Congratulations! You have levelled up {level + 1}")
                .Build();

            await message.Channel.SendMessageAsync(
                embed: embed,
                messageReference: new MessageReference(message.Id),
                text: message.Author.Id.GetPingString(),
                allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
        }
        else
        {
            guildUser.Experience = nextExp;
        }

        await _guildService.UpdateGuildUser(guildUser);
        _cache.Set(key, new object(), TimeSpan.FromSeconds(5));
    }

    private static string GetUserCooldownCacheKey(ulong userId, ulong guildId)
    {
        return $"message-{guildId}-{userId}";
    }
}