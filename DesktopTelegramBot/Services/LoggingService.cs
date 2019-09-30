using System;
using System.Diagnostics;

namespace DesktopTelegramBot.Services
{
	class LoggingService : ILoggingService
	{
		private const string DateTimeFormat = "dd.MM.yyyy HH:mm:ss.mmm";

		private string TimeStamp => DateTime.Now.ToString( DateTimeFormat );

		public void Info( string message )
		{
			Debug.WriteLine( $"[{TimeStamp}] {message}" );
		}

		public void Error( Exception ex )
		{
			Debug.WriteLine( $"[{TimeStamp}] Error: {ex.Message}\r\n{ex.StackTrace}" );
		}
	}
}
