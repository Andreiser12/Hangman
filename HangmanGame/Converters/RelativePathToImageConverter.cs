using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace HangmanGame.Converters
{
    public class RelativePathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string relativePath = value as string;

            if (string.IsNullOrEmpty(relativePath))
            {
                return null;
            }

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            if (!File.Exists(fullPath))
            {
                return null;
            }

            return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
