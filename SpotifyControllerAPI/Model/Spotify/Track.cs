using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class Track : SpotifyBaseObject
    {
        public Album Album { get; set; }

        public Artist[] Artists { get; set; }

        public string[] Available_Markets { get; set; }

        public int Dics_Number { get; set; }

        public int Duration_ms { get; set; }

        public bool Explicit { get; set; }

        public Dictionary<string, string> External_Ids { get; set; }

        public bool Is_Playable { get; set; }

        public Track Linked_From { get; set; }

        public string Restrictions { get; set; }

        public int Popularity { get; set; }

        public string Preview_Url { get; set; }
        
        public int Track_Number { get; set; }

        public string ArtistNames =>  Artists != null ? string.Join(", ", Artists.Select(x => x.Name)) : string.Empty;
    }
}
