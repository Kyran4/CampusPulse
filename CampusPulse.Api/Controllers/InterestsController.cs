using CampusPulse.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InterestsController : ControllerBase
{
    private static readonly List<Interest> Interests = new()
    {
        new Interest { CategoryId = 1, Name = "Sports" },
        new Interest { CategoryId = 2, Name = "Technology" },
        new Interest { CategoryId = 3, Name = "Gaming" },
        new Interest { CategoryId = 4, Name = "Music" },
        new Interest { CategoryId = 5, Name = "Cultural Events" },
        new Interest { CategoryId = 6, Name = "Careers" },
        new Interest { CategoryId = 7, Name = "Study Support" },
        new Interest { CategoryId = 8, Name = "Volunteering" }
    };

    [HttpGet]
    public ActionResult<List<Interest>> GetInterests()
    {
        return Ok(Interests);
    }

    private static readonly Dictionary<int, List<int>> FollowedInterests = new();

    [HttpPost("{interestId}/follow/{userId}")]
    public IActionResult FollowInterest(int interestId, int userId)
    {
        var interest = Interests
            .FirstOrDefault(i => i.CategoryId == interestId);

        if (interest == null)
        {
            return NotFound("Interest not found.");
        }

        if (!FollowedInterests.ContainsKey(userId))
        {
            FollowedInterests[userId] = new List<int>();
        }

        if (!FollowedInterests[userId].Contains(interestId))
        {
            FollowedInterests[userId].Add(interestId);
        }

        return Ok(new
        {
            message = $"Now following {interest.Name}",
            userId,
            interestId
        });
    }

    [HttpDelete("{interestId}/follow/{userId}")]
    public IActionResult UnfollowInterest(int interestId, int userId)
    {
        var interest = Interests
            .FirstOrDefault(i => i.CategoryId == interestId);

        if (interest == null)
        {
            return NotFound("Interest not found.");
        }

        if (!FollowedInterests.ContainsKey(userId))
        {
            return NotFound("No followed interests.");
        }

        if (!FollowedInterests[userId].Contains(interestId))
        {
            return NotFound("Interest not followed.");
        }

        FollowedInterests[userId].Remove(interestId);

        return Ok(new
        {
            message = $"Unfollowed {interest.Name}",
            userId,
            interestId
        });
    }

    [HttpGet("followed/{userId}")]
    public ActionResult<List<Interest>> GetFollowedInterests(int userId)
    {
        if (!FollowedInterests.ContainsKey(userId))
        {
            return Ok(new List<Interest>());
        }

        var followedIds = FollowedInterests[userId];

        var followed = Interests
            .Where(i => followedIds.Contains(i.CategoryId))
            .ToList();

        return Ok(followed);
    }
}