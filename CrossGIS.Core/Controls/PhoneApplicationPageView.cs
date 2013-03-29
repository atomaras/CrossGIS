using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using CrossGIS.Core.ViewModels;
using Microsoft.Phone.Controls;

namespace CrossGIS.Core.Controls
{
    public class PhoneApplicationPageView : PhoneApplicationPage
    {
        public PhoneApplicationPageView()
        {
            this.Loaded += OnLoaded;
        }

        private IViewAware ViewAwareViewModel { get { return DataContext as IViewAware; } }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if(ViewAwareViewModel != null)
                ViewAwareViewModel.OnViewLoaded();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (ViewAwareViewModel != null)
            {
                ViewAwareViewModel.OnDeactivate(e.NavigationMode == NavigationMode.Back);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ViewAwareViewModel != null)
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    ViewAwareViewModel.OnActivate(true);
                }
                else if (e.NavigationMode == NavigationMode.Back)
                {
                    ViewAwareViewModel.OnActivate(false);
                }
            }
                
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (ViewAwareViewModel != null)
            {
                if (!ViewAwareViewModel.CanClose())
                    e.Cancel = true;
            }
        }
    }
}
