using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify.AudioAnalysis
{
    public class Section : AudioBar
    {
        public double Loudness { get; set; }
        public double Tempo { get; set; }
        public double Tempo_Confidence { get; set; }
        public int Key { get; set; }
        public double Key_Confidence { get; set; }
        public int Mode { get; set; }
        public double Mode_Confidence { get; set; }
        public int Time_Signature { get; set; }
        public double Time_Signature_Confidence { get; set; }
    }
}
