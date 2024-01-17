using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Catness.Services.EntityFramework;

public class UserService : CachedEFService
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public UserService(IDbContextFactory<CatnessDbContext> dbContextFactory, IMemoryCache memoryCache) : base(memoryCache)
    {
        _dbContextFactory = dbContextFactory;
    }

    public bool TryGetUserFromCache(ulong userId, out User? user)
    {
        return _memoryCache.TryGetValue(GetUserCacheKey(userId), out user);
    }
    
    public void AddUserToCache(User user)
    {
        _memoryCache.Set(GetUserCacheKey(user.UserId), user, TimeSpan.FromMinutes(5));
    }

    public async Task<User> GetOrAddUser(ulong userId)
    {
        User? user = await GetUser(userId);

        if (user is null)
        {
            user = new User
            {
                UserId = userId
            };
            await AddUser(user);
        }

        return user;
    }

    public async Task<User?> GetUser(ulong userId)
    {
        if (TryGetUserFromCache(userId, out User? user))
        {
            return user;
        }
        
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        User? dbUser = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserId == userId);

        if (dbUser is not null)
        {
            AddUserToCache(dbUser);
        }

        return dbUser;
    }
    

    public async Task<User?> GetUserWithFollows(ulong userId)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Users
            .Include(user => user.Followers)
            .Include(user => user.Following)
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserId == userId);
    }

    public async Task<List<User>> GetUsersWithFollowersWithBirthday()
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        DateTime.UtcNow.Deconstruct(out DateOnly dateOnly, out _);

        return await context.Users
            .Where(user =>
                user.Birthday != null &&
                user.Birthday.Value.Month == dateOnly.Month &&
                user.Birthday.Value.Day == dateOnly.Day)
            .Include(user => user.Followers)
            .ToListAsync();
    }

    public async Task AddUser(User user)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        AddUserToCache(user);
    }

    public async Task UpdateUser(User user)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        context.Users.Update(user);
        await context.SaveChangesAsync();
        AddUserToCache(user);
    }

    public async Task UpdateUser(params User[] users)
    {
        foreach (User user in users)
        {
            await UpdateUser(user);
        }
    }
    
    private static string GetUserCacheKey(ulong userId)
    {
        return $"user-{userId}";
    }
}