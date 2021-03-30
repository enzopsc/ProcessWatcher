using System.Threading;
using System.Windows;
using System.Windows.Threading;
using BECCore.AutoLog;
using log4net.Core;
using ProcessWatcher.Core;
using Splat;

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
            Locator.CurrentMutable.RegisterConstant<IProcessFactory>(new ProcessFactory());
            Statics.Initialize();
            Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
            SetLanguageDictionary();
            Logging.Logger.Debug("-> App -> App : " + "Started Application");
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
                    Language.Culture = new System.Globalization.CultureInfo("it-IT");
                    break;
                case "en-US":
                    Language.Culture = new System.Globalization.CultureInfo("en-US");
                    break;
                default:
                    Language.Culture = new System.Globalization.CultureInfo("it-IT");
                    break;
            }
        }
    }
}