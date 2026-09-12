namespace CampusPulse.Models;

public class FollowCategoryDto
{
    public int CategoryId { get; set; }
    // No UserId - the API derives the follower from the caller's JWT.
}
