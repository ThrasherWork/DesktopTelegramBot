using Catel.IoC;
using Catel.Services;
using DesktopTelegramBot.Helpers;
using DesktopTelegramBot.Services;
using DesktopTelegramBot.ViewModels;
using System;
using System.Windows;

namespace DesktopTelegramBot
{
	public partial class App
	{
		public static ILoggingService Logger { get; private set; }

		protected override async void OnStartup( StartupEventArgs e )
		{
			base.OnStartup( e );

#if DEBUG
			//LogManager.AddDebugListener();
#endif

			var serviceLocator = this.GetServiceLocator();

			try
			{
				bool singleInstance = SingleInstanceHelper.EnsureSingleInstance( AppInfo.Id );
				if ( !singleInstance )
				{
					return;
				}

				Register( serviceLocator );

				Logger = serviceLocator.ResolveType<ILoggingService>();
				var visualizer = serviceLocator.ResolveType<IUIVisualizerService>();
				var settingsService = serviceLocator.ResolveType<ISettingsService>();

				Logger.Info( $"App started ({AppInfo.Version})" );

				settingsService.Load();

				var mainViewModel = MainViewModel.Create();
				await visualizer.ShowDialogAsync( mainViewModel );

				Logger.Info( "App gonna close" );
			}
			catch ( Exception ex )
			{
				ProcessUnhandledException( ex );
			}
			finally
			{
				Shutdown();
			}
		}

		private void Register( IServiceLocator locator )
		{
			locator.RegisterType<ILoggingService, LoggingService>();
			locator.RegisterType<ISettingsService, SettingsService>();
			locator.RegisterType<INotificationService, NotificationService>();
			locator.RegisterType<ITelegramService, TelegramService>();

#if DEBUG
			//locator.RegisterType<ITelegramService, TelegramServiceStub>();
#endif
		}

		private void ProcessUnhandledException( Exception ex )
		{
			MessageBox.Show(
				$"Ошибка при выполнении программы:\r\n\r\n{ex.Message}\r\n\r\n{ex.StackTrace}",
				"Внимание",
				MessageBoxButton.OK,
				MessageBoxImage.Error );
		}
	}
}
