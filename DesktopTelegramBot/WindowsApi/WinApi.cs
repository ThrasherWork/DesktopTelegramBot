using System;
using System.Runtime.InteropServices;

namespace DesktopTelegramBot.WindowsApi
{
	static class WinApi
	{
		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool ShowWindow( IntPtr hWnd, WindowShowStyle nCmdShow );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool SetForegroundWindow( IntPtr hWnd );

		public enum WindowShowStyle
		{
			Restore = 9,
		}
	}
}
