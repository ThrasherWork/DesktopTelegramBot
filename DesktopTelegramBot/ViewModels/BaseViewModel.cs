using System;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using DesktopTelegramBot.Services;

namespace DesktopTelegramBot.ViewModels
{
	abstract class BaseViewModel : ViewModelBase
	{
		private readonly IMessageService _messageService;
		protected ILoggingService Logger { get; }
		protected ISettingsService SettingsProvider { get; }

		protected BaseViewModel()
		{
			IServiceLocator serviceLocator = ServiceLocator.Default;

			_messageService = serviceLocator.ResolveType<IMessageService>();
			Logger = serviceLocator.ResolveType<ILoggingService>();
			SettingsProvider = serviceLocator.ResolveType<ISettingsService>();
		}

		protected void ProcessException( Exception ex, string errorMessage = null )
		{
			Logger.Error( ex );

			if ( !string.IsNullOrEmpty( errorMessage ) )
			{
				_messageService.ShowErrorAsync( errorMessage );
			}
		}
	}
}
