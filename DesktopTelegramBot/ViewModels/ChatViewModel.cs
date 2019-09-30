using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;
using Catel.Collections;
using Catel.IoC;
using Catel.MVVM;
using DesktopTelegramBot.Models;
using DesktopTelegramBot.Services;

namespace DesktopTelegramBot.ViewModels
{
	sealed class ChatViewModel : BaseViewModel
	{
		private ITelegramService _telegramService;
		private INotificationService _notificationService;

		private byte[] _userImage;

		public Chat Chat { get; }
		public FastObservableCollection<ChatMessage> Messages { get; }
		public string CurrentMessage { get; set; }
		public ChatMessage LastMessage { get; private set; }

		public byte[] UserImage
		{
			get => _userImage;
			private set
			{
				_userImage = value;
				Chat.UserInfo.Image = _userImage;
			}
		}


		private Command _sendCommand;
		public Command SendCommand => _sendCommand ?? ( _sendCommand = new Command( Send ) );


		public static ChatViewModel Create( Chat chat )
		{
			IServiceLocator serviceLocator = ServiceLocator.Default;

			return new ChatViewModel( chat )
			{
				_telegramService = serviceLocator.ResolveType<ITelegramService>(),
				_notificationService = serviceLocator.ResolveType<INotificationService>(),
			};
		}

		private ChatViewModel( Chat chat )
		{
			Chat = chat;
			Messages = new FastObservableCollection<ChatMessage>();

			Title = chat.UserInfo.Name;
		}

		public async Task LoadImageAsync()
		{
			try
			{
				int userId = Chat.UserInfo.Id;

				Logger.Info( $"Loading image for user {userId}" );

				byte[] photoBytes = await _telegramService.GetPhotoAsync( userId );

				if ( photoBytes == null )
				{
					Logger.Info( $"No image for user {userId}" );
					return;
				}

				Logger.Info( $"Image loaded for user {userId}" );

				UserImage = photoBytes;
				//Chat.UserInfo.Image = photoBytes;
			}
			catch ( Exception ex )
			{
				Chat.UserInfo.Image = null;

				Logger.Error( ex );
			}
		}

		public void AddMessage( ChatMessage newMessage )
		{
			ChatMessage existingMessage = Messages.FirstOrDefault( x => x.Id == newMessage.Id );

			if ( existingMessage == null )
			{
				AddMessageToList( newMessage );

				if ( SettingsProvider.Settings.PlaySound )
				{
					_notificationService.NotifyReceived();
				}
			}
			else
			{
				existingMessage.ApplyEdit( newMessage );
			}
		}

		public void Send()
		{
			try
			{
				string messateText = CurrentMessage.Trim();

				if ( string.IsNullOrEmpty( messateText ) || string.IsNullOrWhiteSpace( messateText ) )
				{
					return;
				}

				var chatMessage = ChatMessage.Create( 0, _telegramService.BotInfo.Id, Chat, DateTime.UtcNow, ChatMessageType.Outgoing, messateText );
				CurrentMessage = string.Empty;

				if ( SettingsProvider.Settings.PlaySound )
				{
					_notificationService.NotifySent();
				}

				AddMessageToList( chatMessage );

				_telegramService.SendMessage( chatMessage );
			}
			catch ( Exception ex )
			{
				ProcessException( ex );
			}
		}

		private void AddMessageToList( ChatMessage newMessage )
		{
			Messages.Add( newMessage );
			LastMessage = newMessage;
		}
	}
}
