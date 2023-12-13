using System.Text;
using Catness.Persistence;
using Catness.Persistence.Models;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Catness.Modules.Fun;

public class BirthdayModule : InteractionModuleBase
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;
    
    public BirthdayModule(
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
}