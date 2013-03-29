using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CrossGIS.Core.Utils;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using GalaSoft.MvvmLight;
using System.Linq;

namespace CrossGIS.Core.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        //Used to be able to throttle Panning/Zooming when many requests occur
        private bool _isExtentChanging;

        private Map _map;
        public Map Map
        {
            get { return _map; }
            set
            {
                if (_map != null)
                {
                    _map.MapGesture -= OnMapGesture;
                    _map.ExtentChanging -= MapOnExtentChanging;
                    _map.ExtentChanged -= MapOnExtentChanged;

                    if (Layers != null)
                    {
                        foreach (var l in Layers.ToList())
                        {
                            Layers.Remove(l);
                        }
                        Layers = null;
                    }
                }
                _map = value;

                if (_map != null)
                {
                    var layerVMs = _map.Layers.Select(l => LayerViewModel.FromLayer(l));
                    Layers = new ObservableCollection<LayerViewModel>(layerVMs);

                    _map.MapGesture += OnMapGesture;
                    _map.ExtentChanging += MapOnExtentChanging;
                    _map.ExtentChanged += MapOnExtentChanged;
                }
                RaisePropertyChanged(() => Map);
            }
        }

        #region Map Extent Handlers
        private void MapOnExtentChanging(object sender, ExtentEventArgs e)
        {
            _isExtentChanging = true;
        }
        private void MapOnExtentChanged(object sender, ExtentEventArgs e)
        {
            _isExtentChanging = false;
        } 
        #endregion

        private ObservableCollection<LayerViewModel> _layers;
        public ObservableCollection<LayerViewModel> Layers
        {
            get { return _layers; }
            set
            {
                if (_layers != null)
                {
                    _layers.CollectionChanged -= LayersOnCollectionChanged;
                }
                _layers = value;
                if (_layers != null)
                {
                    _layers.CollectionChanged += LayersOnCollectionChanged;
                }
            }
        }

        #region Properties
        public SpatialReference SpatialReference
        {
            get { return Map != null ? Map.SpatialReference : null; }
        }

        public MapPoint Center
        {
            get { return Map != null && Map.Extent != null ? Map.Extent.GetCenter() : null; }
        }

        public Envelope ViewArea
        {
            get { return Map != null ? Map.Extent : null; }
            private set { Map.Extent = value; }
        } 
        #endregion

        private void LayersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var layerVM in e.NewItems.OfType<LayerViewModel>())
                    {
                        layerVM.MapViewModel = this;
                        if (!Map.Layers.Contains(layerVM.Layer))
                            Map.Layers.Add(layerVM.Layer);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var layerVM in e.OldItems.OfType<LayerViewModel>())
                    {
                        layerVM.MapViewModel = null;
                        if (Map.Layers.Contains(layerVM.Layer))
                            Map.Layers.Remove(layerVM.Layer);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    //TODO:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    //TODO:
                    break;
            }
        }

        #region PanTo/ZoomTo
        public void PanTo(Geometry geometry)
        {
            if (Map == null)
                return;

            Throttle(() => Map.PanTo(geometry));
        }

        public void ZoomTo(Geometry geometry)
        {
            if (Map == null)
                return;

            Throttle(() => Map.ZoomTo(geometry));
        }

        public void ZoomToScale(MapPoint point, double scale)
        {
            if (Map == null)
                return;

            var zoomExtent = GetZoomExtent(point, GetResolutionFromScale(scale));
            Throttle(() => Map.ZoomTo(zoomExtent));

        }

        private void Throttle(Action action)
        {
            if (!_isExtentChanging)
                action();
        } 
        #endregion

        #region Helpers
        private Envelope GetZoomExtent(MapPoint point, double resolution)
        {
            double x1 = point.X - resolution*Map.ActualWidth*.5;
            double y1 = point.Y - resolution*Map.ActualHeight*.5;
            double x2 = point.X + resolution*Map.ActualWidth*.5;
            double y2 = point.Y + resolution*Map.ActualHeight*.5;
            return new Envelope(x1, y1, x2, y2) { SpatialReference = point.SpatialReference };
        }

        private double GetResolutionFromScale(double scale)
        {
            return Map.Resolution * (scale / Map.Scale);
        } 
        #endregion

        #region Map Interaction

        private readonly IList<IMapGestureHandler> _mapGestureHandlers = new List<IMapGestureHandler>();

        public void RegisterHandler(IMapGestureHandler handler)
        {
            _mapGestureHandlers.Add(handler);
        }
        public void UnregisterHandler(IMapGestureHandler handler)
        {
            _mapGestureHandlers.Remove(handler);
        }

        private void OnMapGesture(object sender, Map.MapGestureEventArgs e)
        {
            var handlers = _mapGestureHandlers.Reverse().ToList().GetEnumerator();

            while (handlers.MoveNext() && !handlers.Current.OnMapGesture(sender as Map, e))
            {
            }
        }

        #endregion
    }
}
