using SpotifyControllerAPI.Model.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model
{
    public class AggregationSearchTrackItem
    {
        public int Count { get; private set; }
        public Track Track { get; private set; }

        public List<Playlist> Playlists { get; private set; }
        

        public AggregationSearchTrackItem(Track t, Playlist p, AggregationSearchTrackItem firstItem)
        {
            Track = t;
            Count = firstItem.Count + 1;
            Playlists = firstItem.Playlists;
            Playlists.Add(p);
        }

        public AggregationSearchTrackItem(Track t, Playlist p)
        {
            Track = t;
            Count = 1;
            Playlists = new List<Playlist>();
            Playlists.Add(p);
        }

        public void AddCount(int count, Playlist p)
        {
            Count += count;
            Playlists.Add(p);
        }
    }
}
