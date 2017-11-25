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

namespace SpotifyController.Controls.ViewSpotifyObjectControls
{
    public class ViewPlaylistViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ViewPlaylistViewModel(Playlist playlist)
        {
            Playlist = playlist;

            Init();
        }

        private Playlist _playlist;
        public Playlist Playlist
        {
            get => _playlist;
            set
            {
                _playlist = value;
                NotifyPropertyChanged("Playlist");
                NotifyPropertyChanged("PlaylistOwnerName");
            }
        }

        private ObservableCollection<Track> _trackList;

        public ObservableCollection<Track> TrackList
        {
            get
            {
                return _trackList;
            }
            set
            {
                _trackList = value;
                NotifyPropertyChanged("TrackList");
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



        private async void Init()
        {
            UIEnabled = false;

            DataLoader dataLoader = DataLoader.GetInstance();

            Playlist = await dataLoader.GetItemFromHref<Playlist>(Playlist.Href);
            TrackList = new ObservableCollection<Track>((await dataLoader.GetAllItemsFromPagingWrapper(Playlist.Tracks)).Select(x => x.Track));

            UIEnabled = true;
        }
    }
}
