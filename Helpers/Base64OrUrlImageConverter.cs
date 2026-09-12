using System.Globalization;

namespace CampusPulse.Helpers;

// The "ImageBase64" field on Post/Event/User holds either a plain URL
// (that's what the seeded demo content uses - e.g. picsum.photos links) or
// actual base64 image data (that's what a real upload produces, see
// ImagePickerHelper). Binding an Image.Source directly to the raw string
// only works for the URL case - MAUI's implicit string->ImageSource
// conversion doesn't know what to do with raw base64 data, so it silently
// shows nothing. This converter inspects the value and builds the right
// ImageSource either way. It's also the reason seeded images never
// rendered even though the URLs were valid - there was no Image control
// bound to this field anywhere before now.
public class Base64OrUrlImageConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string s || string.IsNullOrWhiteSpace(s))
            return null;

        if (s.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            s.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return ImageSource.FromUri(new Uri(s));
        }

        try
        {
            var bytes = System.Convert.FromBase64String(s);
            return ImageSource.FromStream(() => new MemoryStream(bytes));
        }
        catch
        {
            // Not valid base64 and not a URL - nothing sensible to show.
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
