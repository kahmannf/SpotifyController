using RandomUtilities.Queue;
using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyControllerAPI.Model
{
    public class Session
    {
        /// <summary>
        /// Amount of milliseconds after which the session will update its play-time and current song
        /// </summary>
        public const long SESSION_UPDATE_SLEEP_PAUSE_MS = 1000;

        /// <summary>
        /// Amount of milliseconds that should pass between the ticks (actual value is usually a bit more)
        /// </summary>
        public const long SESSION_TICK_SLEEP_PAUSE_MS = 20;

        /// <summary>
        /// Amount of milliseconds that determine when the session will skip to next song. Note that c# time is only about 10-16 ms acurate 
        /// Dont chooes int.MaxValue. Seriously, just dont.
        /// </summary>
        public const int SESSION_NEXT_SONG_BEVOR_END_MS = 300;

        /// <summary>
        /// refill backlog-queue when itemcount hits this number;
        /// </summary>
        public const int AMOUNT_ITEMS_LEFT_TO_REPOPULATED_BACKLOG = 0;

        private User _user;

        public event EventHandler SessionStateChanged;

        public Session(User user)
        {
            _user = user;
            CurrentBacklogQueue = new FILO<Track>();
            CurrentManualQueue = new FILO<Track>();
            BacklogItems = new List<SpotifyBaseObject>();

            _playedSongs = new List<SessionHistoryItem>();

            rng = new Random();
        }

        private Random rng;

        public string DeviceId { get; private set; }

        public Track CurrentTrack { get; private set; }

        public FILO<Track> CurrentBacklogQueue { get; private set; }

        public FILO<Track> CurrentManualQueue { get; private set; }

        public Track NextTrackPeek
        {
            get
            {
                if (CurrentManualQueue.HasItems)
                    return CurrentManualQueue.Peek();
                if (CurrentBacklogQueue.HasItems)
                    return CurrentBacklogQueue.Peek();
                else
                    return null;
            }
        }

        private List<SessionHistoryItem> _playedSongs { get; set; }

        /// <summary>
        /// Playlist/Albums/Songs added to the playlist
        /// </summary>
        public List<SpotifyBaseObject> BacklogItems { get; private set; }

        public bool IsRunning { get; private set; }

        private CurrentlyPlaying _playbackState;

        private long _unixTimestamp => DateTimeOffset.Now.ToUnixTimeMilliseconds();

        /// <summary>
        /// Evaluates how many milliseconds are left until the current Track is over;
        /// Note that this value assumes the playback was not paused after the last update of _playbackState
        /// update the playbackstate to check whether the track is currently playing
        /// </summary>
        private long _possibleMSLeftInTrack => _playbackState != null ? _playbackState.Item.Duration_ms - _playbackState.Progress_ms + _playbackState.Timestamp - _unixTimestamp : -1;

        public Tuple<Track, SessionContext> PullNextTrack()
        {
            if (CurrentManualQueue.HasItems)
                return new Tuple<Track, SessionContext>(CurrentManualQueue.Pull(), SessionContext.Queue);
            else
                return new Tuple<Track, SessionContext>(CurrentBacklogQueue.Pull(), SessionContext.Backlog);
        }

        private async Task<List<Track>> GetBacklogTracks()
        {
            IEnumerable<Task<List<Track>>> listTrackListChunksTasks = BacklogItems.Select(async(x) => await ApiHelper.GetTracks(x, _user));

            Task<List<Track>> resultTask = listTrackListChunksTasks.Aggregate(async (y, z) => new List<Track>((await y).Union(await z)));

            return (await resultTask).ToList();
        }

        

        private string[] allowedBacklogItems = new string[] { "playlist", "artist", "album", "track" };

        public void Start()
        {
            if (IsRunning)
                throw new InvalidOperationException("The session is alread running");

            IsRunning = true;
            
            RunInternal();
        }

        public void AddItemToBacklog(SpotifyBaseObject item)
        {
            if (!allowedBacklogItems.Contains(item.Type.ToLower()))
            {
                throw new InvalidOperationException($"Cannot add a SpotifyBaseIrem of type \"{item.Type}\" to the backlog");
            }

            BacklogItems.Add(item);

            SessionStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task AddItemToQueue(SpotifyBaseObject item)
        {
            if (!allowedBacklogItems.Contains(item.Type.ToLower()))
            {
                throw new InvalidOperationException($"Cannot add a SpotifyBaseIrem of type \"{item.Type}\" to the manual queue");
            }

            if (item is Track singleTrack)
            {
                CurrentManualQueue.Push(singleTrack);
            }
            else
            {
                foreach (Track track in await ApiHelper.GetTracks(item, _user))
                {
                    CurrentManualQueue.Push(track);
                }
            }

            SessionStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private async Task<bool> PlayTrack(Track track, SessionContext context)
        {
            bool success = await Controller.GetInstance().PlayTrack(track.Uri, DeviceId);

            if (success)
            {
                CurrentTrack = track;
                _playedSongs.Add(new SessionHistoryItem()
                {
                    Context = context,
                    TimeStamp = DateTime.Now,
                    Track = track
                });
                SessionStateChanged?.Invoke(this, EventArgs.Empty);
            }

            return success;
        }

        private async Task<bool> PlayTrack(Tuple<Track, SessionContext> trackContext)
        {
            return await PlayTrack(trackContext.Item1, trackContext.Item2);
        }

        public async Task UpdateSession()
        {
            DataLoader dataLoader = DataLoader.GetInstance();

            _playbackState = await dataLoader.GetCurrentlyPlaying();
            
            if (_playbackState != null && _playbackState.Is_Playing && CurrentTrack != null && _playbackState.Item.Id != CurrentTrack.Id && _playbackState.Item.Id != NextTrackPeek.Id)
            {
                long playBackStarted = _playbackState.Timestamp - _playbackState.Progress_ms;

                _playedSongs.Add(new SessionHistoryItem()
                {
                    Track = _playbackState.Item,
                    TimeStamp = new DateTime(playBackStarted),
                    Context = SessionContext.Unknown
                });
            }

            SessionStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private async void RunInternal()
        {

            await Task.Run(async () =>
            {
                _playbackState = await DataLoader.GetInstance().GetCurrentlyPlaying(true, (int)SESSION_NEXT_SONG_BEVOR_END_MS - 50);
                
                long lastUpadte = _unixTimestamp;

                string lastDeviceId = null;

                long tickStart = 0;

                SessionStateChanged?.Invoke(this, EventArgs.Empty);

                while (IsRunning)
                {
                    tickStart = Environment.TickCount;

                    if (AMOUNT_ITEMS_LEFT_TO_REPOPULATED_BACKLOG >= CurrentBacklogQueue.Count())
                    {
                        await RepopulateBacklog();
                    }

                    if (_unixTimestamp >= lastUpadte + SESSION_UPDATE_SLEEP_PAUSE_MS //update when "updatetimer" tiggers
                        || _possibleMSLeftInTrack < SESSION_NEXT_SONG_BEVOR_END_MS) //update befor a PlayTrack to check whether playback is paused
                    {
                        await UpdateSession();
                        lastUpadte = _unixTimestamp;
                    }

                    //if (CurrentTrack == null || (_playbackState.Is_Playing && _possibleMSLeftInTrack < SESSION_NEXT_SONG_BEVOR_END_MS))
                    if(CurrentTrack == null || (_playbackState != null && CurrentTrack.Id == _playbackState.Item.Id && !_playbackState.Is_Playing && _playbackState.Progress_ms == 0))
                    {
                        Tuple<Track, SessionContext> nextTrackContext = PullNextTrack();

                        if (await PlayTrack(nextTrackContext))
                        {
                            Thread.Sleep(10);
                            await UpdateSession();
                        }
                        else
                        {
                            CurrentManualQueue.Push(nextTrackContext.Item1);
                        }
                    }

                    if (DeviceId != null && DeviceId != lastDeviceId)
                    {
                        await Controller.GetInstance().TransferPlayback(DeviceId, true);
                    }

                    lastDeviceId = DeviceId;

                    int sleepTime = (int)(SESSION_TICK_SLEEP_PAUSE_MS - (Environment.TickCount - tickStart));

                    if (sleepTime > 0)
                        Thread.Sleep(sleepTime);
                }
            });
        }

        public void Stop()
        {
            IsRunning = false;
        }

        private async Task RepopulateBacklog()
        {
            List<Track> tracks = await GetBacklogTracks();

            Shuffle(tracks);

            tracks.ForEach(CurrentBacklogQueue.Push);

            SessionStateChanged?.Invoke(this, EventArgs.Empty);
        }



        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public async void SkipToNext()
        {
            await PlayTrack(PullNextTrack());
        }

        public void SetActiveDevice(string deviceid)
        {
            if (!string.IsNullOrEmpty(deviceid))
            {
                this.DeviceId = deviceid;
            }
        }
    }
}
