using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Toolkit.DataSources;
using GalaSoft.MvvmLight;

namespace CrossGIS.Core.ViewModels
{
    public class LayerViewModel : ObservableObject
    {
        private Layer _layer;
        public Layer Layer
        {
            get { return _layer; }
            set
            {
                var oldLayer = _layer;
                if (oldLayer != value)
                {
                    _layer = value;
                    OnLayerChanged(oldLayer, _layer);
                }
            }
        }

        protected MapViewModel _mapViewModel;
        public virtual MapViewModel MapViewModel
        {
            get { return _mapViewModel; }
            set 
            { 
                var oldMap = _mapViewModel;
                if (oldMap != value)
                {
                    _mapViewModel = value;
                    OnMapViewModelChanged(oldMap, _mapViewModel);
                }
            }
        }
        public bool IsVisible
        {
            get { return Layer.Visible; }
            set
            {
                if (Layer.Visible != value)
                {
                    Layer.Visible = value;
                    RaisePropertyChanged(() => IsVisible);
                }
            }
        }
        protected virtual void OnMapViewModelChanged(MapViewModel oldMap, MapViewModel newMap)
        {
            
        }
        protected virtual void OnLayerChanged(Layer oldLayer, Layer newLayer)
        {

        }

        public static LayerViewModel FromLayer(Layer layer)
        {
            if (layer == null)
                return null;
            if(layer is GpsLayer)
                return new GpsLayerViewModel((GpsLayer)layer);
            if (layer.GetType() == typeof (GraphicsLayer))
                return new GraphicsLayerViewModel((GraphicsLayer) layer);
            return new LayerViewModel() { Layer = layer };
        }
    }
}
