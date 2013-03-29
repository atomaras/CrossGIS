using System;
using System.Device.Location;
using CrossGIS.Core.Services;
using CrossGIS.Core.Utils;
using CrossGIS.Core.ViewModels;
using ESRI.ArcGIS.Client;
using GalaSoft.MvvmLight.Ioc;

namespace CrossGIS.Core
{
    public class Bootstrapper
    {
        public static void Initialize()
        {
            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ViewModelLocator.IoC = SimpleIoc.Default;

            SimpleIoc.Default.Register<IViewModelLocator, ViewModelLocator>();

            //Security
#if !WPF
            //See ManifestAppInfo.DefaultRefererHeader for more info!
            IdentityManager.Current.TokenGenerationReferer = ManifestAppInfo.DefaultRefererHeader;
#endif

#if WP8
            //See ManifestAppInfo.DefaultRefererHeader for more info!
            var refererInjector = new RefererInjectionWebRequestCreator(ManifestAppInfo.DefaultRefererHeader);
            System.Net.WebRequest.RegisterPrefix("http://", refererInjector);
            System.Net.WebRequest.RegisterPrefix("https://", refererInjector);
#endif

            //Services
#if WINDOWS_PHONE
            SimpleIoc.Default.Register<IStateService, PhoneStateService>();
#endif
#if WPF
            SimpleIoc.Default.Register<IStateService, InMemoryStateService>();
#endif

            SimpleIoc.Default.Register<IGeoPositionWatcher<GeoCoordinate>>(
                () =>
                    {
                        var locator = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
                                          {
                                              MovementThreshold = 20
                                          };
                        return locator;
                    });

            SimpleIoc.Default.Register<GpsLayerViewModel>();
            SimpleIoc.Default.Register<IGpsLayerFactory>(() => new GpsLayerFactory());

            SimpleIoc.Default.Register<IXamlResourceProvider, AppXamlResourceProvider>();

            SimpleIoc.Default.Register<ILocatorService>(
                () => new LocatorService()
                          {
                              ServiceUri = Constants.LocatorServiceUrl,
                              DefaultSearchRadius = 5000, //meters
                              MaxSearchResults = 50
                          });

            //ViewModels
            SimpleIoc.Default.Register<MapViewModel>();
            SimpleIoc.Default.Register<PlaceSearchViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();

        }

        private class GpsLayerFactory : IGpsLayerFactory
        {
            public GpsLayerViewModel Create()
            {
                return SimpleIoc.Default.GetInstance<GpsLayerViewModel>(Guid.NewGuid().ToString());
            }
        }
    }
}
