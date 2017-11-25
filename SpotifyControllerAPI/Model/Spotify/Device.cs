using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class Device : SpotifyBaseObject
    {
        public bool Is_Active { get; set; }

        public bool Is_Restricted { get; set; }

        public int? Volume_Percent { get; set; }

        /* Helper property. Note that by default, spotify doenst provide images for devices */
        public string DeviceImage
        {
            get
            {
                if (Images != null && Images.Length != 0)
                {
                    return Images[0].Url;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
