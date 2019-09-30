using DesktopTelegramBot.Models;

namespace DesktopTelegramBot.Services
{
	class SettingsService : ISettingsService
	{
		public Settings Settings { get; private set; }

		public void Load()
		{
			LoadDefault();
		}

		public void LoadDefault()
		{
			Settings = new Settings
			{
				BotToken = "",
				ProxyIp = "139.180.139.73",
				ProxyPort = 1080,

				TelegramTimeoutInSeconds = 0,

				PlaySound = true,
			};
		}
	}
}
