using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model
{
    public class SchedulerResult
    {
        public bool Successful { get; set; }
        public Exception Exception { get; set; }
        public HttpWebResponse Response { get; set; }

        public SchedulerResult(Exception ex)
        {
            Successful = false;
            Exception = ex;
        }

        public SchedulerResult(HttpWebResponse response)
        {
            Successful = true;
            Response = response;
        }
    }
}
