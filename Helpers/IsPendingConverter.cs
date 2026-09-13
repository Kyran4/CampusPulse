using System.Globalization;

namespace CampusPulse.Helpers;

// Bound to Report.Status: true when the status is "Pending". Pass
// ConverterParameter="invert" to get the opposite (true for anything that
// ISN'T Pending) - used to show "Mark Reviewed" only on pending reports and
// "Undo" only on already-reviewed ones, from the same Status value.
public class IsPendingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isPending = value is string status && status == "Pending";
        var invert = parameter is string p && p.Equals("invert", StringComparison.OrdinalIgnoreCase);
        return invert ? !isPending : isPending;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
