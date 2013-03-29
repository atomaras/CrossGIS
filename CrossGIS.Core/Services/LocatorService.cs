using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;

namespace CrossGIS.Core.Services
{
    public class LocatorService : ILocatorService
    {
        public double DefaultSearchRadius { get; set; }
        public int MaxSearchResults { get; set; }
        public string ServiceUri { get; set; }

        public Task<IEnumerable<Location>> FindPlacesAsync(string keyword, MapPoint center, SpatialReference outSRef)
        {
            Locator locator = new Locator(ServiceUri);
            locator.DisableClientCaching = true;
            locator.AutoNormalize = true;

            var p = new LocatorFindParameters();
            if (center != null)
            {
                p.Location = center;
                p.Distance = DefaultSearchRadius;
            }
            p.OutSpatialReference = outSRef;
            p.MaxLocations = MaxSearchResults;
            p.Text = keyword;
            p.OutFields.AddRange(new[] { "Addr_type", "Type", "Distance", "City", "Loc_name" });

            var tcs = new TaskCompletionSource<IEnumerable<Location>>();

            locator.FindCompleted += (_, e) =>
            {
                var results = e.Result.Locations ?? Enumerable.Empty<Location>();
                foreach (var location in results)
                {
                    location.Extent.SpatialReference =
                        location.Graphic.Geometry.SpatialReference = outSRef;
                }
                tcs.TrySetResult(results);
            };
            locator.Failed += (_, e) => tcs.TrySetException(e.Error);
            locator.FindAsync(p);
            return tcs.Task;
        }
    }
}
