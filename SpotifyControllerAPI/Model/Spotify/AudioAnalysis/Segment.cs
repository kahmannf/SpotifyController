using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify.AudioAnalysis
{
    public class Segment : AudioBar
    {
        public double Loudness_Start { get; set; }
        public double Loudness_Max_Time { get; set; }
        public double Loudness_Max { get; set; }
        public double Loudness_End { get; set; }
        public double[] Pitches { get; set; }
        public double[] Timbre { get; set; }
    }
}
