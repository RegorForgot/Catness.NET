﻿using Catness.Persistence;
using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Catness.Services.EFServices;

public class FollowService
{
    private readonly IDbContextFactory<CatnessDbContext> _dbContextFactory;

    public FollowService(
        IDbContextFactory<CatnessDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<Follow?> GetFollow(ulong followerId, ulong followedId)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Follows
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.FollowedId == followerId && user.FollowerId == followedId);
    }
    
    public async Task AddFollow(ulong followerId, ulong followedId)
    {
        await using CatnessDbContext context = await _dbContextFactory.CreateDbContextAsync();

        Follow follow = new Follow
        {
            FollowedId = followedId,
            FollowerId = followerId
        };

        await context.Follows.AddAsync(follow);
        await context.SaveChangesAsync();
    }
}