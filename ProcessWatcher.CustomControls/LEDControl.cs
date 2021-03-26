using System.Windows;
using System.Windows.Controls;

namespace ProcessWatcher.CustomControls
{
	public enum LEDStatus
	{
		White,
		Black,
		Green,
		Red,
		Orange,
		RedToOrange
	}
	public class LEDControl : Control
	{
		public static readonly DependencyProperty TooltipContentProperty = DependencyProperty.Register(
			nameof(TooltipContent), typeof(string), typeof(LEDControl), new PropertyMetadata(default(string)));

		public string TooltipContent
		{
			get { return (string) GetValue(TooltipContentProperty); }
			set { SetValue(TooltipContentProperty, value); }
		}
		public static readonly DependencyProperty LEDStatusProperty = DependencyProperty.Register(
			nameof(LEDStatus), typeof(LEDStatus), typeof(LEDControl), new PropertyMetadata(default(LEDStatus)));

		public LEDStatus LEDStatus
		{
			get { return (LEDStatus) GetValue(LEDStatusProperty); }
			set { SetValue(LEDStatusProperty, value); }
		}

		static LEDControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LEDControl), new FrameworkPropertyMetadata(typeof(LEDControl)));
		}

	}
}