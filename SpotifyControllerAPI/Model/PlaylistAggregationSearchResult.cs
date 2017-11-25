using SpotifyControllerAPI.Model.Spotify;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model
{
    public class PlaylistAggregationSearchResult
    {
        public ConcurrentDictionary<string, AggregationSearchTrackItem> Tracks = new ConcurrentDictionary<string, AggregationSearchTrackItem>();
        public ConcurrentDictionary<string, Playlist> Playlists = new ConcurrentDictionary<string, Playlist>();

        private AggregationSearchTrackItem[] _sortedItems;

        private bool _sorted;

        public void Sort()
        {
            Stopwatch watch = Stopwatch.StartNew();

            if (_sorted)
                return;
            _sorted = true;

            _sortedItems = Tracks.Values.ToArray();

            ApiHelper.MergeSort(_sortedItems,(x, y) => 
            {
                if (x.Count > y.Count)
                    return 1;
                else if (y.Count > x.Count)
                    return -1;
                else
                    return 0;
            });

            watch.Stop();

            string s = watch.Elapsed.ToString();
        }

        public AggregationSearchTrackItem[] GetPage(int pageNumber, int pageSize)
        {
            if (!_sorted)
            {
                Sort();
            }

            AggregationSearchTrackItem[] result = new AggregationSearchTrackItem[pageSize];

            int previousItems = pageNumber * pageSize;

            for (int i = previousItems; i < previousItems + pageSize; i++)
            {
                result[i - previousItems] = _sortedItems[i];
            }

            return result;
        }
    }
}
