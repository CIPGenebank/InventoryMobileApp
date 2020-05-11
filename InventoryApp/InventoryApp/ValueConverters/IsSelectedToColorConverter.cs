using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace InventoryApp.ValueConverters
{
    public class IsSelectedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSelected = (bool)value;

            if (isSelected)
                return Color.FromHex("#EEEEEE");
            else
                return Color.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Color)value != Color.Transparent ? true : false ;
        }
    }
}
