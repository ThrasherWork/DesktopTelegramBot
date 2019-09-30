namespace DesktopTelegramBot.Models
{
	class Chat
	{
		public long Id { get; private set; }
		public UserInfo UserInfo { get; private set; }

		private Chat()
		{
		}

		public static Chat Create( long id, UserInfo userInfo )
		{
			return new Chat
			{
				Id = id,
				UserInfo = userInfo,
			};
		}
	}
}
