using System.Security.Cryptography;
using System.Text;

namespace CampusPulse.Helpers;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[16];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        byte[] combined = new byte[48];
        Array.Copy(salt, 0, combined, 0, 16);
        Array.Copy(hash, 0, combined, 16, 32);

        return Convert.ToBase64String(combined);
    }

    public static bool Verify(string password, string storedHash)
    {
        byte[] combined = Convert.FromBase64String(storedHash);

        byte[] salt = new byte[16];
        Array.Copy(combined, 0, salt, 0, 16);

        byte[] stored = new byte[32];
        Array.Copy(combined, 16, stored, 0, 32);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] computed = pbkdf2.GetBytes(32);

        return stored.SequenceEqual(computed);
    }
}
