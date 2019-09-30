using Catel.MVVM.Converters;
using System;
using System.Globalization;

namespace DesktopTelegramBot.Converters
{
	class UtcToLocalTimeConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( !(value is DateTime utc) )
			{
				return default( DateTime );
			}

			var local = utc.ToLocalTime();
			return local;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
}
