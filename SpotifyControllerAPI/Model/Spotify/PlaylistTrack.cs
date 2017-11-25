using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class PlaylistTrack
    {
        public DateTime? Added_At { get; set; }

        public User Added_By { get; set; }

        public bool Local { get; set; }

        public Track Track { get; set; }
    }
}
