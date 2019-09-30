using Catel.MVVM;

namespace DesktopTelegramBot.Messages
{
	class RestoreExistingViewMessage
	{
		public IViewModel ViewModel { get; set; }
		public bool Restored { get; set; }

		public static RestoreExistingViewMessage Create( IViewModel viewModel )
		{
			return new RestoreExistingViewMessage( viewModel );
		}

		private RestoreExistingViewMessage( IViewModel viewModel )
		{
			ViewModel = viewModel;
		}
	}
}
