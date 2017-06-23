using System;
using System.Globalization;
using Xamarin.Forms;

namespace LK
{
    public class ChatProfilePictureConverter: IValueConverter
    {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				if (value is String)
				{
					String profilePictureUrl = (string)value;
					if (profilePictureUrl == "")
						return "defaultprofilepic.png";
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
