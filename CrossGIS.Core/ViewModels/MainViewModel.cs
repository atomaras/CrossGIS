using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CrossGIS.Core.Models;
using CrossGIS.Core.Services;
using CrossGIS.Core.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CrossGIS.Core.ViewModels
{
    public class MainViewModel : ViewModelBase, IViewAware
    {
        private readonly IViewModelLocator _viewModelLocator;

        public MainViewModel(IViewModelLocator viewModelLocator,
            IStateService stateService,
            IGpsLayerFactory gpsLayerFactory)
        {
            State = Guard.NotNull(stateService, "stateService");

            _viewModelLocator = Guard.NotNull(viewModelLocator, "viewModelLocator");
            GpsLayerFactory = Guard.NotNull(gpsLayerFactory, "gpsLayerFactory");

            ToggleGpsCommand = new RelayCommand(ToggleGps, CanToggleGps);
        }

        private IGpsLayerFactory GpsLayerFactory { get; set; }

        public IStateService State { get; private set; }
        public PlaceSearchViewModel PlaceSearch { get { return _viewModelLocator.PlaceSearchViewModel; } }
        public MapViewModel MapViewModel { get { return _viewModelLocator.MapViewModel; } }

        #region GPS
        private GpsLayerViewModel _gpsViewModel;
        public GpsLayerViewModel GpsViewModel
        {
            get { return _gpsViewModel; }
            private set
            {
                if (_gpsViewModel != null)
                {
                    MapViewModel.Layers.Remove(_gpsViewModel);
                }
                _gpsViewModel = value;
                if (_gpsViewModel != null)
                {
                    MapViewModel.Layers.Add(_gpsViewModel);
                }
                RaisePropertyChanged(() => GpsViewModel);
            }
        }

        public ICommand ToggleGpsCommand { get; private set; }

        private void ToggleGps()
        {
            if (GpsViewModel == null)
                GpsViewModel = GpsLayerFactory.Create();

#if WP8
            if (!GpsViewModel.AreLocationServicesAvailable)
            {
                if (MessageBox.Show(
                    "Location Services are required to perform this operation.\n" +
                    "You can enable Location Services through the Location Settings page.\n" +
                    "Open Location Settings?",
                    "Location Required", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
                }
                return;
            }
#endif
            if (GpsViewModel.IsEnabled)
                GpsViewModel.IsEnabled = false;
            else
            {
                GpsViewModel.IsEnabled = true;
                GpsViewModel.AutoPanMode = AutoPanMode.AutoPan;
            }
        }
        private bool CanToggleGps()
        {
            return MapViewModel != null && MapViewModel.Map != null;
        }
        #endregion

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(() => IsLoading, ref _isLoading, value); }
        }

        #region IViewAware
        void IViewAware.OnViewLoaded()
        {
            if (MapViewModel.Map == null)
            {
                OpenMapAsync();
            }
        }

        private async Task OpenMapAsync()
        {
            IsLoading = true;
            try
            {
                var result = await Portal.GetMapAsync(Constants.DefaultWebMapId);
                MapViewModel.Map = result.Map;
                ToggleGpsCommand.Raise();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool _reenableGps;

        void IViewAware.OnActivate(bool isNew)
        {
            if (!isNew && _reenableGps)
            {
                if (GpsViewModel != null && !GpsViewModel.IsEnabled)
                    GpsViewModel.IsEnabled = true;
                _reenableGps = false;
            }
        }
        void IViewAware.OnDeactivate(bool close)
        {
            if (GpsViewModel != null && GpsViewModel.IsEnabled)
            {
                _reenableGps = true;
                GpsViewModel.IsEnabled = false;
            }
                
        }

        bool IViewAware.CanClose()
        {
#if WINDOWS_PHONE
            if (PlaceSearch.HasSearchResults)
            {
                if (MessageBox.Show("Are your sure you want to dismiss the search results?", 
                    "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    PlaceSearch.SearchResults.Clear();
                }
                return false;
            }
            return true;
#endif
#if WPF
            return MessageBox.Show("Are you sure?", "Exit Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
#endif
        } 
        #endregion

        

        
    }
}
