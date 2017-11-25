using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class CurrentlyPlaying
    {
        public SpotifyBaseObject Context { get; set; }

        public long Timestamp { get; set; }

        public int Progress_ms { get; set; }

        public bool Is_Playing { get; set; }

        public Track Item { get; set;}
    }
}
