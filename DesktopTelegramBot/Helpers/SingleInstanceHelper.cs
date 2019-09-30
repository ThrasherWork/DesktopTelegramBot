using DesktopTelegramBot.WindowsApi;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DesktopTelegramBot.Helpers
{
	class SingleInstanceHelper
	{
		private static Mutex _mutex;

		public static bool EnsureSingleInstance( string appId )
		{
			bool singleInstance = IsSingleInstance( appId );

			if ( !singleInstance )
			{
				ShowRunningApp();
			}

			return singleInstance;
		}

		public static bool IsSingleInstance( string appId )
		{
			_mutex = new Mutex( true, appId );

			if ( _mutex.WaitOne( TimeSpan.Zero, true ) )
				return true;

			return false;
		}

		private static void ShowRunningApp()
		{
			var currentProcess = Process.GetCurrentProcess();
			var otherProcess = Process
				.GetProcessesByName( currentProcess.ProcessName )
				.FirstOrDefault( x => x.Id != currentProcess.Id );

			if ( otherProcess != null )
			{
				WinApi.ShowWindow( otherProcess.MainWindowHandle, WinApi.WindowShowStyle.Restore );
				WinApi.SetForegroundWindow( otherProcess.MainWindowHandle );
			}
		}
	}
}
