using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Catness.Services.EntityFramework;

public class BotPersistenceService
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public BotPersistenceService(
        IDbContextFactory<CatnessDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<BotPersistence> GetOrAddKeyValuePair(string key)
    {
        BotPersistence? persistence = await GetValueFromKey(key);

        if (persistence is null)
        {
            persistence = new BotPersistence
            {
                Key = key
            };

            await AddKeyValuePair(persistence);
        }

        return persistence;
    }

    public async Task AddKeyValuePair(BotPersistence persistence)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        await context.BotPersistence.AddAsync(persistence);
        await context.SaveChangesAsync();
    }

    public async Task<BotPersistence?> GetValueFromKey(string key)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        return await context.BotPersistence
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Key == key);
    }

    public async Task UpdateKeyValuePair(BotPersistence persistence)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        context.BotPersistence.Update(persistence);
        await context.SaveChangesAsync();
    }
}