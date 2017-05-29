using System;
using System.Globalization;
using Xamarin.Forms;

namespace LK
{
    public class ChatDateFormatConverter : IValueConverter
    {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
            if (value is DateTime)
            {
                DateTime dt = (DateTime)value;
                TimeSpan timeSince = DateTime.Now - dt;
                if (timeSince > TimeSpan.FromDays(7))
                {
                    CultureInfo ci = new CultureInfo("nb-NO");
                    return dt.ToString("dd.MM.yy", ci);
                }
                else if (timeSince > TimeSpan.FromDays(1))
                {
                    return String.Format("{0:%d} d", timeSince);
                }
                else if (timeSince >= TimeSpan.FromHours(1))
                {
                    return String.Format("{0:%h} t", timeSince);
                }
                else if (timeSince >= TimeSpan.FromMinutes(1))
                {
                    return String.Format("{0:%m} min", timeSince);
                }
                else
                {
                    return String.Format("Nå", timeSince);
                }
            }
            else return (DateTime)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

    }
}
