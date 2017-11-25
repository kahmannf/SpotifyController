using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model
{
    public class WebRequestStatus
    {
        public int RequestsInQueue { get; set; }
        public bool Blocked { get; set; }
        public int RemainingWaitTime { get; set; }
        public int RunningThreads { get; set; }
    }
}
