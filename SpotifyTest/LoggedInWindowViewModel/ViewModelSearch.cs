using SpotifyControllerAPI.Model;
using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SpotifyController.LoggedInWindowViewModel
{
    public class ViewModelSearch : TabItemBaseViewModel, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Member + NotifyPropertyChanged Method

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
        
        public ViewModelSearch(ViewModelLoggedIn parent) : base(parent)
        {
            _visibilitySetters = new Dictionary<VisibilityConfigs, Action<Visibility>>()
            {
                { VisibilityConfigs.SearchInProgress, (v) => { SimpleSearchProgressVisibility = v; } },
                { VisibilityConfigs.SimpleResult, (v) => { SimpleSearchResultVisibility = v; } },
                { VisibilityConfigs.AdvancedResult, (v) => { AdvancedSearchResultVisibility = v; } },
            };

            SetVisibilities(VisibilityConfigs.None);

            SearchTracks = true;

            AdvSearchMaxPlaylists = 100;

            _simpleResults = new List<SearchResult>();
        }

        PlaylistAggregationSearch _search;


        private string _searchText;

        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                NotifyPropertyChanged("SearchText");
            }
        }

        private bool _searchTracks;

        public bool SearchTracks
        {
            get
            {
                return _searchTracks;
            }
            set
            {
                _searchTracks = value;
                NotifyPropertyChanged("SearchTracks");
            }
        }

        public GridLength SimpleTracksVisibility => SearchTracks ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

        private bool _searchPlaylists;

        public bool SearchPlaylists
        {
            get
            {
                return _searchPlaylists;
            }
            set
            {
                _searchPlaylists = value;
                NotifyPropertyChanged("SearchPlaylists");
            }
        }

        public GridLength SimplePlaylistsVisibility => SearchPlaylists ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

        private bool _searchArtists;

        public bool SearchArtists
        {
            get
            {
                return _searchArtists;
            }
            set
            {
                _searchArtists = value;
                NotifyPropertyChanged("SearchArtists");
            }
        }
        public GridLength SimpleArtistsVisibility => SearchArtists ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

        private bool _searchAlbums;

        public bool SearchAlbums
        {
            get
            {
                return _searchAlbums;
            }
            set
            {
                _searchAlbums = value;
                NotifyPropertyChanged("SearchAlbums");
            }
        }
        public GridLength SimpleAlbumsVisibility => SearchAlbums ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

        private Visibility _simpleSearchProgressVisibility;

        public Visibility SimpleSearchProgressVisibility
        {
            get
            {
                return _simpleSearchProgressVisibility;
            }
            set
            {
                _simpleSearchProgressVisibility = value;
                NotifyPropertyChanged("SimpleSearchProgressVisibility");
            }
        }

        private Visibility _simpleSearchResultVisibility;

        public Visibility SimpleSearchResultVisibility
        {
            get
            {
                return _simpleSearchResultVisibility;
            }
            set
            {
                _simpleSearchResultVisibility = value;
                NotifyPropertyChanged("SimpleSearchResultVisibility");
            }
        }

        private Visibility _advancedSearchResultVisibility;

        public Visibility AdvancedSearchResultVisibility
        {
            get
            {
                return _advancedSearchResultVisibility;
            }
            set
            {
                _advancedSearchResultVisibility = value;
                NotifyPropertyChanged("AdvancedSearchResultVisibility");
            }
        }

        private string _searchProgressMessage;

        public string SearchProgressMessage
        {
            get
            {
                return _searchProgressMessage;
            }
            set
            {
                _searchProgressMessage = value;
                NotifyPropertyChanged("SearchProgressMessage");
            }
        }

        private List<SearchResult> _simpleResults;

        private SearchResult _currentSimpleResult;

        public SearchResult CurrentSimpleResult
        {
            get
            {
                return _currentSimpleResult;
            }
            set
            {
                _currentSimpleResult = value;
                NotifyPropertyChanged("CurrentSimpleResult");

                if (_currentSimpleResult != null)
                {
                    SimpleAlbums = _currentSimpleResult.Albums;
                    SimpleArtists = _currentSimpleResult.Artists;
                    SimplePlaylists = _currentSimpleResult.Playlists;
                    SimpleTracks = _currentSimpleResult.Tracks;
                }
            }
        }

        private PagingWrapper<Track> _simpleTracks;

        public PagingWrapper<Track> SimpleTracks
        {
            get
            {
                return _simpleTracks;
            }
            set
            {
                _simpleTracks = value;
                NotifyPropertyChanged("SimpleTracks");
            }
        }

        private PagingWrapper<Artist> _simpleArtists;

        public PagingWrapper<Artist> SimpleArtists
        {
            get
            {
                return _simpleArtists;
            }
            set
            {
                _simpleArtists = value;
                NotifyPropertyChanged("SimpleArtists");
            }
        }

        private PagingWrapper<Album> _simpleAlbums;

        public PagingWrapper<Album> SimpleAlbums
        {
            get
            {
                return _simpleAlbums;
            }
            set
            {
                _simpleAlbums = value;
                NotifyPropertyChanged("SimpleAlbums");
            }
        }

        private PagingWrapper<Playlist> _simplePlaylists;

        public PagingWrapper<Playlist> SimplePlaylists
        {
            get
            {
                return _simplePlaylists;
            }
            set
            {
                _simplePlaylists = value;
                NotifyPropertyChanged("SimplePlaylists");
            }
        }

        private int _advSearchMaxPlaylists;

        public int AdvSearchMaxPlaylists
        {
            get
            {
                return _advSearchMaxPlaylists;
            }
            set
            {
                _advSearchMaxPlaylists = value;
                NotifyPropertyChanged("AdvSearchMaxPlaylists");
            }
        }


        public override async void Update()
        {
            _parent.BlockUI();

            _parent.UnblockUI();
        }

        private enum VisibilityConfigs { None, SearchInProgress, SimpleResult, AdvancedResult }

        Dictionary<VisibilityConfigs, Action<Visibility>> _visibilitySetters;

        private void SetVisibilities(VisibilityConfigs config)
        {
            foreach (VisibilityConfigs key in _visibilitySetters.Keys)
            {
                if (key == config)
                {
                    _visibilitySetters[key](Visibility.Visible);
                }
                else
                {
                    _visibilitySetters[key](Visibility.Collapsed);
                }
            }
        }

        public void StartAdvancedSearch()
        {
            _parent.BlockUI();

            if (string.IsNullOrEmpty(SearchText) || AdvSearchMaxPlaylists == 0)
                return;

            SetVisibilities(VisibilityConfigs.SearchInProgress);
            
            SearchProgressMessage = "Start loading playlists";

            _search = new PlaylistAggregationSearch();

            _advancedSearchRunning = true;

            _search.Run(SearchText, AdvSearchMaxPlaylists, _parent.LoggedInUser, CompleteAdvancedSearch, Dispatcher.CurrentDispatcher);

            UpdateAdvancedSearchProcess();
        }

        private bool _advancedSearchRunning;

        private async void UpdateAdvancedSearchProcess()
        {
            while (_advancedSearchRunning)
            {
                int totalLoaded = _search.Results.Tracks.Values.Count == 0 ? 0 : _search.Results.Tracks.Values.Select(x => x.Count).Aggregate((y, z) => y + z);

                SearchProgressMessage = $"Searching and processing playlists. {_search.CurrentParallelTasks} parallel tasks running.\n" +
                                        $"Playlists loaded:     {_search.Results.Playlists.Count} \n" +
                                        $"Unique tracks loaded: {_search.Results.Tracks.Count}\n" +
                                        $"Total tracks loaded:  {totalLoaded}";

                await Task.Run(() => System.Threading.Thread.Sleep(800));
            }
        }

        private void CompleteAdvancedSearch()
        {
            PlaylistAggregationSearchResult result = _search.Results;

            result.GetPage(0, 10);

            SetVisibilities(VisibilityConfigs.AdvancedResult);

            _parent.UnblockUI();
        }
        
        public async void StartSimpleSearch()
        {
            if (string.IsNullOrEmpty(SearchText))
                return;

            SetVisibilities(VisibilityConfigs.SearchInProgress);

            SearchProgressMessage = "Allocating search scope...";

            List<string> searchScope = new List<string>();

            if (SearchTracks)
                searchScope.Add("track");

            if (SearchPlaylists)
                searchScope.Add("playlist");

            if (SearchArtists)
                searchScope.Add("artist");

            if (SearchAlbums)
                searchScope.Add("album");

            if (searchScope.Count > 0)
            {
                int itemsAmount = (12 / searchScope.Count) + (int)(4D / (double)searchScope.Count)  ;

                SearchConfiguration config = new SearchConfiguration()
                {
                    Amount = itemsAmount,
                    Offset = 0,
                    Scope = searchScope,
                    SearchText = SearchText
                };

                if (_simpleResults != null && _simpleResults.FirstOrDefault(x => x.Config.CompareSearch(config)) is SearchResult sr)
                {
                    CurrentSimpleResult = sr;
                }
                else
                {
                    SearchProgressMessage = "Waiting for a response from the spotify servers...";

                    SearchResult result = await DataLoader.GetInstance().Search(config);

                    _simpleResults.Add(result);

                    CurrentSimpleResult = result;
                }
            }

            SetVisibilities(VisibilityConfigs.SimpleResult);
            NotifyPropertyChanged("SimpleTracksVisibility");
            NotifyPropertyChanged("SimplePlaylistsVisibility");
            NotifyPropertyChanged("SimpleArtistsVisibility");
            NotifyPropertyChanged("SimpleAlbumsVisibility");
        }
        
    }
}
