using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace HangmanGame.Converters
{
    public class CategoryToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string category = value as string;
            if (string.IsNullOrEmpty(category))
                return null;

            string fileName = category.ToLower().Replace(" ", "_") + ".jpg";
            string uri = $"pack://application:,,,/Resources/Backgrounds/{fileName}";

            try
            {
                return new BitmapImage(new Uri(uri, UriKind.Absolute));
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}