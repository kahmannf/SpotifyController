using Newtonsoft.Json;
using SpotifyControllerAPI.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Web
{
    public class Controller
    {
        public const string PLAY_TRACK_URL = "https://api.spotify.com/v1/me/player/play";

        public const string TRANSFER_PLAYBACK_URL = "https://api.spotify.com/v1/me/player";

        public static Controller GetInstance()
        {
            if (_instance == null)
                _instance = new Controller();

            return _instance;
        }

        private static Controller _instance;

        private Controller()
        {
        }

        public async Task<bool> PlayTrack(string trackuri, string deviceid = null,  int callCount = 0)
        {
            if (callCount < 5)
            {

                PlayTrackRequest ptr = new PlayTrackRequest()
                {
                    uris = new string[] { trackuri }
                };

                string body = JsonConvert.SerializeObject(ptr);

                string url = string.Empty;

                if (string.IsNullOrEmpty(deviceid))
                {
                    url = PLAY_TRACK_URL;
                }
                else
                {
                    url = WebHelper.GetQueryUrl(PLAY_TRACK_URL, new NameValueCollection() { { "device_id", deviceid } });
                }

                HttpWebResponse response = await ExecuteWebrequest(url, body);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.NoContent: //204 =>Successful
                        return true;
                    case HttpStatusCode.Accepted://202 => device currently unavailable, retry the request after 5 seconds, but no more than at most 5 retries.
                        await Task.Run(() => Thread.Sleep(5000));

                        return await PlayTrack(trackuri, deviceid, callCount + 1);
                    default: //for debug purpose
                        StreamReader reader = new StreamReader(response.GetResponseStream());

                        string resBody = reader.ReadToEnd();


                        return false;
                }
            }

            return false;
        }

        public async Task TransferPlayback(string deviceId, bool forcePlay = false)
        {
            Dictionary<string, object> requestBody = new Dictionary<string, object>();

            requestBody.Add("device_ids", new string[] { deviceId });
            requestBody.Add("play", forcePlay);

            string bodyString = JsonConvert.SerializeObject(requestBody);

            await ExecuteWebrequest(TRANSFER_PLAYBACK_URL, bodyString);
        }

        #region ExecuteWebrequest Methods

        private async Task<HttpWebResponse> ExecuteWebrequest(string url, string body = null)
        {
            HttpWebRequest request = WebHelper.CreateTokenizedRequest(url);

            request.Method = "PUT";

            return await ExecuteWebrequest(request, body);
        }

        /// <summary>
        /// Executes a webrequest
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <param name="retryOnLimit">wheter or not the webrequest should be retried if spotify limits the amount of requests</param>
        /// <param name="retryAfterLimitms">a maximum of milliseconds to wait for a retry. if you would have to wait longer, an exception is thrown. if 0 will always wait</param>
        /// <returns></returns>
        private async Task<HttpWebResponse> ExecuteWebrequest(HttpWebRequest request,  string body = null)
        {
            try
            {

                if (!string.IsNullOrEmpty(body))
                {
                    request.ContentType = "application/json";

                    request.Accept = "application/json";

                    Stream requestStream = request.GetRequestStream();

                    await requestStream.WriteAsync(body.Select(x => (byte)x).ToArray(), 0, body.Length);
                }

                SchedulerResult result = await WebRequestScheduler.Instance.RunInSchedule(request);

                if (!result.Successful)
                    throw result.Exception;

                return result.Response;
            }
            catch (WebException ex)
            {
                return (HttpWebResponse)ex.Response;
            }


        }

        #endregion
    }
}
