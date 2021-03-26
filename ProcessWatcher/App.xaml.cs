using System.Threading;
using System.Windows;
using System.Windows.Threading;
using BECCore.AutoLog;
using log4net.Core;

namespace ProcessWatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private AppBootstrapper _appBootstrapper;
        public App()
        {
            BECCore.AutoLog.Logging.Setup("Logs/Log.log", Level.All);
            Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
            SetLanguageDictionary();
            Logging.Logger.Debug("-> App -> App : " + "Started Application");
            //Unosquare.FFME.Library.FFmpegDirectory = ConfigurationManager.AppSettings.Get("FFMPEG");
            _appBootstrapper = new AppBootstrapper();
        }
        private void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logging.Logger.Fatal("-> App -> Unhandled : " , e.Exception);
            e.Handled = true;
        }

        private void SetLanguageDictionary()
        {
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "it-IT":
                    Language.Resources.Language.Culture = new System.Globalization.CultureInfo("it-IT");
                    break;
                case "en-US":
                    Language.Resources.Language.Culture = new System.Globalization.CultureInfo("en-US");
                    break;
                default:
                    Language.Resources.Language.Culture = new System.Globalization.CultureInfo("it-IT");
                    break;
            }
        }
    }
}