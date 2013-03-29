using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;

namespace CrossGIS.Core.Services
{
    public interface ILocatorService
    {
        double DefaultSearchRadius { get; set; }
        int MaxSearchResults { get; set; }

        String ServiceUri { get; set; }

        Task<IEnumerable<Location>> FindPlacesAsync(string keyword, MapPoint center, SpatialReference outSRef);
    }
}
