using System;
using Catel.Collections;

namespace DesktopTelegramBot.Tools
{
	static class Extensions
	{
		public static void Raise<TArgsType>( this EventHandler<TArgsType> pHandler, object pSender, TArgsType pArgs )
			where TArgsType : System.EventArgs
		{
			EventHandler<TArgsType> handler = pHandler;
			handler?.Invoke( pSender, pArgs );
		}

		public static void MoveItemToTop<T>( this FastObservableCollection<T> collection, T item )
		{
			using ( collection.SuspendChangeNotifications() )
			{
				collection.Remove( item );
				collection.Insert( 0, item );
			}
		}
	}
}
