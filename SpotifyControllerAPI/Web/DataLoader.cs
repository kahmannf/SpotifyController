using Newtonsoft.Json;
using SpotifyControllerAPI.Model;
using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Model.Spotify.AudioAnalysis;
using SpotifyControllerAPI.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SpotifyControllerAPI.Web
{
    public class DataLoader
    {
        public const string GET_USER_URL = "https://api.spotify.com/v1/users/";
        public const string GET_LOGGED_IN_USER_URL = "https://api.spotify.com/v1/me/";

        public const string GET_USERS_PLAYLISTS_URL = "https://api.spotify.com/v1/me/playlists";

        public const string GET_CURRENTLY_PLAYING = "https://api.spotify.com/v1/me/player/currently-playing";

        public const string GET_USER_DEVICES_URL = "https://api.spotify.com/v1/me/player/devices";

        /// <summary>
        /// replace 0 with albumid
        /// </summary>
        public const string GET_ALBUM_TRACKS_URL_FORMAT = "https://api.spotify.com/v1/albums/{0}/tracks";

        /// <summary>
        /// replace 0 with artistid
        /// </summary>
        public const string GET_ARTIST_ALBUMS_URL_FORMAT = "https://api.spotify.com/v1/artists/{0}/albums";

        /// <summary>
        /// replace 0 with trackid
        /// </summary>
        public const string GET_AUDIO_ANALYSIS_URL_FORMAT = "https://api.spotify.com/v1/audio-analysis/{0}";

        public const string GET_SEARCH_RESULT_URL = "https://api.spotify.com/v1/search";

        private static DataLoader _instance;

        public static DataLoader GetInstance()
        {
            if (_instance == null)
                _instance = new DataLoader();

            return _instance;
        }

        private DataLoader()
        {
        }


        public async Task<CurrentlyPlaying> GetCurrentlyPlaying(bool retryOnLimit = false, int retryAfterLimitms = 5000)
        {
            try
            {
                Stopwatch watch = Stopwatch.StartNew();

                long unixtimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                CurrentlyPlaying result = await ExecuteWebrequest<CurrentlyPlaying>(GET_CURRENTLY_PLAYING, retryOnLimit, retryAfterLimitms);

                watch.Stop();
                
                if (result != null)
                {
                    result.Timestamp = unixtimestamp;
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<PagingWrapper<Playlist>> GetUsersPlaylistPage(int amount, int offset)
        {
            try
            {
                if (amount < 1 || amount > 50)
                {
                    throw new ArgumentOutOfRangeException("Amount has to be within 1 and 50");
                }

                if (offset < 0 || offset > 100_000)
                {
                    throw new ArgumentOutOfRangeException("Offset has to be within 0 and 100.000");
                }

                NameValueCollection parameter = new NameValueCollection()
            {
                { "limit", amount.ToString() },
                { "offset", offset.ToString() }
            };

                string url = WebHelper.GetQueryUrl(GET_USERS_PLAYLISTS_URL, parameter);

                return await ExecuteWebrequest<PagingWrapper<Playlist>>(url);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SpotifyLoadException($"Failed to load user-playlist (amount: {amount}; offset: {offset}). See inner Exception for details.", ex);
            }
        }

        public async Task<T> GetItemFromHref<T>(string href, bool retryOnLimit = false, int retryAfterLimitms = 5000)
            where T : new()
        {
            return await ExecuteWebrequest<T>(href, retryOnLimit, retryAfterLimitms);
        }

        public async Task<User> GetUser(string id)
        {
            string url = GET_USER_URL + HttpUtility.UrlEncode(id);

            return await ExecuteWebrequest<User>(url);
        }

        public async Task<User> GetLoggedInUser()
        {
            try
            {
                return await ExecuteWebrequest<User>(GET_LOGGED_IN_USER_URL);
            }
            catch (Exception ex)
            {
                throw new SpotifyLoadException("Failed to load the LoggedInUser. See inner exception for details.", ex);
            }
        }

        public async Task<DevicePayloadItem> GetUserDevices()
        {
            try
            {
                return await ExecuteWebrequest<DevicePayloadItem>(GET_USER_DEVICES_URL);
            }
            catch (Exception ex)
            {
                throw new SpotifyLoadException("Failed to load user-devices. See inner Exception for details.", ex);
            }
        }

        public async Task<List<Album>> GetArtitstAllAlbums(string artistid, string market)
        {
            string baseUrl = string.Format(GET_ARTIST_ALBUMS_URL_FORMAT, artistid);

            NameValueCollection qparams = new NameValueCollection()
            {
                { "market", market },
                { "limit", 50.ToString() }
            };

            string url = WebHelper.GetQueryUrl(baseUrl, qparams);

            PagingWrapper<Album> firstPage = await ExecuteWebrequest<PagingWrapper<Album>>(url);

            return await GetAllItemsFromPagingWrapper(firstPage);
        }

        public async Task<List<T>> GetAllItemsFromPagingWrapper<T>(PagingWrapper<T> page, bool retryOnLimit = false, int retryAfterLimitms = 5000)
            where T : new()
        {
            List<T> result = new List<T>();

            PagingWrapper<T> current = page;

            List<PagingWrapper<T>> previous = new List<PagingWrapper<T>>();

            while (!string.IsNullOrEmpty(current.Previous))
            {
                current = await GetItemFromHref<PagingWrapper<T>>(current.Previous, retryOnLimit, retryAfterLimitms);

                previous.Add(current);
            }
            
            if (previous.Count > 0)
            {
                result.AddRange(((previous as IEnumerable<PagingWrapper<T>>).Reverse()// returns ordered enumerable paging wrappers
                    .Select(x => x.Items as IEnumerable<T>) // returns IEnumerable<IEnumerable<T>>
                    .Aggregate((x, y) => (x).Union(y))));
            }

            if (page.Items == null)
            {
                page = await GetItemFromHref<PagingWrapper<T>>(page.Href, retryOnLimit, retryAfterLimitms);
            }

            result.AddRange(page.Items);

            current = page;

            while (!string.IsNullOrEmpty(current.Next))
            {
                current = await GetItemFromHref<PagingWrapper<T>>(current.Next, retryOnLimit, retryAfterLimitms);

                result.AddRange(current.Items);
            }

            return result;
        }

        public async Task<List<Track>> GetAblumAllTracks(string albumid)
        {
            string baseUrl = string.Format(GET_ALBUM_TRACKS_URL_FORMAT, albumid);

            NameValueCollection qparams = new NameValueCollection()
            {
                { "limit", "50" }
            };

            string url = WebHelper.GetQueryUrl(baseUrl, qparams);

            PagingWrapper<Track> firstPage = await ExecuteWebrequest<PagingWrapper<Track>>(url);

            return await GetAllItemsFromPagingWrapper(firstPage);
        }

        public async Task<AudioAnalysis> GetAudioAnalysis(string trackId)
        {
            string url = string.Format(GET_AUDIO_ANALYSIS_URL_FORMAT, trackId);

            return await ExecuteWebrequest<AudioAnalysis>(url);
        }

        public async Task<SearchResult> Search(SearchConfiguration config, bool retryOnLimit = false, int retryAfterLimitms = 5000)
        {
            string baseUrl = GET_SEARCH_RESULT_URL;

            NameValueCollection qparams = new NameValueCollection()
            {
                { "type", string.Join(",", config.Scope) },
                { "q", config.SearchText },
                { "limit", config.Amount.ToString() },
                { "offset", config.Offset.ToString() },
                { "market", "from_token" }
            };

            string url = WebHelper.GetQueryUrl(baseUrl, qparams);

            SearchResult result = await ExecuteWebrequest<SearchResult>(url);

            result.Config = config;

            return result;
        } 

        #region ExecuteWebrequest Methodes

        private async Task<TResult> ExecuteWebrequest<TResult>(string url, bool retryOnLimit = false, int retryAfterLimitms = 5000)
            where TResult : new()
        {
            HttpWebRequest request = WebHelper.CreateTokenizedRequest(url);
            try
            {
                return await ExecuteWebrequest<TResult>(request, retryOnLimit, retryAfterLimitms);
            }
            catch (Exception ex)
            {
                throw new SpotifyLoadException($"A WebException occured while consuming the WebAPI on URL {url}. See inner exceptions for Deatils", ex);
            }
        }

        /// <summary>
        /// Executes a webrequest
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <param name="retryOnLimit">wheter or not the webrequest should be retried if spotify limits the amount of requests</param>
        /// <param name="retryAfterLimitms">a maximum of milliseconds to wait for a retry. if you would have to wait longer, an exception is thrown. if 0 will always wait</param>
        /// <returns></returns>
        private async Task<TResult> ExecuteWebrequest<TResult>(HttpWebRequest request, bool retryOnLimit = false, int retryAfterLimitms = 5000)
            where TResult : new()
        {
            try
            {
                SchedulerResult schedulerResult = await WebRequestScheduler.Instance.RunInSchedule(request);

                if (!schedulerResult.Successful)
                    throw schedulerResult.Exception;

                HttpWebResponse response = schedulerResult.Response;

                string data = string.Empty;

                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    data = await reader.ReadToEndAsync();
                }

                TResult result = default(TResult);

                if (!string.IsNullOrEmpty(data))
                {
                    result = JsonConvert.DeserializeObject<TResult>(data);

                    //if (!result.FromJson(data))
                    //{
                    //    throw new Exception("Failed to parse the json-string.");
                    //}
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new SpotifyLoadException("A WebException occured while consuming the WebAPI. See inner exceptions for Details", ex);
            }

            
        }

        #endregion
    }
}
