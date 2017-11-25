using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyControllerAPI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace SpotifyControllerAPI.Web.Authentication
{
    public class Authenticator : IDisposable
    {
        private static Authenticator _instance;

        //public const string BROWSER_DIRECTORY = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

        public const string SPOTIFY_AUTH_URL = "https://accounts.spotify.com/authorize";

        public const string SPOTIFY_TOKEN_URL = "https://accounts.spotify.com/api/token";

        public const string SPOTIFY_ACCESS_SCOPE = 
            "playlist-read-private " +
            "playlist-read-collaborative " +
            "streaming " +
            "user-follow-read " +
            "user-library-read " +
            "user-read-private " +
            "user-read-birthdate " +
            "user-read-email " +
            "user-top-read " +
            "user-read-playback-state " +
            "user-modify-playback-state " +
            "user-read-currently-playing " +
            "user-read-recently-played";


        private static WebApiConfig _webApiConfig;

        private const string RETURN_URL = "http://localhost:5000/spotify_callback/";


        private static string AuthenticationBase64
        {
            get
            {
                string value = _webApiConfig.ClientId + ":" + _webApiConfig.ClientSecret;

                var plainTextBytes = Encoding.UTF8.GetBytes(value);
                return Convert.ToBase64String(plainTextBytes);
            }
        }

        public string AccessToken { get; private set; }

        public string RefreshToken { get; private set; }

        public string TokenType { get; private set; }

        public string Scope { get; private set; }

        public int TokenExpiresIn { get; private set; }

        public long TokenRecievedAt { get; private set; }

        private string _state { get; set; }

        private bool _keepAuthenticated = false;

        private bool _waitingForTokens = false;

        public bool IsAuthenticated => _keepAuthenticated && AccessToken != null && RefreshToken != null;

        public static Authenticator GetInstance()
        {
            if (_instance == null)
                _instance = new Authenticator();

            return _instance;
        }

        private Authenticator()
        {
            _webApiConfig = WebApiConfig.Load();
        }

        public async Task<bool> StartAuthenticatedContext(Action<string> sendUpdateMessage)
        {
            try
            {
                if (await Authenticate(sendUpdateMessage))
                {
                    _keepAuthenticated = true;
                    KeepAutheticatedInternal();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A exception occured while establishing a authenticated context.:" + Environment.NewLine + Environment.NewLine + ex.ToString());
            }
            return false;
        }

        public async void EndAuthenticatedContext()
        {
            _keepAuthenticated = false;

            await Task.Run(() =>Thread.Sleep(2000));

            AccessToken = null;
            RefreshToken = null;
        }

        private async void KeepAutheticatedInternal()
        {
            while (_keepAuthenticated)
            {
                long passedTicks = (Environment.TickCount - TokenRecievedAt);

                int secondsLeft = (int)passedTicks / 1000;

                if (secondsLeft < 10 && !_waitingForTokens)
                {
                    await RefreshAccessToken(); 
                }


                await Task.Run(() => Thread.Sleep(1000));
            }
        }

        private async Task<bool> Authenticate(Action<string> sendUpdateMessage)
        {
            _state = Guid.NewGuid().ToString();

            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection()
            {
                { "response_type", "code" },
                { "client_id", _webApiConfig.ClientId },
                { "scope", SPOTIFY_ACCESS_SCOPE },
                { "redirect_uri", RETURN_URL },
                { "state", _state}
            };

            sendUpdateMessage("Waiting for User-Log-In");

            HttpListener listener = new HttpListener();

            listener.Prefixes.Add(RETURN_URL);

            listener.Start();
            
            string url = WebHelper.GetQueryUrl(SPOTIFY_AUTH_URL, nvc);

            //Seems like you can just run an URL....
            //System.Diagnostics.Process.Start(BROWSER_DIRECTORY, url);
            System.Diagnostics.Process.Start(url);

            HttpListenerContext context = listener.GetContext();

            System.Collections.Specialized.NameValueCollection result = context.Request.QueryString;

            string responseString = "<script>close();</script>";

#pragma warning disable CS4014 // Da dieser Aufruf nicht abgewartet wird, wird die Ausführung der aktuellen Methode fortgesetzt, bevor der Aufruf abgeschlossen ist
            context.Response.OutputStream.WriteAsync(responseString.Select(x => (byte)x).ToArray(), 0, responseString.Length);
#pragma warning restore CS4014 // Da dieser Aufruf nicht abgewartet wird, wird die Ausführung der aktuellen Methode fortgesetzt, bevor der Aufruf abgeschlossen ist

            context.Response.Close();

            if (result.Get("state") != _state)
            {
                sendUpdateMessage("Failed to retrieve Access-Code!");
                return false;
            }
            else
            {

                string formTemplate =
                    "grant_type=authorization_code&" +
                    "code={0}&" +
                    "redirect_uri={1}";

                string formBody = string.Format(formTemplate, result.Get("code"), RETURN_URL);

                sendUpdateMessage("Requiring AccessTokens...");

                return await AquireTokens(formBody);
            }
        }

        private async Task<bool> AquireTokens(string formBody)
        {
            _waitingForTokens = true;

            HttpWebRequest request = WebRequest.CreateHttp(SPOTIFY_TOKEN_URL);

            request.Headers.Add("Authorization", "Basic " + AuthenticationBase64);

            request.Method = "POST";

            request.ContentType = "application/x-www-form-urlencoded";
            
            Stream requestStream = request.GetRequestStream();

            await requestStream.WriteAsync(formBody.Select(x => (byte)x).ToArray(), 0, formBody.Length);

            SchedulerResult result = await WebRequestScheduler.Instance.RunInSchedule(request);

            if (!result.Successful)
                throw result.Exception;

            HttpWebResponse response = result.Response;

            try
            {
                request = null;
                requestStream?.Close();
                requestStream?.Dispose();
                requestStream = null;
            }
            catch { }

            Stream responseStream = response.GetResponseStream();

            string resultData = WebHelper.ReadStream(responseStream);

            if (!string.IsNullOrEmpty(resultData))
            {
                TokenRecievedAt = Environment.TickCount;
                return ParseTokenRequestData(resultData);
            }

            try
            {
                response = null;
                responseStream?.Close();
                responseStream?.Dispose();
                responseStream = null;
            }
            catch { }
            
            _waitingForTokens = false;

            return false;
        }

        private async Task RefreshAccessToken()
        {
            string formTemplate =
                "grant_type=refresh_token&" +
                "refresh_token={0}";

            string formBody = string.Format(formTemplate, RefreshToken);

            await AquireTokens(formBody);
        }

        private bool ParseTokenRequestData(string rawJson)
        {
            JObject data = (JObject)JsonConvert.DeserializeObject(rawJson);

            int count = 0;

            foreach (var item in data)
            {
                switch (item.Key)
                {
                    case "access_token":
                        AccessToken = item.Value.ToString();
                        count++;
                        break;
                    case "refresh_token":
                        RefreshToken = item.Value.ToString();
                        count++;
                        break;
                    case "token_type":
                        TokenType = item.Value.ToString();
                        count++;
                        break;
                    case "expires_in":
                        TokenExpiresIn = int.Parse(item.Value.ToString());
                        count++;
                        break;
                }
            }

            _waitingForTokens = false;

            return count == 4;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EndAuthenticatedContext();

                    _instance = null;
                }

                // TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer weiter unten überschreiben.
                // TODO: große Felder auf Null setzen.

                disposedValue = true;
            }
        }

        // TODO: Finalizer nur überschreiben, wenn Dispose(bool disposing) weiter oben Code für die Freigabe nicht verwalteter Ressourcen enthält.
        // ~Authenticator() {
        //   // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
        //   Dispose(false);
        // }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            // TODO: Auskommentierung der folgenden Zeile aufheben, wenn der Finalizer weiter oben überschrieben wird.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
