using RandomUtilities.Queue;
using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SpotifyControllerAPI.Model
{
    public class PlaylistAggregationSearch
    {
        private bool _started = false;

        private bool _searchLoadStartCompleted = false;

        private string _searchText;
        private int _maxCount;
        private User _user;
        private Action _callback;
        private Dispatcher _uiDispatcher;

        private long _maxParallelTask = long.MaxValue;
        private int _runningGetSearchPageTasks;
        private int _runningGetTrackTasks;
        private int _runningSortingTasks;

        public int CurrentParallelTasks => _runningGetSearchPageTasks + _runningGetTrackTasks + _runningSortingTasks;

        private PlaylistAggregationSearchResult _result;

        public PlaylistAggregationSearchResult Results => _result;

        

        public void Run(string searchText, int maxCount, User user, Action callback, Dispatcher uiDispatcher)
        {
            if (_started)
                throw new InvalidOperationException("Search already running");

            _started = true;

            _searchText = searchText;
            _maxCount = maxCount;
            _user = user;
            _callback = callback;
            _uiDispatcher = uiDispatcher;

            //_playlists = new FILO<Playlist>();
            //_trackChucks = new FILO<List<Track>>();
            _result = new PlaylistAggregationSearchResult();

            Task.Run(() => RunGetPlaylistsLoop());
        }

        private void RunGetPlaylistsLoop()
        {
            Task.Run(async () =>
            {
                SearchConfiguration config = new SearchConfiguration()
                {
                    Offset = 0,
                    Amount = 1,
                    Scope = new List<string>() { "playlist" },
                    SearchText = _searchText
                };

                DataLoader dataLoader = DataLoader.GetInstance();

                SearchResult firstPage = await dataLoader.Search(config, true);

                int itemsToBeLoaded = RandomUtilities.MathUtil.Util.TakeSmaller(_maxCount, firstPage.Playlists.Total);

                int pagesToBeLoaded = itemsToBeLoaded / 50 + (itemsToBeLoaded % 50 > 0 ? 1 : 0);


                Parallel.For(0, pagesToBeLoaded, async(i) =>
                {
                    _runningGetSearchPageTasks++;
                    await GetAndProcessSearchPage(i * 50);
                });

                //for (int i = 0; i < itemsToBeLoaded; i += 50)
                //{
                //    while (CurrentParallelTasks >= _maxParallelTask)
                //        Thread.Sleep(5);

                //    _runningGetSearchPageTasks++;
                //    await GetAndProcessSearchPage(i);
                //}

                Thread.Sleep(50);

                _searchLoadStartCompleted = true;

                if (_searchLoadStartCompleted && CurrentParallelTasks - _runningSortingTasks == 0)
                {
                    while (_runningSortingTasks > 0)
                        Thread.Sleep(5);
                    _callback();
                }
            });
        }

        private async Task GetAndProcessSearchPage(int offset)
        {

            SearchConfiguration config = new SearchConfiguration()
            {
                Offset = offset,
                Amount = 50,
                Scope = new List<string>() { "playlist" },
                SearchText = _searchText
            };

            PagingWrapper<Playlist> page = (await DataLoader.GetInstance().Search(config, true)).Playlists;

            //Parallel.ForEach(page.Items, async(playlist) =>
            //{
            //    _runningGetTrackTasks++;
            //    await GetAndProcessTracksFromPlaylist(playlist);
            //});


            foreach (Playlist p in page.Items)
            {
                while (CurrentParallelTasks >= _maxParallelTask)
                    Thread.Sleep(5);

                _runningGetTrackTasks++;
                await GetAndProcessTracksFromPlaylist(p);
            }

            _runningGetSearchPageTasks--;

            if (_searchLoadStartCompleted && CurrentParallelTasks - _runningSortingTasks == 0)
            {
                while (_runningSortingTasks > 0)
                    Thread.Sleep(5);
                _callback();
            }
        }

        private async Task GetAndProcessTracksFromPlaylist(Playlist p)
        {
            if (!_result.Playlists.TryAdd(p.Id, p))
            {
            }

            List<Track> tracks = new List<Track>();

            if (p.Tracks.Items != null && p.Tracks.Items.Length == p.Tracks.Total)
            {
                tracks = new List<Track>(p.Tracks.Items.Select(x => x.Track).Where(y => y != null));
            }
            else
            {
                var plTracks = await DataLoader.GetInstance().GetAllItemsFromPagingWrapper(p.Tracks, true, 5000);
                tracks = new List<Track>(plTracks.Select(x => x.Track).Where(y => y != null));
            }

            //IEnumerable<IGrouping<string, Track>> groups = (from track in tracks where track.Id != null group track by track.Id into g select g);

            tracks = tracks.Where(x => x.Id != null).ToList();

            while (tracks.Count + CurrentParallelTasks >= _maxParallelTask)
                Thread.Sleep(5);

            _runningSortingTasks++;


            await Task.Run(() => 
            {
                tracks.ForEach((t) =>
                {
                    if (!_result.Tracks.TryAdd(t.Id, new AggregationSearchTrackItem(t, p)))
                    {
                        _result.Tracks[t.Id].AddCount(1, p);
                    }
                    
                });
                _runningSortingTasks--;
            });

            _runningGetTrackTasks--;

            if (_searchLoadStartCompleted && CurrentParallelTasks - _runningSortingTasks == 0)
            {
                while (_runningSortingTasks > 0)
                    Thread.Sleep(5);
                _callback();
            }
        }
    }
}
