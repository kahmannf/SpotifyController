using SpotifyControllerAPI.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SpotifyControllerAPI.Web
{
    public static class WebHelper
    {
        public static string GetQueryUrl(string baseUrl, NameValueCollection qparams)
        {
            var builder = new UriBuilder(baseUrl);

            // Create query string with all values
            builder.Query = string.Join("&", qparams.AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(qparams[key]))));

            // Omit empty values
            builder.Query = string.Join("&", qparams.AllKeys.Where(key => !string.IsNullOrWhiteSpace(qparams[key])).Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(qparams[key]))));

            return builder.Uri.ToString();
        }

        public static HttpWebRequest CreateTokenizedRequest(string url)
        {
            Authenticator auth = Authenticator.GetInstance();

            if (!auth.IsAuthenticated)
            {
                throw new InvalidOperationException("A tokenized request can only be aquired if the Authenticator is in an authenticated state!");
            }

            HttpWebRequest request = WebRequest.CreateHttp(url);
            
            request.Headers.Add("Authorization", "Bearer " + auth.AccessToken);

            return request;
        }

        public static string ReadStream(Stream s)
        {
            string resultData = string.Empty;

            while (s.ReadByte() is int i && i > -1)
            {
                resultData += (char)i;
            }

            return resultData;
        }
    }
}
