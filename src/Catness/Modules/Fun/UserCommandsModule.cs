using System.Text;
using Catness.Persistence;
using Catness.Persistence.Models;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Catness.Modules.Fun;

public class UserCommandsModule : InteractionModuleBase
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public UserCommandsModule(
        IDbContextFactory<CatnessDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    [SlashCommand("birthday-set", "set your birthday")]
    public async Task SetBirthday(DateTime time)
    {
        await using CatnessDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();

        User? user = dbContext.Users.FirstOrDefault(user => user.Id == Context.User.Id);
        if (user is null)
        {
            await dbContext.Users.AddAsync(new User
            {
                Birthday = new DateTimeOffset(time, TimeSpan.Zero),
                Id = Context.User.Id
            });
            await dbContext.SaveChangesAsync();
            await RespondAsync("Added birthday as " + dbContext.Users.FirstOrDefault(user => user.Id == Context.User.Id)!.Birthday);
        }
        else
        {
            StringBuilder sb = new StringBuilder($"Old birthday was {user.Birthday}; ");
            user.Birthday = new DateTimeOffset(time, TimeSpan.Zero);
            await dbContext.SaveChangesAsync();

            sb.Append($"New birthday is {dbContext.Users.FirstOrDefault(user => user.Id == Context.User.Id)!.Birthday}");
            await RespondAsync(sb.ToString());
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
}