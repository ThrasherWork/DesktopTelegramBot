using DesktopTelegramBot.EventArgs;
using DesktopTelegramBot.Models;
using DesktopTelegramBot.Tools;
using MihaZupan;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace DesktopTelegramBot.Services
{
	class TelegramService : ITelegramService
	{
		private readonly ILoggingService _loggingService;
		private readonly ISettingsService _settingsService;

		private bool _isRunning;
		private ITelegramBotClient _telegramClient;
		private readonly ConcurrentQueue<ChatMessage> _messagesQueue;

		public UserInfo BotInfo { get; private set; }

		public TelegramService(
			ILoggingService loggingService,
			ISettingsService settingsService )
		{
			_loggingService = loggingService;
			_settingsService = settingsService;

			_messagesQueue = new ConcurrentQueue<ChatMessage>();
		}

		public event EventHandler<NewMessageEventArgs> NewMessage;

		public async Task Start()
		{
			if ( _isRunning )
				return;

			try
			{
				_isRunning = true;

				_telegramClient = CreateClient( _settingsService.Settings );
				Subscribe( _telegramClient );

				BotInfo = await GetInfoAsync( _telegramClient );

				_telegramClient.StartReceiving();
				StartSending();

				_loggingService.Info( "TelegramService started" );
			}
			catch
			{
				_isRunning = false;
				throw;
			}
		}

		public void Stop()
		{
			if ( !_isRunning )
				return;

			try
			{
				_isRunning = false;

				_telegramClient.StopReceiving();
				Unsubscribe( _telegramClient );

				_loggingService.Info( "TelegramService stopped" );
			}
			catch
			{
				_isRunning = false;
				throw;
			}
		}

		private TelegramBotClient CreateClient( Settings settings )
		{
			var proxy = new HttpToSocks5Proxy( settings.ProxyIp, settings.ProxyPort );
			var client = new TelegramBotClient( settings.BotToken, proxy );

			if ( settings.TelegramTimeoutInSeconds > 0 )
			{
				client.Timeout = TimeSpan.FromSeconds( settings.TelegramTimeoutInSeconds );
			}

			return client;
		}

		private async Task<UserInfo> GetInfoAsync( ITelegramBotClient telegramClient )
		{
			var user = await telegramClient.GetMeAsync();
			byte[] image = await GetPhotoAsync( user.Id );

			var botInfo = UserInfo.Create( user.Id, user.Username, image );
			return botInfo;
		}

		private void Subscribe( ITelegramBotClient telegramClient )
		{
			telegramClient.OnMessage += OnClientMessage;
			telegramClient.OnMessageEdited += OnClientMessage;
		}

		private void Unsubscribe( ITelegramBotClient telegramClient )
		{
			telegramClient.OnMessage -= OnClientMessage;
			telegramClient.OnMessageEdited -= OnClientMessage;
		}

		private void OnClientMessage( object sender, MessageEventArgs e )
		{
			try
			{
				string username = $"{e.Message.Chat.FirstName} {e.Message.Chat.LastName}".Trim();

				if ( e.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text )
				{
					_loggingService.Info( $"Unknown image type from {username} : {e.Message.Type}" );
					return;
				}

				var chatMessage = ChatMessage.Create(
					e.Message.MessageId,
					e.Message.From.Id,
					Chat.Create( e.Message.Chat.Id, UserInfo.Create( e.Message.From.Id, username ) ),
					e.Message.Date,
					ChatMessageType.Incoming,
					e.Message.Text );

				_loggingService.Info( $"New message from '{chatMessage.Chat.Id} ({username})': {chatMessage.Message}" );

				var messageEventArgs = NewMessageEventArgs.Create( chatMessage );

				NewMessage.Raise( this, messageEventArgs );
			}
			catch ( Exception ex )
			{
				_loggingService.Error( ex );
			}
		}

		public void SendMessage( ChatMessage message )
		{
			_messagesQueue.Enqueue( message );

			_loggingService.Info( $"Message '{message.Id}' added to queue: {message.Message}" );
		}

		public async Task<byte[]> GetPhotoAsync( int userId )
		{
			var photos = await _telegramClient.GetUserProfilePhotosAsync( userId, 0, 1 );
			var photoId = photos.Photos.FirstOrDefault()?.FirstOrDefault()?.FileId;

			if ( photoId == null )
			{
				return null;
			}

			var file = await _telegramClient.GetFileAsync( photoId );

			using ( var stream = new MemoryStream() )
			{
				await _telegramClient.DownloadFileAsync( file.FilePath, stream );

				var bytes = stream.ToArray();
				return bytes;
			}
		}

		public void StartSending()
		{
			Task.Run( async () =>
			{
				while ( _isRunning )
				{
					await SendMessageInternalAsync();
				}
			} );
		}

		private async Task SendMessageInternalAsync()
		{
			bool dequeued = _messagesQueue.TryPeek( out ChatMessage message );

			if ( !dequeued )
			{
				Thread.Sleep( 50 );
				return;
			}

			try
			{
				var x = await _telegramClient.SendTextMessageAsync( message.Chat.Id, message.Message );
				message.Id = x.MessageId;

				_loggingService.Info( $"Message '{message.Id}' sent" );
			}
			catch ( Exception ex )
			{
				_loggingService.Error( ex );
				return;
			}

			_messagesQueue.TryDequeue( out message );
		}
	}
}
