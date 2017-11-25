using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify.AudioAnalysis
{
    public class AnalysisMetaData
    {
        public string Analyzer_Version { get; set; }
        public string Plattform { get; set; }
        public string Detailed_Status { get; set; }
        public int Status_Code { get; set; }
        public long Timestamp { get; set; }
        public double Analysis_Time { get; set; }
        public string Input_Process { get; set; }
    }
}
