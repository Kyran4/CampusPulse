namespace CampusPulse.Helpers;

public static class ValidationHelper
{
    public static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email)
            && email.Contains("@")
            && email.Contains(".");
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        if (password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsLower)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(ch => "!@#$%^&*()_+-=[]{}|;:'\",.<>/?".Contains(ch))) return false;

        return true;
    }

    public static bool IsRequired(string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsValidEventDate(DateTime date)
    {
        return date > DateTime.Now;
    }

    public static bool IsValidCapacity(int capacity)
    {
        return capacity > 0;
    }
}
