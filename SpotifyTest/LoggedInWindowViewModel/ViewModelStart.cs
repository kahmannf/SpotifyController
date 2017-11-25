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
    public class ViewModelStart : TabItemBaseViewModel, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Member + NotifyPropertyChanged Method

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion

        private User _loggedInUser;
        
        public ViewModelStart(ViewModelLoggedIn parent) : base(parent)
        {
            _loggedInUser = parent.LoggedInUser;
            
            ManualQueueTracks = new List<Track>();
            BacklogQueueTracks = new List<Track>();
        }
        
        public ObservableCollection<LoggedInWindowTabItem> Tabs
        {
            get
            {
                return _parent.TabItems;
            }
            set
            {
                _parent.TabItems = value;
                NotifyPropertyChanged("Tabs");
            }
        }

        private string _displayName;

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }

        private DateTime _birthDate;

        public DateTime BirthDate
        {
            get
            {
                return _birthDate;
            }
            set
            {
                _birthDate = value;
                NotifyPropertyChanged("BirthDate");
            }
        }

        public void CloseTab(LoggedInWindowTabItem tabitem)
        {
            _parent.CloseTab(tabitem);
        }

        private string _imageUrl;

        public string ImageUrl
        {
            get
            {
                //seems like the debugger doesnt like weblink & image combinations...
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return "../images/debugging.jpg";
                }

                return _imageUrl;
            }
            set
            {
                _imageUrl = value;
                NotifyPropertyChanged("ImageUrl");
            }
        }

        public Visibility NoItemsQueueVisibility
        {
            get
            {
                return _manualQueueTracks?.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility NoItemsBacklogVisibility
        {
            get
            {
                return _backlogQueueTracks?.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private List<Track> _manualQueueTracks;

        public List<Track> ManualQueueTracks
        {
            get { return _manualQueueTracks; }
            set
            {
                _manualQueueTracks = value;
                NotifyPropertyChanged("ManualQueueTracks");
                NotifyPropertyChanged("ManualQueueTracks");
            }
        }

        private List<Track> _backlogQueueTracks;

        public List<Track> BacklogQueueTracks
        {
            get { return _backlogQueueTracks; }
            set
            {
                _backlogQueueTracks = value;
                NotifyPropertyChanged("BacklogQueueTracks");
                NotifyPropertyChanged("NoItemsBacklogVisibility");
            }
        }


        private string _currentSong;

        public string CurrentSong
        {
            get { return _currentSong; }
            set
            {
                _currentSong = value;
                NotifyPropertyChanged("CurrentSong");
            }
        }

        private string _currentArtists;

        public string CurrentArtists
        {
            get { return _currentArtists; }
            set
            {
                _currentArtists = value;
                NotifyPropertyChanged("CurrentArtists");
            }
        }

        private string _currentSongImage;

        public string CurrentSongImage
        {
            get
            {
                if (string.IsNullOrEmpty(_currentSongImage))
                {
                    return ("../images/defaultimage.jpg");
                }
                else
                {
                    return _currentSongImage;
                }

            }
            set
            {
                _currentSongImage = value;
                NotifyPropertyChanged("CurrentSongImage");
            }
        }



        private bool _enableSessionControl;

        public bool EnableSessionControl
        {
            get { return _enableSessionControl; }
            set
            {
                _enableSessionControl = value;
                NotifyPropertyChanged("EnableSessionControl");
            }
        }



        public void SwitchTab(string targetName)
        {
            SwitchTab(Tabs.FirstOrDefault(x => x.Name == targetName));
        }

        public void SwitchTab(LoggedInWindowTabItem target)
        {
            if (target != null)
                _parent.SelectedTabItem = target;
        }

        public override async void Update()
        {
            _parent.BlockUI();

            await UpdateLoggedInUserInfo();

            UpdateSessionInfo();

            _parent.UnblockUI();
        }

        private async Task UpdateLoggedInUserInfo()
        {
            await _parent.UpdateLoggedInUser();

            DisplayName = _loggedInUser.Display_Name;

            BirthDate = _loggedInUser.BirthDate;


            if (_loggedInUser.Images.Length > 0)
            {
                ImageUrl = _loggedInUser.Images[0].Url;
            }
            else
            {
                ImageUrl = "../images/defaultimage.jpg";
            }
        }

        public void UpdateSessionInfo()
        {
            BacklogQueueTracks = new List<Track>(_parent.Session.CurrentBacklogQueue.PeekAll());
            ManualQueueTracks = new List<Track>(_parent.Session.CurrentManualQueue.PeekAll());

            if (_parent.Session.CurrentTrack != null)
            {
                CurrentSong = _parent.Session.CurrentTrack.Name;
                CurrentArtists = _parent.Session.CurrentTrack.ArtistNames;

                if (_parent.Session.CurrentTrack.Album != null)
                {
                }
            }

            EnableSessionControl = _parent.Session.IsRunning;
        }

        public void NextTrack()
        {
            _parent.Session.SkipToNext();
        }
    }
}
