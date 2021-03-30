using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using ProcessWatcher.Core;
using ProcessWatcher.ViewModels;
using Splat;

namespace ProcessWatcher
{
	// public interface IProcessConfiguration
	// {
	// 	string Path { get; set; }
	// 	bool AutoRestart { get; set; }
	// 	bool IsValid { get; }
	// }
	// public class ProcessConfiguration : IProcessConfiguration
	// {
	// 	public ProcessConfiguration(string path, bool autoRestart)
	// 	{
	// 		Path = path;
	// 		AutoRestart = autoRestart;
	// 	}
	//
	// 	public string Path { get; set; }
	// 	public bool AutoRestart { get; set; }
	//
	// 	public bool IsValid => File.Exists(Path);
	// }

	public static class ProcessConfigurationExtensions
	{
		public const string PathKey = "Percorso";
		public const string AutoRestartKey = "AutoRestart";
		public static string ToPathKey(this int num) => $"{PathKey}{num}";
		public static string ToPathKey(this long num) => $"{PathKey}{num}";
		public static string ToAutoRestartKey(this int num) => $"{AutoRestartKey}{num}";
		public static string ToAutoRestartKey(this long num) => $"{AutoRestartKey}{num}";
	}
	public class AppConfig
	{

		public ObservableCollection<IProcessViewModel> ProcessConfigurations { get; }
		public int LogsBufferSize { get; }

		public AppConfig()
		{
			LogsBufferSize = int.Parse(GetKeyFromConfigManager(nameof(LogsBufferSize)));
			ProcessConfigurations = new ObservableCollection<IProcessViewModel>();
			for (int i = 1; i < 100; i++)
			{
				try
				{
					var path = GetKeyFromConfigManager(i.ToPathKey());
					var pathAndName = Locator.Current.GetService<IProcessFactory>().BuildProcessViewModel(path, bool.Parse(GetKeyFromConfigManager(i.ToAutoRestartKey())));
					if(pathAndName.IsValid)
						ProcessConfigurations.Add(pathAndName);
				}
				catch
				{
					break;
				}
			}
		}


		public bool AddNewProcess(IProcessViewModel processConfiguration)
		{
			lock (ProcessConfigurations)
			{
				if (ProcessConfigurations.Any(e => e.Path.Equals(processConfiguration.Path, StringComparison.InvariantCultureIgnoreCase)))
					return false;
				ProcessConfigurations.Add(processConfiguration);
				var indexOf = ProcessConfigurations.IndexOf(processConfiguration) + 1;
				AddUpdateAppSettings(indexOf.ToPathKey(), processConfiguration.Path);
				AddUpdateAppSettings(indexOf.ToAutoRestartKey(), processConfiguration.AutoRestart.ToString());
				return true;
			}
		}

		public bool UpdateProcess(IProcessViewModel processConfiguration)
		{
			lock (ProcessConfigurations)
			{
				var processConfigurationToUpdate = ProcessConfigurations.FirstOrDefault(e => e.Path.Equals(processConfiguration.Path, StringComparison.InvariantCultureIgnoreCase));
				if (processConfigurationToUpdate == null)
					return false;
				processConfigurationToUpdate.Path = processConfiguration.Path;
				processConfigurationToUpdate.AutoRestart = processConfiguration.AutoRestart;
				var indexOf = ProcessConfigurations.IndexOf(processConfigurationToUpdate) + 1;
				AddUpdateAppSettings(indexOf.ToPathKey(), processConfigurationToUpdate.Path);
				AddUpdateAppSettings(indexOf.ToAutoRestartKey(), processConfigurationToUpdate.AutoRestart.ToString());
				return true;
			}
		}
		public bool RemoveProcess(IProcessViewModel processConfiguration)
		{
			lock (ProcessConfigurations)
			{
				var processConfigurationToRem = ProcessConfigurations.FirstOrDefault(e => e.Path.Equals(processConfiguration.Path, StringComparison.InvariantCultureIgnoreCase));
				if (processConfigurationToRem == null)
					return false;
				var removed = ProcessConfigurations.Remove(processConfigurationToRem);
				if (!removed) return false;
				foreach (var x1 in ProcessConfigurations.Select((configuration, i) => new {Configuration = configuration, Index = i+1}))
				{
					AddUpdateAppSettings(x1.Index.ToPathKey(), x1.Configuration.Path);
					AddUpdateAppSettings(x1.Index.ToAutoRestartKey(), x1.Configuration.AutoRestart.ToString());
				}

				var itemToRem = ProcessConfigurations.Count + 1;
				RemoveNumericSettings(itemToRem.ToPathKey());
				RemoveNumericSettings(itemToRem.ToAutoRestartKey());
				return true;
			}
		}


		private string GetKeyFromConfigManager(string name) => ConfigurationManager.AppSettings[name];
		// internal void SetupPasswords()
		// {
		// 	RSACryptoServiceProvider key = new RSACryptoServiceProvider(new CspParameters
		// 	{
		// 		KeyContainerName = "OmniLog"
		// 	});
		// 	RSAPKCS1KeyExchangeFormatter rsapkcs1KeyExchangeFormatter = new RSAPKCS1KeyExchangeFormatter(key);
		// 	RSAPKCS1KeyExchangeDeformatter rsapkcs1KeyExchangeDeformatter = new RSAPKCS1KeyExchangeDeformatter(key);
		// 	try
		// 	{
		// 		this.SQLDecryptedPassword = Encoding.ASCII.GetString(rsapkcs1KeyExchangeDeformatter.DecryptKeyExchange(Convert.FromBase64String(this.SQLPassword)));
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		this.SQLDecryptedPassword = this.SQLPassword;
		// 		this.SQLPassword = Convert.ToBase64String(rsapkcs1KeyExchangeFormatter.CreateKeyExchange(Encoding.ASCII.GetBytes(this.SQLDecryptedPassword)));
		// 	}
		// }
		//
		static void AddUpdateAppSettings(string key, string value)
		{
			try
			{
				var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				var settings = configFile.AppSettings.Settings;
				if (settings[key] == null)
				{
					settings.Add(key, value);
				}
				else
				{
					settings[key].Value = value;
				}
				configFile.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
			}
			catch (ConfigurationErrorsException)
			{

			}
		}
		static void RemoveNumericSettings(string key)
		{
			try
			{
				var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				var settings = configFile.AppSettings.Settings;
				if (settings[key] == null)
					return;
				settings.Remove(key);
				configFile.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
			}
			catch (ConfigurationErrorsException)
			{

			}
		}
	}
}