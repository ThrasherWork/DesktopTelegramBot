using System.Media;

namespace DesktopTelegramBot.Services
{
	class NotificationService : INotificationService
	{
		public void NotifyReceived()
		{
			SystemSounds.Asterisk.Play();
		}

		public void NotifySent()
		{
			SystemSounds.Beep.Play();
		}
	}
}
