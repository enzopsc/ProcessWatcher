using System;
using Notifications.Wpf;

namespace ProcessWatcher
{
	public class NotificationEventArgs : EventArgs
	{
		public string Title { get; }
		public string Message { get; }
		public NotificationType NotificationType { get; }
		public NotificationEventArgs(string title, string message, NotificationType notificationType)
		{
			Title = title;
			Message = message;
			NotificationType = notificationType;
		}
	}
}