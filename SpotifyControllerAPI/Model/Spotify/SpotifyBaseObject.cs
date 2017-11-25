using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class SpotifyBaseObject
    {
        public string Href { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Uri { get; set; }

        public Image[] Images { get; set; }

        public Dictionary<string, string> External_Urls { get; set; }
        public string ImageUrl
        {
            get => Images != null ? Images.Select(x => x.Url).FirstOrDefault(y => !string.IsNullOrEmpty(y)) : string.Empty;
        }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }

    public class SpotifyObjectEqualityComparere : IEqualityComparer<SpotifyBaseObject>
    {
        public bool Equals(SpotifyBaseObject x, SpotifyBaseObject y)
        {
            return x.GetType() == y.GetType() && x.Id == y.Id;
        }

        public int GetHashCode(SpotifyBaseObject obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
