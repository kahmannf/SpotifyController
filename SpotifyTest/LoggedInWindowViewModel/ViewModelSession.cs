using SpotifyControllerAPI.Model;
using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Web;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpotifyController.LoggedInWindowViewModel
{
    public class ViewModelSession : TabItemBaseViewModel, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Member + NotifyPropertyChanged Method

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
        
        private bool _isSessionRunning;

        public bool IsSessionRunning
        {
            get { return _isSessionRunning; }
            set
            {
                _isSessionRunning = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("CanSessionBeStarted");
                NotifyPropertyChanged("StopSessionButtonText");

                CanHighjackSession = !IsSessionRunning && _parent.CurrentlyPlaying?.Context != null;
            }
        }

        public bool CanSessionBeStarted { get => !IsSessionRunning; }

        private bool _canHighjackSession;

        public bool CanHighjackSession
        {
            get { return _canHighjackSession; }
            set
            {
                _canHighjackSession = value;
                NotifyPropertyChanged();

                HighjackPanelVisibility = CanHighjackSession ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _highjackPanelVisibility;

        public Visibility HighjackPanelVisibility
        {
            get { return _highjackPanelVisibility; }
            set
            {
                _highjackPanelVisibility = value;
                NotifyPropertyChanged();
            }
        }


        private bool _isCurrentlyPlaying;

        public bool IsCurrentlyPlaying
        {
            get { return _isCurrentlyPlaying; }
            set
            {
                _isCurrentlyPlaying = value;
                NotifyPropertyChanged();
            }
        }

        public string StopSessionButtonText
        {
            get => IsSessionRunning ? "Stop Session" : "Clear Session";
        }


        private long _lastUpdateTime;

        #region Currently Playing Formularproperties

        private string _playbackType;

        public string PlaybackType
        {
            get
            {
                if (string.IsNullOrEmpty(_playbackType))
                    return string.Empty;

                char[] a = _playbackType.ToCharArray();
                a[0] = char.ToUpper(a[0]);
                return new string(a);
            }
            set
            {
                _playbackType = value;
                NotifyPropertyChanged("PlaybackType");
                NotifyPropertyChanged("CurrentlyPlayingString");
            }
        }

        private string _playbackContextName;

        public string PlaybackContextName
        {
            get
            {
                return _playbackContextName;
            }
            set
            {
                _playbackContextName = value;
                NotifyPropertyChanged("PlaybackContextName");
                NotifyPropertyChanged("CurrentlyPlayingString");
            }
        }

        public string CurrentlyPlayingString
        {
            get
            {
                if (string.IsNullOrEmpty(PlaybackType) && string.IsNullOrEmpty(PlaybackContextName))
                {
                    return string.Empty;
                }
                else if (string.IsNullOrEmpty(PlaybackContextName))
                {
                    return PlaybackType;
                }
                else if (string.IsNullOrEmpty(PlaybackType))
                {
                    return PlaybackContextName;
                }
                else
                {
                    return PlaybackType + " - " + PlaybackContextName;
                }
            }
        }

        private string _currentlyPlayingImage;

        public string CurrentlyPlayingImage
        {
            get
            {
                //seems like the debugger doesnt like weblink & image combinations...
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return "../images/debugging.jpg";
                }

                return _currentlyPlayingImage;
            }
            set
            {
                _currentlyPlayingImage = value;
                NotifyPropertyChanged("CurrentlyPlayingImage");
            }
        }


        #endregion


        private ObservableCollection<Device> _devices;

        public ObservableCollection<Device> Devices
        {
            get
            {
                return _devices;
            }
            set
            {
                _devices = value;
                NotifyPropertyChanged("Devices");
            }
        }

        private Device _activeDevice;

        public Device ActiveDevice
        {
            get
            {
                return _activeDevice;
            }
            set
            {
                _activeDevice = value;
                NotifyPropertyChanged("ActiveDevice");
            }
        }

        private ObservableCollection<SpotifyBaseObject> _sessionItems;

        public ObservableCollection<SpotifyBaseObject> SessionItems
        {
            get
            {
                return _sessionItems;
            }
            set
            {
                _sessionItems = value;
                NotifyPropertyChanged("SessionItems");
            }
        }



        public ViewModelSession(ViewModelLoggedIn parent) : base(parent)
        {
            _parent.Session.SessionStateChanged += Session_SessionStateChanged;
        }

        private void Session_SessionStateChanged(object sender, EventArgs e)
        {
            UpdateSessionItems();
        }

        public override async void Update()
        {
            _parent.BlockUI();

            await LoadCurrentlyPlayingAsync();
            await LoadDevicesAsync();
            UpdateSessionItems();

            _parent.UnblockUI();
        }

        private async Task LoadCurrentlyPlayingAsync()
        {
            DataLoader dataLoader = DataLoader.GetInstance();

            await _parent.UpdateCurrentlyPlaying();

            if (_parent.CurrentlyPlaying != null)
            {
                _lastUpdateTime = _parent.CurrentlyPlaying.Timestamp;

                IsCurrentlyPlaying = _parent.CurrentlyPlaying.Is_Playing;

                IsSessionRunning = _parent.Session.IsRunning;

                SpotifyBaseObject contextItem = null;

                if (_parent.CurrentlyPlaying.Context != null)
                {
                    PlaybackType = _parent.CurrentlyPlaying.Context.Type;


                    switch (_parent.CurrentlyPlaying.Context.Type)
                    {
                        case "album":
                            contextItem = _parent.CurrentlyPlaying.Item.Album;
                            break;
                        default:
                            contextItem = await dataLoader.GetItemFromHref<SpotifyBaseObject>(_parent.CurrentlyPlaying.Context.Href);
                            break;
                    }


                    PlaybackContextName = contextItem.Name;
                }

                if (contextItem != null && contextItem.Images.Length > 0)
                {
                    CurrentlyPlayingImage = contextItem.Images[0].Url;
                }
                else
                {
                    CurrentlyPlayingImage = "../images/defaultimage.jpg";
                }
            }
            else
            {
                IsCurrentlyPlaying = false;
                CanHighjackSession = false;
                HighjackPanelVisibility = Visibility.Collapsed;
            }
        }

        private async Task LoadDevicesAsync()
        {
            await _parent.UpdateDevices();
            
            Devices = new ObservableCollection<Device>(_parent.Devices);

            ActiveDevice = _parent.ActiveDevice;
        }

        private void UpdateSessionItems()
        {
            IsSessionRunning = _parent.Session.IsRunning;
            SessionItems = new ObservableCollection<SpotifyBaseObject>(_parent.Session.BacklogItems);
        }

        public async void HighjackSession()
        {
            if (CanHighjackSession)
            {
                DataLoader dataLoader = DataLoader.GetInstance();

                switch (_parent.CurrentlyPlaying.Context.Type.ToLower())
                {
                    case "playlist":
                        Playlist playlist = await dataLoader.GetItemFromHref<Playlist>(_parent.CurrentlyPlaying.Context.Href);
                        _parent.Session.AddItemToBacklog(playlist);
                        break;
                    case "album":
                        Album album = await dataLoader.GetItemFromHref<Album>(_parent.CurrentlyPlaying.Context.Href);
                        _parent.Session.AddItemToBacklog(album);
                        break;
                    case "artist":
                        Artist artist = await dataLoader.GetItemFromHref<Artist>(_parent.CurrentlyPlaying.Context.Href);
                        _parent.Session.AddItemToBacklog(artist);
                        break;
                    default:
                        throw new ArgumentException($"Unknown type for CurrentlyPlaying.Context.Type. Expected \"playlist\", \"album\" or \"artist\". Got \"{_parent.CurrentlyPlaying.Context.Type.ToLower()}\"");
                }

                StartSession();

                Update();
            }
        }
        

        public void StartSession()
        {
            if (_parent.Session.BacklogItems.Count == 0)
            {
                MessageBox.Show("CAnnot start a session that has no items");
            }
            else
            {
                _parent.BlockUI();

                _parent.Session.Start();

                _parent.UnblockUI();
            }
        }

        private void ClearSession()
        {
            if(IsSessionRunning)
            {
                throw new InvalidOperationException("Cannot clear the session while it is running");
            }

            _parent.Session = new Session(_parent.LoggedInUser);
        }

        public async void StopSession()
        {
            if (IsSessionRunning)
            {
                _parent.Session.Stop();
            }
            else
            {
                ClearSession();
            }

            await Task.Run(() => { System.Threading.Thread.Sleep(500); });

            Update();
        }
    }
}
