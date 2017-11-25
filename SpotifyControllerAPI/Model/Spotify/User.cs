using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model.Spotify
{
    public class User : SpotifyBaseObject
    {
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// The Country Code
        /// </summary>
        public string Country { get; set; }

        public string Display_Name { get; set; }

        public string Email { get; set; }
        
        public Followers Followers { get; set; }

        public string Product { get; set; }
        

        /// <summary>
        /// Display_Name (if not null) and spotify id in "{}"
        /// </summary>
        public string UIDisplayName
        {
            get => string.IsNullOrEmpty(Display_Name) ? Id : Display_Name;
        }
    }
}
