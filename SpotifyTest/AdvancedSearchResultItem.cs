using SpotifyControllerAPI.Model.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyController
{
    public class AdvancedSearchResultItem
    {
        public int Count { get; private set; }
        public Track Track { get; private set; }

        public AdvancedSearchResultItem(Track t)
            :this(t, 1)
        {
        }

        public AdvancedSearchResultItem(Track t, int amount)
        {
            Track = t;
            Count = amount;
        }

        public bool CheckIfSame(Track t, int count)
        {
            if (t.Id == Track.Id)
            {
                Count += count;
                return true;
            }
            return false;
        }
    }
}
