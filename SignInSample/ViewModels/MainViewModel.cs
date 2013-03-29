using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.WebMap;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SignInSample.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string WebMapId = "9a9dbe1e7bdd450fb9628a67f4323c73"; //Secure
        private const string Referer = "f";

        public MainViewModel()
        {
            LoadMapCommand = new RelayCommand(LoadMap);

            //Force all WebRequests to send a default Referer
            var refererInjectingWebRequestCreator = new RefererInjectionWebRequestCreator(Referer);
            WebRequest.RegisterPrefix("http://", refererInjectingWebRequestCreator);
            WebRequest.RegisterPrefix("https://", refererInjectingWebRequestCreator);

            IdentityManager.Current.TokenGenerationReferer = Referer;
        }

        public Map Map { get; set; }

        public ICommand LoadMapCommand { get; set; }


        private void LoadMap()
        {
            Document webmap = new Document();
            webmap.GetMapCompleted +=
                (sender, args) =>
                {
                    if (args.Error == null)
                    {
                        Map = args.Map;
                        RaisePropertyChanged(() => Map);
                    }
                    else
                    {
                        MessageBox.Show("Unable to open WebMap");
                    }
                };
            webmap.GetMapAsync(WebMapId);
        }

    }
}
