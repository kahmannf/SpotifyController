using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class Playlist : SpotifyBaseObject
    {
        public bool Collaborative { get; set; }

        public string Description { get; set; }

        public Followers Followers { get; set; }
        
        public User Owner { get; set; }

        public bool? Public { get; set; }

        public string Snapshot_Id { get; set; }

        public PagingWrapper<PlaylistTrack> Tracks { get; set; }

    }
}
