using System.Windows;
using System.Windows.Interop;
using Catel.Messaging;
using DesktopTelegramBot.Messages;
using DesktopTelegramBot.WindowsApi;

namespace DesktopTelegramBot.Views
{
	public partial class ChatWindow
	{
		public ChatWindow()
		{
			InitializeComponent();

			Closing += OnClosing;

			MessageMediator.Default.Register<RestoreExistingViewMessage>( this, OnRestoreMessage );
		}

		private void OnClosing( object sender, System.ComponentModel.CancelEventArgs e )
		{
			MessageMediator.Default.Unregister<RestoreExistingViewMessage>( this, OnRestoreMessage );
		}

		private void OnRestoreMessage( RestoreExistingViewMessage message )
		{
			if ( message.ViewModel != DataContext ) return;

			//SystemCommands.RestoreWindow( this );

			var handleSource = PresentationSource.FromVisual( this ) as HwndSource;
			System.IntPtr handle = handleSource.Handle;

			WinApi.ShowWindow( handle, WinApi.WindowShowStyle.Restore );
			WinApi.SetForegroundWindow( handle );

			message.Restored = true;
		}
	}
}
