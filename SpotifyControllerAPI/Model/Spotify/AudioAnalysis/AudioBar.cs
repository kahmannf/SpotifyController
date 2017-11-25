using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify.AudioAnalysis
{
    public class AudioBar
    {
        public double Start { get; set; }
        public double Duration { get; set; }
        public double Confidence { get; set; }
    }
}
