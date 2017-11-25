using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class SearchResult
    {
        public PagingWrapper<Artist> Artists { get; set; }
        public PagingWrapper<Album> Albums { get; set; }
        public PagingWrapper<Track> Tracks { get; set; }
        public PagingWrapper<Playlist> Playlists { get; set; }
        public SearchConfiguration Config { get; set; }

        public object GetPagingWrapper<T>()
        {
            if (typeof(T) == typeof(Artist))
            {
                return this.Artists;
            }
            else if (typeof(T) == typeof(Album))
            {
                return this.Albums;
            }
            else if (typeof(T) == typeof(Track))
            {
                return this.Tracks;
            }
            else if (typeof(T) == typeof(Playlist))
            {
                return this.Playlists;
            }
            else
            {
                throw new Exception($"No PagingWrapper for the Type \"{typeof(T).GetType().ToString()}\"");
            }
        }
    }
}
