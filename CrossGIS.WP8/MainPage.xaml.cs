using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CrossGIS.Core.Controls;

namespace CrossGIS.WP8
{
    public partial class MainPage : PhoneApplicationPageView
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnVisualStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            MainAppBar.IsVisible = e.NewState.Name == "Hidden";
        }

        private void OnSearchAccentBarMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CurrentVisualState.Name == "Collapsed")
                VisualStateManager.GoToState(this, "Expanded", false);
            else if (CurrentVisualState.Name == "Expanded")
                VisualStateManager.GoToState(this, "Collapsed", false);
        }

        private void SearchTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            MainAppBar.IsVisible = false;
        }

        private void SearchTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            MainAppBar.IsVisible = true;
        }

        private void OnHasSearchResultsChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue)
            {
                if(CurrentVisualState.Name == "Hidden")
                    VisualStateManager.GoToState(this, "Collapsed", false);
            }
            else
                VisualStateManager.GoToState(this, "Hidden", false);
        }

        private VisualState CurrentVisualState
        {
            get
            {
                return
                    VisualStateManager.GetVisualStateGroups(LayoutRoot).OfType<VisualStateGroup>().First().CurrentState;
            }
        }
    }
}