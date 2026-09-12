using CampusPulse.Models;

namespace CampusPulse.Services;

public class AuthenticationService
{
    public async Task<User?> LoginAsync(string email, string password)
    {
        await Task.Delay(300);

        if (email == "student@campuspulse.nz" &&
            password == "Password123")
        {
            return new User
            {
                UserId = 1,
                DisplayName = "Demo Student",
                Email = "student@campuspulse.nz",
                Role = "Student",
                IsActive = true
            };
        }

        return null;
    }
}