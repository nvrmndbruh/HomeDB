using System.Globalization;

namespace HomeDB.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter != null && parameter.ToString() == "reverse")
                return value == null ? false : true;
            return value == null ? true : false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
