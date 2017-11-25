using SpotifyControllerAPI.Model.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model
{
    public enum SessionContext { Unknown, Backlog, Queue }
    public class SessionHistoryItem
    {
        public DateTime TimeStamp { get; set; }
        public Track Track { get; set; }
        public SessionContext Context { get; set; }
    }
}
