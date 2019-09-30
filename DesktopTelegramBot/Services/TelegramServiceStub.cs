using System;
using System.Threading;
using System.Threading.Tasks;
using DesktopTelegramBot.EventArgs;
using DesktopTelegramBot.Models;
using DesktopTelegramBot.Tools;
using Chat = DesktopTelegramBot.Models.Chat;

namespace DesktopTelegramBot.Services
{
	class TelegramServiceStub : ITelegramService
	{
		private readonly ILoggingService _loggingService;
		private bool _isRunning;
		private readonly Random _randy = new Random( DateTime.Now.Millisecond );

		public TelegramServiceStub( ILoggingService loggingService )
		{
			_loggingService = loggingService;
		}

		public UserInfo BotInfo => UserInfo.Create( 11, "[DEMO-BOT]" );

		public event EventHandler<NewMessageEventArgs> NewMessage;

		public async Task Start()
		{
			await Task.Delay( 500 );

			_isRunning = true;
			Task.Run( (Action)GenerateMessages );
		}

		public void Stop()
		{
			_isRunning = false;
		}

		public void SendMessage( ChatMessage message )
		{
			_loggingService.Info( $"Sent message: {message.Message}" );
		}

		private void GenerateMessages()
		{
			while ( _isRunning )
			{
				Thread.Sleep( 2000 );

				int chatId = _randy.Next( 3, 15 );
				string username = $"Chat №{chatId}";

				var chatMessage = ChatMessage.Create(
					_randy.Next( 1, 30 ),
					chatId,
					Chat.Create( chatId, UserInfo.Create( chatId, username ) ),
					DateTime.UtcNow,
					ChatMessageType.Incoming,
					Guid.NewGuid().ToString( "N" ) );

				_loggingService.Info( $"New message from '{chatMessage.Chat.Id} ({username})': {chatMessage.Message}" );

				var messageEventArgs = NewMessageEventArgs.Create( chatMessage );

				NewMessage.Raise( this, messageEventArgs );
			}
		}

		public async Task<byte[]> GetPhotoAsync( int userId )
		{
			return null;
		}
	}
}
