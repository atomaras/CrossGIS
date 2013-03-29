namespace CrossGIS.Core.ViewModels
{
    public interface IViewModelLocator
    {
        MainViewModel MainViewModel { get; }
        MapViewModel MapViewModel { get; }
        PlaceSearchViewModel PlaceSearchViewModel { get; }
    }
}