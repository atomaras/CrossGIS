using System.Threading.Tasks;
using ESRI.ArcGIS.Client.WebMap;

namespace CrossGIS.Core.Models
{
    public static class Portal
    {
        public static Task<GetMapCompletedEventArgs> GetMapAsync(string itemId)
        {
            var tcs = new TaskCompletionSource<GetMapCompletedEventArgs>();
            var webmap = new Document();
            webmap.GetMapCompleted += 
                (s, e) =>
                    {
                        if (e.Error != null)
                            tcs.TrySetException(e.Error);
                        else
                            tcs.TrySetResult(e);
                    };
            webmap.GetMapAsync(itemId);
            return tcs.Task;
        }
    }
}
