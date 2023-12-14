namespace Catness.Persistence.Models;

public class Follow
{
    public ulong FollowerId { get; set; }
    public User Follower { get; set; }
    public ulong FollowedId { get; set; }
    public User Followed { get; set;  }
}