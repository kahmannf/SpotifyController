using SpotifyController.LoggedInWindowControls;
using SpotifyController.LoggedInWindowViewModel;
using SpotifyControllerAPI.Model;
using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Model.Spotify.AudioAnalysis;
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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SpotifyController
{
    public class ViewModelLoggedIn : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Member + NotifyPropertyChanged Method

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion

        public ViewModelLoggedIn()
        {
            Init();
        }
        

        private User _loggedInUser;

        public User LoggedInUser
        {
            get
            {
                return _loggedInUser;
            }
            private set
            {
                _loggedInUser = value;
                NotifyPropertyChanged("LoggedInUser");
            }
        }

        private List<Device> _devices;

        public List<Device> Devices
        {
            get
            {
                return _devices;
            }
            private set
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
            private set
            {
                _activeDevice = value;
                NotifyPropertyChanged("ActiveDevice");
            }
        }

        private CurrentlyPlaying _currentlyPlaying;

        public CurrentlyPlaying CurrentlyPlaying
        {
            get { return _currentlyPlaying; }
            private set
            {
                _currentlyPlaying = value;
                NotifyPropertyChanged();
            }
        }

        private Session _session;

        public Session Session
        {
            get
            {
                return _session;
            }
            set
            {
                _session = value;
                NotifyPropertyChanged("Session");
            }
        }



        private bool _uiEnabled;

        public bool UIEnabled
        {
            get
            {
                return _uiEnabled;
            }
            set
            {
                _uiEnabled = value;
                NotifyPropertyChanged("UIEnabled");
            }
        }



        private ViewModelStart _viewModelStart;

        public ViewModelStart ViewModelStart
        {
            get
            {
                return _viewModelStart;
            }
            set
            {
                _viewModelStart = value;
                NotifyPropertyChanged("ViewModelStart");
            }
        }

        private ViewModelSession _viewModelSession;

        public ViewModelSession ViewModelSession
        {
            get
            {
                return _viewModelSession;
            }
            set
            {
                _viewModelSession = value;
                NotifyPropertyChanged("ViewModelSession");
            }
        }

        private ViewModelSearch _viewModelsearch;

        public ViewModelSearch ViewModelSearch
        {
            get
            {
                return _viewModelsearch;
            }
            set
            {
                _viewModelsearch = value;
                NotifyPropertyChanged("ViewModelSearch");
            }
        }

        private ViewModelPlaylists _viewModelPlaylists;

        public ViewModelPlaylists ViewModelPlaylists
        {
            get
            {
                return _viewModelPlaylists;
            }
            set
            {
                _viewModelPlaylists = value;
                NotifyPropertyChanged("ViewModelPlaylists");
            }
        }


        private ObservableCollection<LoggedInWindowTabItem> _tabItems;

        public ObservableCollection<LoggedInWindowTabItem> TabItems
        {
            get
            {
                return _tabItems;
            }
            set
            {
                _tabItems = value;
                NotifyPropertyChanged("TabItems");
            }
        }

        private LoggedInWindowTabItem _selectedTabItem;

        public LoggedInWindowTabItem SelectedTabItem
        {
            get
            {
                return _selectedTabItem;
            }
            set
            {
                _selectedTabItem = value;
                UpdateSelectedTab();
                NotifyPropertyChanged("SelectedTabItem");
            }
        }

        private string _webStatus;

        public string WebStatus
        {
            get { return _webStatus; }
            set
            {
                _webStatus = value;
                NotifyPropertyChanged("WebStatus");
            }
        }


        public void UpdateSelectedTab()
        {
            SelectedTabItem?.ViewModel?.Update();
        }

        private async void Init()
        {
            BlockUI();

            _uiThreadDispatcher = Dispatcher.CurrentDispatcher;

            DispatcherTimer disTimer = new DispatcherTimer(DispatcherPriority.Send);
            disTimer.Tick += DisTimer_Tick;
            disTimer.Interval = new TimeSpan(0, 0, 0, 0, 800);
            disTimer.Start();

            await LoadInitialData();

            WebRequestScheduler.Instance.StatusChanged += WebRequests_StatusChanged; ;

            Session = new Session(LoggedInUser);

            Session.SessionStateChanged += Session_SessionStateChanged;

            FillTabControl();
            
            UnblockUI();
        }

        private void DisTimer_Tick(object sender, EventArgs e)
        {
            WebStatus = _asyncWebStatusMessage;
        }

        private void WebRequests_StatusChanged(object sender, WebRequestStatus e)
        {
            _lastWebStatusUpdate = Environment.TickCount;
            string message = string.Empty;

            message += $"{e.RequestsInQueue} Webrequest in Queue. Running {e.RunningThreads} Threads parallel.";

            if (e.Blocked)
            {
                message += $"Request blocked by Spotify. Waiting for {e.RemainingWaitTime / 1000} more Seconds";
            }

            _asyncWebStatusMessage = message;
        }

        private long _lastWebStatusUpdate = 0;

        string _asyncWebStatusMessage;
        

        Dispatcher _uiThreadDispatcher;

        private void Session_SessionStateChanged(object sender, EventArgs e)
        {
            if (ViewModelStart != null)
            {
                _uiThreadDispatcher.BeginInvoke((Action)(() => ViewModelStart.UpdateSessionInfo()));
            }
        }

        public async Task UpdateLoggedInUser()
        {
            _loggedInUser = await DataLoader.GetInstance().GetLoggedInUser();
        }

        public void CloseTab(LoggedInWindowTabItem tabitem)
        {
            if (SelectedTabItem == tabitem)
            {
                SelectedTabItem = TabItems[TabItems.IndexOf(tabitem) - 1];
            }

            TabItems.Remove(tabitem);
        }

        public async Task UpdateDevices()
        {
            DevicePayloadItem devicePayload = await DataLoader.GetInstance().GetUserDevices();

            Device activeDevice = null;

            if (devicePayload != null && devicePayload.Devices != null)
            {
                foreach (Device d in devicePayload.Devices)
                {
                    if (d.Images == null || d.Images.Length == 0)//to be expected
                    {
                        string imagePath = string.Empty;

                        switch (d.Type.ToLower())
                        {
                            case "computer":
                            case "smartphone":
                            case "speaker":
                                imagePath = "../images/" + d.Type.ToLower() + ".png";
                                break;
                            default:
                                imagePath = "../images/defaultimage.png";
                                break;
                        }

                        d.Images = new SpotifyControllerAPI.Model.Spotify.Image[]
                        {
                        new SpotifyControllerAPI.Model.Spotify.Image()
                        {
                            Url = imagePath
                        }
                        };
                    }
                }

                Devices = new List<Device>(devicePayload.Devices);

                activeDevice = Devices.FirstOrDefault(x => x.Is_Active);
            }
            else
            {
                Devices = new List<Device>();
            }

            if (activeDevice == null)
            {

            }


            if (activeDevice == null) //create a placeholder
            {
                activeDevice = new Device
                {
                    Is_Active = true,
                    Name = "None",
                    Images = new SpotifyControllerAPI.Model.Spotify.Image[]
                    {
                        new SpotifyControllerAPI.Model.Spotify.Image()
                        {
                            Url = "../images/none.png"
                        }
                    },
                    Type = "none",
                    Volume_Percent = 0
                };
            }

            ActiveDevice = activeDevice;
        }

        public async Task UpdateCurrentlyPlaying()
        {
            CurrentlyPlaying = await DataLoader.GetInstance().GetCurrentlyPlaying();
        }

        private void FillTabControl()
        {
            LoggedInWindowTabItem tabStart = new LoggedInWindowTabItem()
            {
                Name = "start",
                Content = new UserControlStart()
            };

            LoggedInWindowTabItem tabSession = new LoggedInWindowTabItem()
            {
                Name = "session",
                Content = new UserControlSession()
            };

            LoggedInWindowTabItem tabSearch = new LoggedInWindowTabItem()
            {
                Name = "search",
                Content = new UserControlSearch()
            };

            LoggedInWindowTabItem tabPlaylist = new LoggedInWindowTabItem()
            {
                Name = "playlist",
                Content = new UserControlPlaylist()
            };

            LoggedInWindowTabItem tabLibrary = new LoggedInWindowTabItem()
            {
                Name = "library",
                Content = new UserControlPlaylist()
            };

            LoggedInWindowTabItem tabUser = new LoggedInWindowTabItem()
            {
                Name = "user",
                Content = new UserControlPlaylist()
            };

            LoggedInWindowTabItem tabFeatured = new LoggedInWindowTabItem()
            {
                Name = "featured",
                Content = new UserControlPlaylist()
            };


            TabItems = new ObservableCollection<LoggedInWindowTabItem>()
            {
                tabStart, tabSession, tabSearch, tabPlaylist, tabLibrary, tabUser, tabFeatured
            };

            //Tabitems have to be created for viemodel start

            tabStart.ViewModel = ViewModelStart = new ViewModelStart(this);

            tabSession.ViewModel = ViewModelSession = new ViewModelSession(this);

            tabSearch.ViewModel = ViewModelSearch = new ViewModelSearch(this);

            tabPlaylist.ViewModel = ViewModelPlaylists = new ViewModelPlaylists(this);

            //temp
            tabLibrary.ViewModel = tabUser.ViewModel = tabFeatured.ViewModel = new ViewModelPlaylists(this);

            SelectedTabItem = tabStart;
        }


        private async Task LoadInitialData()
        {

            _loggedInUser = await DataLoader.GetInstance().GetLoggedInUser();

        }

        int blockCount = 0;

        public void BlockUI()
        {
            blockCount++;
            UIEnabled = false;
        }

        public void UnblockUI()
        {
            blockCount--;
            if (blockCount == 0)
            {
                UIEnabled = true;
            }
        }

        public void ViewSpotifyBaseObject<TSpotifyObject>(TSpotifyObject viewObject)
            where TSpotifyObject : SpotifyBaseObject, new()
        {
            LoggedInWindowTabItem viewTab = null;

            viewTab = TabItems.FirstOrDefault(x => x.ViewModel is ViewModelViewObject<TSpotifyObject> vm && vm.ViewSource is TSpotifyObject obj && obj.Id == viewObject.Id);

            if (viewTab == null)

            {
                UserControlViewObject content = new UserControlViewObject();

                content.RemoveByEvent += ViewContent_CloseByEvent;

                viewTab = new LoggedInWindowTabItem(true)
                {
                    Content = content,
                    //Name = viewObject.Name, name will be set inside of the viewmodel<T>
                    ViewModel = new ViewModelViewObject<TSpotifyObject>(this, viewObject)
                };



                TabItems.Add(viewTab);
            }

            SelectedTabItem = viewTab;
        }

        private void ViewContent_CloseByEvent(object sender, EventArgs e)
        {
            if (sender is UserControlViewObject content && TabItems.FirstOrDefault(x => x.Content == content) is LoggedInWindowTabItem tabItem)
            {
                content.RemoveByEvent -= ViewContent_CloseByEvent;
                
                TabItems.Remove(tabItem);
            }
        }

        public async void ViewAudioAnalysis(Track track)
        {
            AudioAnalysis analysis = await DataLoader.GetInstance().GetAudioAnalysis(track.Id);

            MessageBox.Show("Analysis retrieved. UI not implemented");
        }
    }
}
