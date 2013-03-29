using System;
using System.Windows.Input;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using GalaSoft.MvvmLight;

namespace CrossGIS.Core.Models
{
    public class PlaceSearchResult : ObservableObject
    {
        public PlaceSearchResult()
        {
            
        }
        public static PlaceSearchResult FromLocation(Location location, Graphic locationGraphic)
        {
            return new PlaceSearchResult
                       {
                           Location = location,
                           Type = GetLocationType(locationGraphic),
                           Name = location != null ? location.Name : null,
                           Graphic = locationGraphic,
                           Distance = GetDistance(locationGraphic)
                       };
        }

        public Location Location { get; private set; }

        private String _name;
        public String Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }

        private String _type;

        public String Type
        {
            get { return _type; }
            set { Set(() => Type, ref _type, value); }
        }

        public double? Distance { get; set; }
        public ICommand GoToCommand { get; set; }
        public Graphic Graphic { get; set; }
        public Symbol Symbol { get; set; }

        private static double GetDistance(Graphic locationGraphic)
        {
            if (locationGraphic.Attributes.ContainsKey("Distance"))
            {
                double meters = Convert.ToDouble(locationGraphic.Attributes["Distance"]);
                return meters / 1000; //km
            }
            return 0;
        }

        private static String GetLocationType(Graphic locationGraphic)
        {
            var attributes = locationGraphic.Attributes;
            if ("POI".Equals(attributes["Addr_type"]))
            {
                var locatorName = attributes["Loc_name"] as String ?? String.Empty;
                if (locatorName.EndsWith("POI2"))
                    return attributes["City"] as String;
            }
            return null;// attributes["Type"] as String;
        }
    }
}
