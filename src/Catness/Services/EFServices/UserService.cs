﻿using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Catness.Services.EFServices;

public class UserService
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public UserService(IDbContextFactory<CatnessDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
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
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserId == userId);
    }

    public async Task<User?> GetUserWithGuilds(ulong userId)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Users
            .Include(user => user.Guilds)
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserId == userId);
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

    public async Task AddUser(User user)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateUser(User user)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        context.Users.Update(user);
        await context.SaveChangesAsync();
    }
}