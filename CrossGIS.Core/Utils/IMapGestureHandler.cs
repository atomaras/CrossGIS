using ESRI.ArcGIS.Client;

namespace CrossGIS.Core.Utils
{
    public interface IMapGestureHandler
    {
        bool OnMapGesture(Map map, Map.MapGestureEventArgs e);
    }
}
