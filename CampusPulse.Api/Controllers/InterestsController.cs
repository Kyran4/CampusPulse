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
}