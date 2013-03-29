using System;
using System.Device.Location;
using System.Diagnostics;
using CrossGIS.Core.Utils;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Toolkit.DataSources;
using GalaSoft.MvvmLight.Ioc;

namespace CrossGIS.Core.ViewModels
{
    public enum AutoPanMode { None, AutoPan }

    public enum GpsState { Off, AutoPanOn, AutoPanOff }

    public class GpsLayerViewModel : LayerViewModel, IMapGestureHandler
    {
        private readonly IGeoPositionWatcher<GeoCoordinate> _locator;
 
        private bool _zoomToPosition;

        public GpsLayerViewModel(GpsLayer layer)
        {
            Layer = layer;
        }

        [PreferredConstructor]
        public GpsLayerViewModel(IGeoPositionWatcher<GeoCoordinate> locator)
        {
            _locator = Guard.NotNull(locator, "locator");

            Layer = new GpsLayer()
            {
                IsEnabled = false,
                GeoPositionWatcher = _locator
            };
        }

        private GpsLayer GpsLayer { get { return Layer as GpsLayer; } }

        private GpsState _state = GpsState.Off;
        public GpsState State
        {
            get { return _state; }
            private set { Set(() => State, ref _state, value); }
        }

        private AutoPanMode _autoPanMode = AutoPanMode.None;
        public AutoPanMode AutoPanMode
        {
            get { return _autoPanMode; }
            set
            {
                if (Set(() => AutoPanMode, ref _autoPanMode, value))
                {
                    TryZoomToPosition();
                    UpdateGpsState();
                }
            }
        }

#if WP8
        public bool AreLocationServicesAvailable
        {
            get
            {
                var status = new Windows.Devices.Geolocation.Geolocator().LocationStatus;
                return status != Windows.Devices.Geolocation.PositionStatus.Disabled &&
                       status != Windows.Devices.Geolocation.PositionStatus.NotAvailable;
            }
        }
#endif
        public bool IsEnabled
        {
            get { return GpsLayer.IsEnabled; }
            set
            {
                if (GpsLayer.IsEnabled != value)
                {
                    _zoomToPosition = true;
                    GpsLayer.IsEnabled = value;
                    GpsLayer.Visible = GpsLayer.IsEnabled;

                    TryZoomToPosition();
                    UpdateGpsState();
                }
            }
        }

        public MapPoint Position
        {
            get { return GpsLayer.Position; }
        }

        public bool IsPositionValid
        {
            get { return Position != null; }
        }

        protected override void OnMapViewModelChanged(MapViewModel oldMap, MapViewModel newMap)
        {
            if(oldMap != null)
                oldMap.UnregisterHandler(this);
            if(newMap != null)
                newMap.RegisterHandler(this);
        }

        protected override void OnLayerChanged(Layer oldLayer, Layer newLayer)
        {
            var oldGpsLayer = oldLayer as GpsLayer;
            if (oldGpsLayer != null)
            {
                oldGpsLayer.PositionChanged -= OnGpsPositionChanged;
            }
            var newGpsLayer = newLayer as GpsLayer;
            if (newGpsLayer != null)
            {
                newGpsLayer.PositionChanged += OnGpsPositionChanged;
            }
        }

        private void OnGpsPositionChanged(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("Gps Position changed Point=" + Position);
            RaisePropertyChanged(() => Position);

            if (AutoPanMode == AutoPanMode.AutoPan && Position != null)
            {
                if (_zoomToPosition)
                {
                    _zoomToPosition = false;
                    MapViewModel.ZoomToScale(Position, Constants.DefaultGpsZoomScale);
                }
                else
                    MapViewModel.PanTo(Position);
            }

        }

        private void UpdateGpsState()
        {
            if (IsEnabled)
            {
                State = AutoPanMode == AutoPanMode.AutoPan ? GpsState.AutoPanOn : GpsState.AutoPanOff;
            }
            else
                State = GpsState.Off;
        }

        private void TryZoomToPosition()
        {
            if (AutoPanMode == AutoPanMode.AutoPan &&
                        Position != null && IsEnabled)
                MapViewModel.ZoomToScale(Position, Constants.DefaultGpsZoomScale);
        }

        #region MapGesture Handling
        public bool OnMapGesture(Map map, Map.MapGestureEventArgs e)
        {
            if (AutoPanMode == AutoPanMode.AutoPan && IsAutoPanCancellingGesture(e.Gesture))
            {
                AutoPanMode = AutoPanMode.None;
            }
            return false;
        }

        private bool IsAutoPanCancellingGesture(GestureType gesture)
        {
            return gesture == GestureType.Drag ||
                gesture == GestureType.Flick ||
                gesture == GestureType.Pinch;
        } 
        #endregion
    }
}
