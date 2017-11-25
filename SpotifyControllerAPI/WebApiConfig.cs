using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SpotifyControllerAPI
{
    public class WebApiConfig
    {
        private static string _filename => System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "webapiconfig.config");

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        internal static WebApiConfig Load()
        {
            string filename = _filename;
            if (!System.IO.File.Exists(filename))
            {
                return CreateNew();
            }
            else
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open))
                {
                    return (WebApiConfig)new XmlSerializer(typeof(WebApiConfig)).Deserialize(fs);
                }
            }
        }

        private static WebApiConfig CreateNew()
        {
            WebApiConfig conf = new WebApiConfig()
            {
                ClientId = "Your ClientID",
                ClientSecret = "Your ClientSecret"
            };

            using (System.IO.FileStream fs = new System.IO.FileStream(_filename, System.IO.FileMode.CreateNew))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(WebApiConfig));

                serializer.Serialize(fs, conf);
            }

            return conf;
        }
    }
}
