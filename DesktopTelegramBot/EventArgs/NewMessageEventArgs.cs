using DesktopTelegramBot.Models;

namespace DesktopTelegramBot.EventArgs
{
	class NewMessageEventArgs : System.EventArgs
	{
		public ChatMessage Message { get; private set; }

		private NewMessageEventArgs()
		{
			
		}

		public static NewMessageEventArgs Create( ChatMessage message )
		{
			return new NewMessageEventArgs
			{
				Message = message
			};
		}
	}
}
