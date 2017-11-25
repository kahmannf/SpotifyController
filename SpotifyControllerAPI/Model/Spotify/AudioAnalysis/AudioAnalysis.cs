using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify.AudioAnalysis
{
    public class AudioAnalysis
    {
        public AudioBar[] Bars { get; set; }

        public AudioBar[] Beats { get; set; }

        public AnalysisMetaData Meta { get; set; }

        public Section[] Sections { get; set; }

        public Segment[] Segments { get; set; }

        public AudioBar[] Tatums { get; set; }

        public AnalysisTrack Track { get; set; }
    }
}
