using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class Album : SpotifyBaseObject
    {
        public string Album_Type { get; set; }

        public Artist[] Artists { get; set; }

        public string[] Available_Markets { get; set; }

        public CopyRight[] Copyrights { get; set; }

        public Dictionary<string,string> External_Ids { get; set; }

        public string[] Genres { get; set; }

        public string Label { get; set; }

        public int Popularity { get; set; }

        public string Release_Date { get; set; }

        public string Release_Date_Precision { get; set; }

        public PagingWrapper<Track> Tracks { get; set; }

    }
}
