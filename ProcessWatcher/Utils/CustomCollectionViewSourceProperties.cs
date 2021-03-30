using System;
using System.Windows;
using System.Windows.Data;

namespace ProcessWatcher.Utils
{
	public class CustomPropertyGroupDescription : PropertyGroupDescription
	{
		public bool Test { get; set; }
	}
	// public class CustomCollectionViewSourceProperties
	// {
	// 	public static readonly DependencyProperty IsBubbleSourceProperty = DependencyProperty.RegisterAttached(
	// 		"IsBubbleSource",
	// 		typeof(Boolean),
	// 		typeof(AquariumObject2)
	// 		);
	// 	public static void SetIsBubbleSource(UIElement element, Boolean value)
	// 	{
	// 		element.SetValue(IsBubbleSourceProperty, value);
	// 	}
	// 	public static Boolean GetIsBubbleSource(UIElement element)
	// 	{
	// 		return (Boolean)element.GetValue(IsBubbleSourceProperty);
	// 	}
	// }
}