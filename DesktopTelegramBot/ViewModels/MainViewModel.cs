using Catel.Collections;
using Catel.IoC;
using Catel.Messaging;
using Catel.MVVM;
using Catel.Services;
using DesktopTelegramBot.Messages;
using DesktopTelegramBot.Models;
using DesktopTelegramBot.Services;
using DesktopTelegramBot.Tools;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopTelegramBot.ViewModels
{
	sealed class MainViewModel : BaseViewModel
	{
		private ITelegramService _telegramService;
		private IDispatcherService _dispatcherService;
		private IUIVisualizerService _visualizer;

		public bool IsLoading { get; private set; }
		public UserInfo BotInfo { get; private set; }
		public FastObservableCollection<ChatViewModel> Chats { get; }

		public bool NotificationsOn { get; private set; }


		private TaskCommand _loadCommand;
		public TaskCommand LoadCommand => _loadCommand ?? ( _loadCommand = new TaskCommand( Load ) );

		private Command<ChatViewModel> _openChatCommand;
		public Command<ChatViewModel> OpenChatCommand => _openChatCommand ?? ( _openChatCommand = new Command<ChatViewModel>( OpenChat ) );

		private Command _switchNotificationsCommand;
		public Command SwitchNotificationsCommand => _switchNotificationsCommand ?? ( _switchNotificationsCommand = new Command( SwitchNotifications ) );


		public static MainViewModel Create()
		{
			IServiceLocator serviceLocator = ServiceLocator.Default;
			
			var instance = new MainViewModel
			{
				_telegramService = serviceLocator.ResolveType<ITelegramService>(),
				_dispatcherService = serviceLocator.ResolveType<IDispatcherService>(),
				_visualizer = serviceLocator.ResolveType<IUIVisualizerService>(),
			};

			instance._telegramService.NewMessage += instance.OnTelegramServiceNewMessage;
			instance.NotificationsOn = instance.SettingsProvider.Settings.PlaySound;

			return instance;
		}

		private MainViewModel()
		{
			Chats = new FastObservableCollection<ChatViewModel>();

			Title = AppInfo.Name;
		}

		private async Task Load()
		{
			try
			{
				IsLoading = true;

				await _telegramService.Start();
				BotInfo = _telegramService.BotInfo;

				Title = $"[{_telegramService.BotInfo.Name}] - {AppInfo.Name}";
			}
			catch ( Exception ex )
			{
				ProcessException( ex );
			}
			finally
			{
				IsLoading = false;
			}
		}

		protected override async Task OnClosingAsync()
		{
			await base.OnClosingAsync();

			try
			{
				_telegramService.Stop();
			}
			catch ( Exception ex )
			{
				Logger.Error( ex );
			}
		}

		private void OnTelegramServiceNewMessage( object sender, EventArgs.NewMessageEventArgs e )
		{
			_dispatcherService.InvokeIfRequired( async () =>
			{
				try
				{
					var chat = Chats.FirstOrDefault( x => x.Chat.Id == e.Message.Chat.Id );
					bool newChat = chat == null;

					if ( newChat )
					{
						chat = ChatViewModel.Create( e.Message.Chat );
						Chats.Add( chat );
					}

					chat.AddMessage( e.Message );
					Chats.MoveItemToTop( chat );

					if ( newChat )
					{
						await chat.LoadImageAsync();
					}
				}
				catch ( Exception ex )
				{
					ProcessException( ex );
				}
			} );
		}

		private async void OpenChat( ChatViewModel chat )
		{
			try
			{
				Logger.Info( $"Gonna open chat '{chat.Chat.UserInfo.Name}'" );

				var restoreViewMessage = RestoreExistingViewMessage.Create( chat );
				MessageMediator.Default.SendMessage( restoreViewMessage );

				if ( restoreViewMessage.Restored )
				{
					return;
				}

				await _visualizer.ShowAsync( chat );
			}
			catch ( Exception ex )
			{
				ProcessException( ex );
			}
		}

		private void SwitchNotifications()
		{
			SettingsProvider.Settings.PlaySound = !SettingsProvider.Settings.PlaySound;

			NotificationsOn = SettingsProvider.Settings.PlaySound;
		}
	}
}
