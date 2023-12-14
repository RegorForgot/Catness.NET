using Catness.Enums;
using Catness.Persistence;
using Catness.Persistence.Models;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Catness.Modules.Fun;

[Group("user", "User commands")]
public class UserCommandsModule : InteractionModuleBase
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public UserCommandsModule(
        IDbContextFactory<CatnessDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    [SlashCommand("birthday-set", "Set your birthday")]
    public async Task SetBirthday(
        [ComplexParameter] BirthdayType birthday)
    {
        DateOnly date;

        try
        {
            date = birthday.GetDate();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            await RespondAsync($"Error! {ex.Message}");
            return;
        }

        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        User? user = dbContext.Users.FirstOrDefault(user => user.UserId == Context.User.Id);

        if (user is null)
        {
            await dbContext.Users.AddAsync(new User
            {
                Birthday = date,
                UserId = Context.User.Id
            });

            await dbContext.SaveChangesAsync();
            await RespondAsync($"Added birthday as {date}");
        }
        else if (user.Birthday == date)
        {
            await RespondAsync($"Birthday provided is equal, not removing birthday");
        }
        else
        {
            string responseString = $"Old birthday was {user.Birthday}; New birthday is {date}";

            user.Birthday = date;
            await dbContext.SaveChangesAsync();

            await RespondAsync(responseString);
        }
    }

    [SlashCommand("avatar", "Get a user's avatar")]
    public async Task GetAvatar(
        [Summary(description: "User to get the avatar for")]
        IUser? user = null,
        [Summary(description: "Server avatar")]
        bool server = false,
        [Summary(description: "Whether the embed response should be private ")]
        bool ephemeral = false)
    {
        if (user is null)
        {
            user = Context.User;
        }

        string imageUrl;

        if (user is IGuildUser guildUser && server)
        {
            imageUrl = guildUser.GetDisplayAvatarUrl(ImageFormat.Auto, 2048);
        }
        else
        {
            imageUrl = user.GetAvatarUrl(ImageFormat.Auto, 2048);
        }

        Embed embed = new EmbedBuilder
        {
            Title = $"{user.Username}'s avatar",
            ImageUrl = imageUrl
        }.Build();

        await RespondAsync(embed: embed, ephemeral: ephemeral);
    }
    
    [SlashCommand("follow", "Follow another user")]
    public async Task FollowUser(
        [Summary(description: "User to follow")]
        IUser user)
    {
        if (user == Context.User)
        {
            await RespondAsync("You cannot follow yourself!", ephemeral: true);
        }
        else if (user is ISelfUser)
        {
            await RespondAsync("You cannot follow me", ephemeral: true);
        }

        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        User? followed = dbContext.Users.FirstOrDefault(userDb => userDb.UserId == user.Id);

        if (followed is null)
        {
            await dbContext.Users.AddAsync(new User
            {
                UserId = user.Id
            });
            await dbContext.SaveChangesAsync();
        }

        Follow? follow = dbContext.Follows.FirstOrDefault(follow =>
            follow.FollowerId == Context.User.Id && follow.FollowedId == user.Id);

        if (follow is null)
        {
            await dbContext.Follows.AddAsync(new Follow
            {
                FollowedId = user.Id,
                FollowerId = Context.User.Id
            });
            await RespondAsync($"Started following <@{user.Id}>!", allowedMentions: new AllowedMentions(AllowedMentionTypes.Users));
            await dbContext.SaveChangesAsync();
        }
        else
        {
            await RespondAsync($"You are already following <@{user.Id}>.", allowedMentions: AllowedMentions.None);
        }
    }
}