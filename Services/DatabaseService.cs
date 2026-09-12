using System.Text.Json;

namespace CampusPulse.Services;

public class DatabaseService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private string GetPath(string filename)
    {
        return Path.Combine(FileSystem.AppDataDirectory, filename);
    }

    // ============================
    // JWT Token Storage
    // ============================
    // Preferences instead of SecureStorage: SecureStorage on Windows is
    // backed by PasswordVault, which requires the app to have a packaged
    // identity (MSIX). Most dev/debug runs are unpackaged, and in that case
    // SecureStorage silently fails to persist - GetTokenAsync comes back
    // null even right after a successful SetAsync. That means ApiAuthHandler
    // never attaches a token, so every [Authorize] endpoint 401s while
    // [AllowAnonymous] ones keep working - exactly the split between what
    // was/wasn't working. Preferences isn't encrypted, but for local-device
    // storage of a short-lived JWT in a student project this is a
    // reasonable trade for something that actually works on every platform
    // without extra packaging setup.
    public async Task SaveTokenAsync(string token)
    {
        Preferences.Default.Set("jwt_token", token);
        await Task.CompletedTask;
    }

    public async Task<string?> GetTokenAsync()
    {
        var token = Preferences.Default.Get("jwt_token", string.Empty);
        await Task.CompletedTask;
        return string.IsNullOrEmpty(token) ? null : token;
    }

    public void ClearToken()
    {
        Preferences.Default.Remove("jwt_token");
    }

    // ============================
    // User Storage
    // ============================
    public async Task SaveUserAsync(object user)
    {
        var json = JsonSerializer.Serialize(user, _jsonOptions);
        File.WriteAllText(GetPath("user.json"), json);
    }

    public T? LoadUser<T>()
    {
        var path = GetPath("user.json");
        if (!File.Exists(path)) return default;

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    public void ClearUser()
    {
        var path = GetPath("user.json");
        if (File.Exists(path)) File.Delete(path);
    }

    // ============================
    // Generic JSON Cache
    // ============================
    public void SaveCache<T>(string key, T data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        File.WriteAllText(GetPath($"{key}.json"), json);
    }

    public T? LoadCache<T>(string key)
    {
        var path = GetPath($"{key}.json");
        if (!File.Exists(path)) return default;

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    public void ClearCache(string key)
    {
        var path = GetPath($"{key}.json");
        if (File.Exists(path)) File.Delete(path);
    }
}
