using System;
using System.ComponentModel;
using System.Windows;
using CrossGIS.Core.ViewModels;

namespace CrossGIS.WPF.Core
{
    public class WindowView : Window
    {
        private bool _hasBeenActivated;

        public WindowView()
        {
            Loaded += OnLoaded;
        }

        private IViewAware ViewAwareViewModel { get { return DataContext as IViewAware; } }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (ViewAwareViewModel != null)
                ViewAwareViewModel.OnViewLoaded();
        }

        protected override void OnActivated(EventArgs e)
        {
            if (ViewAwareViewModel != null)
                ViewAwareViewModel.OnActivate(!_hasBeenActivated);
            _hasBeenActivated = true;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            if (ViewAwareViewModel != null)
                ViewAwareViewModel.OnDeactivate(false);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewAwareViewModel != null)
            {
                if (!ViewAwareViewModel.CanClose())
                    e.Cancel = true;
            }
        }
    }
}
