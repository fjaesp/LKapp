using System;
using Xamarin.Forms;

namespace LK
{
    public class ChatAuthorConverter : IValueConverter
    {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
            try
            {
                if (value is String)
                {
                    String author = (string)value;
                    if (author == App.CurrentUser.displayName)
                        return "Meg";
                    else
                        return (string)value;
                }
            }
            catch { }


            return (string)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
    }
}
