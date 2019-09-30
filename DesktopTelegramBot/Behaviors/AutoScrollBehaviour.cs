using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace DesktopTelegramBot.Behaviors
{
	public class AutoScrollBehavior : Behavior<ScrollViewer>
	{
		private bool _autoScroll = true;
		private ScrollViewer _scrollViewer;

		protected override void OnAttached()
		{
			base.OnAttached();

			_scrollViewer = AssociatedObject;
			_scrollViewer.ScrollChanged += OnScrollChanged;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			if ( _scrollViewer != null )
			{
				_scrollViewer.ScrollChanged -= OnScrollChanged;
			}
		}

		private void OnScrollChanged( object sender, ScrollChangedEventArgs e )
		{
			if ( Math.Abs( e.ExtentHeightChange ) < double.Epsilon )
			{
				_autoScroll = Math.Abs( _scrollViewer.VerticalOffset - _scrollViewer.ScrollableHeight ) < double.Epsilon;
			}

			if ( _autoScroll && Math.Abs( e.ExtentHeightChange ) > double.Epsilon )
			{
				_scrollViewer.ScrollToVerticalOffset( _scrollViewer.ExtentHeight );
			}
		}
	}
}
