using System.Globalization;

namespace CampusPulse.Helpers;

// True when the string HAS a value (not null/empty). Used to hide an Image
// element when there's no image, rather than showing an empty box.
public class StringHasValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => !string.IsNullOrWhiteSpace(value as string);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
