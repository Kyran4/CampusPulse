namespace CampusPulse.Helpers;

public static class ImageHelper
{
    public static ImageSource? FromBase64(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
            return null;

        byte[] bytes = Convert.FromBase64String(base64);
        return ImageSource.FromStream(() => new MemoryStream(bytes));
    }
}
