using DesktopTelegramBot.Models;

namespace DesktopTelegramBot.Services
{
	interface ISettingsService
	{
		Settings Settings { get; }
		void Load();
		void LoadDefault();
	}
}
