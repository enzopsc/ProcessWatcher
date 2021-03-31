using System;

namespace ProcessWatcher
{
	public static class Statics
	{
		private static AppConfig _appConfig;
		public static AppConfig AppConfig => _appConfig;
		private static bool _isInitialized;
		public static void Initialize()
		{
			if (_isInitialized) return;
			_isInitialized = true;
			BECCore.AutoLog.Logging.Setup();
			_appConfig = new AppConfig();
			_appConfig.InitProcessViewModels();
		}

		public static event EventHandler<NotificationEventArgs> NotificationEvent;

		public static void Notify(object sender, NotificationEventArgs notificationEventArgs)
		{
			NotificationEvent?.Invoke(sender, notificationEventArgs);
		}
	}
}