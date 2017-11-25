using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class Artist : SpotifyBaseObject
    {
        public Followers Followers { get; set; }

        public string[] Genres { get; set; }

        public int Popularity { get; set; }
    }
}
