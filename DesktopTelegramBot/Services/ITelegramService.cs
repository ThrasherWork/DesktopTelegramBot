using System;
using System.Threading.Tasks;
using DesktopTelegramBot.EventArgs;
using DesktopTelegramBot.Models;

namespace DesktopTelegramBot.Services
{
	interface ITelegramService
	{
		UserInfo BotInfo { get; }

		Task Start();
		void Stop();

		void SendMessage( ChatMessage message );
		Task<byte[]> GetPhotoAsync( int userId );

		event EventHandler<NewMessageEventArgs> NewMessage;
	}
}
