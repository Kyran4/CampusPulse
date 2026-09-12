namespace CampusPulse.Helpers;

// Shared by CreatePostViewModel, CreateEventViewModel, and ProfileViewModel -
// one place for "let the user pick a photo, turn it into the base64 string
// the API expects." Optional everywhere it's used: returning null just
// means the user cancelled, and callers treat that as "no change."
public static class ImagePickerHelper
{
    public static async Task<string?> PickImageAsBase64Async()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo == null)
                return null;

            using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            return Convert.ToBase64String(memoryStream.ToArray());
        }
        catch (FeatureNotSupportedException)
        {
            return null;
        }
        catch (PermissionException)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }
}
