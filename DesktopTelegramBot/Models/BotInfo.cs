namespace DesktopTelegramBot.Models
{
	class UserInfo
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public byte[] Image { get; set; }

		public static UserInfo Create( int id, string name, byte[] image = null )
		{
			return new UserInfo
			{
				Id = id,
				Name = name,
				Image = image,
			};
		}

		private UserInfo()
		{
		}
	}
}
