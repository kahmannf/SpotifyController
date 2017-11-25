using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify.AudioAnalysis
{
    public class AnalysisTrack
    {
        public string Num_Samples { get; set; }
        public double Duration { get; set; }
        public string Sample_MD5 { get; set; }
        public double Offset_Seconds { get; set; }
        public double Window_Seconds { get; set; }
        public int Analysis_Sample_Rate { get; set; }
        public int Analysis_Channels { get; set; }
        public double End_Of_Fade_Int { get; set; }
        public double Start_Of_Fade_Out { get; set; }
        public double Loudness { get; set; }
        public double Tempo { get; set; }
        public double Tempo_Confidence { get; set; }
        public int Key { get; set; }
        public double Key_Confidence { get; set; }
        public int Mode { get; set; }
        public double Mode_Confidence { get; set; }
        public int Time_Signature { get; set; }
        public double Time_Signature_Confidence { get; set; }
        public string CodeString { get; set; }
        public double Code_Version { get; set; }
        public string EchoPrintString { get; set; }
        public double EchoPrint_Version { get; set; }
        public string SynchString { get; set; }
        public double Synch_Version { get; set; }
        public string RhythmString { get; set; }
        public double Rhythm_Version { get; set; }

    }
}
