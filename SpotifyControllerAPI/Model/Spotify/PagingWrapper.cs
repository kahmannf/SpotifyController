using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class PagingWrapper<T>
        where T : new()
    {
        public string Href { get; set; }
        public T[] Items { get; set; }
        public int Limit { get; set; }
        public string Next { get; set; }
        public int Offset { get; set; }
        public string Previous { get; set; }
        public int Total { get; set; }
    }
}
