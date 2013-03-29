using CrossGIS.Core.Design;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace CrossGIS.Core.ViewModels
{
    public class ViewModelLocator : IViewModelLocator
    {
        public static ISimpleIoc IoC;

        public MainViewModel MainViewModel
        {
            get { return IoC.GetInstance<MainViewModel>(); }
        }
        public MapViewModel MapViewModel
        {
            get { return IoC.GetInstance<MapViewModel>(); }
        }
        public PlaceSearchViewModel PlaceSearchViewModel
        {
            get { return Get<PlaceSearchViewModel>(new DesignPlaceSeachViewModel()); }
        }

        private T Get<T>(T design)
        {
            if (ViewModelBase.IsInDesignModeStatic)
                return design;
            return IoC.GetInstance<T>();
        }
    }
}
