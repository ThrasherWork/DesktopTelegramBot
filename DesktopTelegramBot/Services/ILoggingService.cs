using System;

namespace DesktopTelegramBot.Services
{
	public interface ILoggingService
	{
		void Info( string message );
		void Error( Exception ex );
	}
}
