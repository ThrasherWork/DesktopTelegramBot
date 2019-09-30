namespace DesktopTelegramBot.Models
{
	class Settings
	{
		public string BotToken { get; set; }
		public string ProxyIp { get; set; }
		public int ProxyPort { get; set; }
		public int TelegramTimeoutInSeconds { get; set; }

		public bool PlaySound { get; set; }
	}
}
