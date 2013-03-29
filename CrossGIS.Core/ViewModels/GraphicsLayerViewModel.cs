using System.Collections.Generic;
using ESRI.ArcGIS.Client;

namespace CrossGIS.Core.ViewModels
{
    public class GraphicsLayerViewModel : LayerViewModel
    {
        public GraphicsLayerViewModel()
        {
        }
        public GraphicsLayerViewModel(GraphicsLayer graphicsLayer)
        {
            Layer = graphicsLayer;
        }

        private GraphicsLayer GraphicsLayer { get { return Layer as GraphicsLayer; } }

        public IEnumerable<Graphic> Graphics
        {
            get { return GraphicsLayer.GraphicsSource; }
            set { GraphicsLayer.GraphicsSource = value; }
        }
    }
}
