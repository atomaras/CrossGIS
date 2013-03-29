using System.Collections.Generic;
using CrossGIS.Core.Models;
using CrossGIS.Core.ViewModels;
using ESRI.ArcGIS.Client;

namespace CrossGIS.Core.Design
{
    public class DesignPlaceSeachViewModel : PlaceSearchViewModel
    {
        public DesignPlaceSeachViewModel()
        {
            foreach (var placeSearchResult in this.CreateSearchResults(10))
            {
                SearchResults.Add(placeSearchResult);
            }
        }

        private IEnumerable<PlaceSearchResult> CreateSearchResults(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var pSearch = PlaceSearchResult.FromLocation(null, new Graphic());
                pSearch.Name = "Place " + i;
                pSearch.Distance = i*1.1;
                pSearch.Type = "Redlands";
                yield return pSearch;
            }
        }
    }
}
