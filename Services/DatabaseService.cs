using SQLite;
using CampusPulse.Models;
using CampusPulse.Helpers;

namespace CampusPulse.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        var dbPath = GetDatabasePath();
        _db = new SQLiteAsyncConnection(dbPath);

        Initialize().Wait();
    }

    private string GetDatabasePath()
    {
#if DEBUG
        // Developer mode – easy to inspect
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "campuspulse_dev.db");
#else
        // Production mode – secure app data storage
        return Path.Combine(FileSystem.AppDataDirectory, "campuspulse.db");
#endif
    }

    private async Task Initialize()
    {
        await _db.CreateTableAsync<User>();
        await _db.CreateTableAsync<Post>();
        await _db.CreateTableAsync<Comment>();
        await _db.CreateTableAsync<Reaction>();
        await _db.CreateTableAsync<Event>();
        await _db.CreateTableAsync<EventRegistration>();
        await _db.CreateTableAsync<Report>();
        await _db.CreateTableAsync<Category>();

        await SeedData();
    }

    private async Task SeedData()
    {
        // Only seed if empty
        if (await _db.Table<User>().CountAsync() > 0)
            return;

        // Seed categories
        var categories = new List<Category>
        {
            new() { Name = "Clubs" },
            new() { Name = "Sports" },
            new() { Name = "Academic" },
            new() { Name = "Social" },
            new() { Name = "Announcements" }
        };

        await _db.InsertAllAsync(categories);

        // Seed users
        var admin = new User
        {
            DisplayName = "Admin",
            Email = "admin@campuspulse.com",
            PasswordHash = PasswordHasher.Hash("Admin123!"),
            Role = "Admin",
            IsActive = true
        };

        var student1 = new User
        {
            DisplayName = "Kyran",
            Email = "kyran@student.com",
            PasswordHash = PasswordHasher.Hash("Password123!"),
            Role = "Student",
            IsActive = true
        };

        var student2 = new User
        {
            DisplayName = "Ava",
            Email = "ava@student.com",
            PasswordHash = PasswordHasher.Hash("Password123!"),
            Role = "Student",
            IsActive = true
        };

        var student3 = new User
        {
            DisplayName = "Liam",
            Email = "liam@student.com",
            PasswordHash = PasswordHasher.Hash("Password123!"),
            Role = "Student",
            IsActive = true
        };

        await _db.InsertAsync(admin);
        await _db.InsertAsync(student1);
        await _db.InsertAsync(student2);
        await _db.InsertAsync(student3);

        // Seed events
        var events = new List<Event>
        {
            new()
            {
                Title = "Coding Club Meetup",
                Description = "Weekly coding meetup for all skill levels.",
                Date = DateTime.Now.AddDays(3),
                Location = "Room B201",
                CategoryId = 1,
                CreatedBy = admin.UserId,
                Capacity = 30
            },
            new()
            {
                Title = "Basketball Tryouts",
                Description = "Open tryouts for the campus basketball team.",
                Date = DateTime.Now.AddDays(5),
                Location = "Gym Hall",
                CategoryId = 2,
                CreatedBy = admin.UserId,
                Capacity = 20
            },
            new()
            {
                Title = "Study Skills Workshop",
                Description = "Improve your study habits and exam performance.",
                Date = DateTime.Now.AddDays(7),
                Location = "Library Conference Room",
                CategoryId = 3,
                CreatedBy = admin.UserId,
                Capacity = 50
            }
        };

        await _db.InsertAllAsync(events);

        // Seed posts
        var posts = new List<Post>
        {
            new()
            {
                UserId = student1.UserId,
                Title = "Join the Coding Club!",
                Content = "We meet every Wednesday. All levels welcome!",
                CategoryId = 1
            },
            new()
            {
                UserId = student2.UserId,
                Title = "Basketball Tryouts Soon",
                Content = "Get ready for the big day!",
                CategoryId = 2
            },
            new()
            {
                UserId = student3.UserId,
                Title = "Study Workshop",
                Content = "Don't miss this helpful session.",
                CategoryId = 3
            }
        };

        await _db.InsertAllAsync(posts);

        // Seed comments
        var comments = new List<Comment>
        {
            new() { PostId = 1, UserId = student2.UserId, Content = "Sounds awesome!" },
            new() { PostId = 1, UserId = student3.UserId, Content = "I'll be there!" },
            new() { PostId = 2, UserId = student1.UserId, Content = "Good luck everyone!" }
        };

        await _db.InsertAllAsync(comments);

        // Seed reactions
        var reactions = new List<Reaction>
        {
            new() { PostId = 1, UserId = student1.UserId, ReactionType = "Like" },
            new() { PostId = 1, UserId = student2.UserId, ReactionType = "Helpful" },
            new() { PostId = 2, UserId = student3.UserId, ReactionType = "Interested" }
        };

        await _db.InsertAllAsync(reactions);
    }

    // Expose DB for services
    public SQLiteAsyncConnection Connection => _db;
}
