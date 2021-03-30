using System;
using System.Globalization;
using System.Windows.Data;
using ProcessWatcher.CustomControls;
using ProcessWatcher.ViewModels;

namespace ProcessWatcher.Utils
{
	public class StatusToLEDStatusConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ProcessStatus processStatus)
			{
				switch (processStatus)
				{
					case ProcessStatus.Error:
						return LEDStatus.Black;
					case ProcessStatus.Created:
						return LEDStatus.White;
					case ProcessStatus.Starting:
						return LEDStatus.RedToOrange;
					case ProcessStatus.Running:
						return LEDStatus.Green;
					case ProcessStatus.Stopped:
						return LEDStatus.Red;
					case ProcessStatus.ManuallyStopped:
						return LEDStatus.Orange;
				}
			}
			return LEDStatus.Black;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}