using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CrossGIS.Core.Models;
using CrossGIS.Core.Services;
using CrossGIS.Core.Utils;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
#if WP8
using Windows.Phone.Speech.Recognition;
#endif

namespace CrossGIS.Core.ViewModels
{
    public class PlaceSearchViewModel : ViewModelBase
    {
        private readonly IViewModelLocator _viewModelLocator;

        private readonly ILocatorService _locatorService;
        private readonly IXamlResourceProvider _xamlResourceProvider;

        //For DesignTime Only!
        public PlaceSearchViewModel()
        {
            
        }

        [PreferredConstructor]
        public PlaceSearchViewModel(ILocatorService locatorService, IViewModelLocator viewModelLocator, IXamlResourceProvider xamlResourceProvider)
        {
            _locatorService = Guard.NotNull(locatorService, "locatorService");
            _viewModelLocator = Guard.NotNull(viewModelLocator, "viewModelLocator");
            _xamlResourceProvider = Guard.NotNull(xamlResourceProvider, "xamlResourceProvider");

            SearchCommand = new RelayCommand(Search, CanSearch);
#if WP8
            RecognizeSpeechCommand = new RelayCommand(RecognizeSpeech, CanRecognizeSpeech);
#endif
            GoToPlaceCommand = new RelayCommand<PlaceSearchResult>(GoToPlace, CanGoToPlace);
        }

        private MainViewModel MainViewModel { get { return _viewModelLocator.MainViewModel; } }
        private MapViewModel MapViewModel { get { return _viewModelLocator.MapViewModel; } }

        private Symbol SearchSymbol { get { return _xamlResourceProvider.Resolve<Symbol>("SearchSymbol"); } }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                Set(() => SearchText, ref _searchText, value);
                SearchCommand.Raise();
            }
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get { return _isSearching; }
            set { Set(() => IsSearching, ref _isSearching, value); }
        }

        private GraphicsLayerViewModel _placeSearchLayer;
        private GraphicsLayerViewModel PlaceSearchLayer
        {
            get
            {
                return _placeSearchLayer ?? (_placeSearchLayer = CreateGraphicsLayer());
            }
        }

        private ObservableCollection<Graphic> _searchResultGraphics;
        private ObservableCollection<Graphic> SearchResultGraphics
        {
            get { return _searchResultGraphics ?? (_searchResultGraphics = new ObservableCollection<Graphic>()); }
        }

        public bool HasSearchResults { get { return SearchResults.Any(); } }

        private ObservableCollection<PlaceSearchResult> _searchResults;
        public ObservableCollection<PlaceSearchResult> SearchResults
        {
            get
            {
                if (_searchResults == null)
                {
                    _searchResults = new ObservableCollection<PlaceSearchResult>();
                    _searchResults.CollectionChanged += SearchResultsOnCollectionChanged;
                }
                return _searchResults;
            }
        }

        private void SearchResultsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var placeSearch in e.NewItems.OfType<PlaceSearchResult>())
                    {
                        SearchResultGraphics.Add(placeSearch.Graphic);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var placeSearch in e.NewItems.OfType<PlaceSearchResult>())
                    {
                        SearchResultGraphics.Remove(placeSearch.Graphic);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    //TODO:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    SearchResultGraphics.Clear();
                    break;
            }
            RaisePropertyChanged(() => HasSearchResults);
        }

        #region GoToPlace Command
        public ICommand GoToPlaceCommand { get; private set; }

        private bool CanGoToPlace(PlaceSearchResult place)
        {
            return place != null;
        }

        private void GoToPlace(PlaceSearchResult place)
        {
            MapViewModel.ZoomToScale(place.Location.Extent.GetCenter(), Constants.DefaultFeatureZoomScale);
        } 
        #endregion

        #region Search
        public ICommand SearchCommand { get; private set; }

        private bool CanSearch()
        {
            return !string.IsNullOrEmpty(SearchText) && !IsSearching && MapViewModel.Center != null;
        }

        private void Search()
        {
            if(!CanSearch())
                return;

            SearchAsync();
        } 

        private async Task SearchAsync()
        {
            IsSearching = true;
            try
            {
                //Start with the Map's current center
                MapPoint searchCenter = MapViewModel.Center;

                //If Gps is active and has a valid position
                if (MainViewModel.GpsViewModel != null &&
                    MainViewModel.GpsViewModel.IsEnabled &&
                    MainViewModel.GpsViewModel.IsPositionValid)
                {
                    var gpsPosition = MainViewModel.GpsViewModel.Position;

                    //Use the GPS position as our search center only if it's within view
                    if (MapViewModel.ViewArea.Intersects(gpsPosition.Extent))
                    {
                        searchCenter = gpsPosition;
                    }
                }
                var locations =
                    await _locatorService.FindPlacesAsync(SearchText, searchCenter, MapViewModel.SpatialReference);

                SearchResults.Clear();

                foreach (var location in locations)
                {
                    SearchResults.Add(CreatePlaceSearchResult(location));
                }

                

                if (!MapViewModel.Layers.Contains(PlaceSearchLayer))
                    MapViewModel.Layers.Add(PlaceSearchLayer);

                PlaceSearchLayer.IsVisible = SearchResults.Any();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsSearching = false;
            }
        }
        #endregion

#if WP8
        #region Speech Support
        public ICommand RecognizeSpeechCommand { get; private set; }

        private bool CanRecognizeSpeech()
        {
            return true;
        }

        private void RecognizeSpeech()
        {
            if (!CanRecognizeSpeech())
                return;

            RecognizeAsync();
        } 

        private SpeechRecognizerUI _recognizer;

        private async Task RecognizeAsync()
        {
            if (_recognizer == null)
            {
                try
                {
                    _recognizer = new SpeechRecognizerUI();
                    _recognizer.Recognizer.Grammars.AddGrammarFromPredefinedType("free", SpeechPredefinedGrammar.WebSearch);
                    await _recognizer.Recognizer.PreloadGrammarsAsync();
                }
                catch (Exception)
                {
                    Dispose(ref _recognizer);
                    return;
                }
            }
            try
            {
                var result = await _recognizer.RecognizeWithUIAsync();
                if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
                {
                    SearchText = result.RecognitionResult.Text;
                    SearchCommand.TryExecute(null);
                }
            }
            catch (Exception)
            {
                Dispose(ref _recognizer);
            }
        }
        #endregion
#endif

        #region Helpers
        private static void Dispose<T>(ref T disposable) where T: class, IDisposable
        {
            if(disposable != null)
                disposable.Dispose();
            disposable = null;
        }

        private PlaceSearchResult CreatePlaceSearchResult(Location location)
        {
            var pSearch = PlaceSearchResult.FromLocation(location, location.Graphic);
            pSearch.GoToCommand = this.GoToPlaceCommand;
            pSearch.Symbol = SearchSymbol;
            return pSearch;
        }

        private GraphicsLayerViewModel CreateGraphicsLayer()
        {
            var gLayer = new GraphicsLayer()
            {
                ID = "PlaceSearchResults",
                Renderer = new SimpleRenderer() { Symbol = SearchSymbol }
            };
            return new GraphicsLayerViewModel(gLayer) { Graphics = SearchResultGraphics };
        } 
        #endregion
    }
}
