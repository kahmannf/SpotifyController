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
    public class ViewAlbumViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        
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

        private Album _album;

        public Album Album
        {
            get
            {
                return _album;
            }
            set
            {
                _album = value;
                NotifyPropertyChanged("Album");
                foreach (string s in _albumDependingProperties)
                {
                    NotifyPropertyChanged(s);
                }
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

        string[] _albumDependingProperties = new string[] { "AlbumType", "Artists", "Genres", "ImageUrl", "Label", "Name", "Popularity", "ReleaseDate" };

        public string AlbumType => Album.Album_Type;
        public Artist[] Artists => Album.Artists;
        public string Genres => Album?.Genres != null && Album.Genres.Length > 0 ? string.Join(", ", Album.Genres) : "-";
        public string ImageUrl => Album.ImageUrl;
        public string Label => Album.Label;
        public string Name => Album.Name;
        public int Popularity => Album.Popularity;
        public string ReleaseDate => Album.Release_Date;



        public ViewAlbumViewModel(Album album)
        {
            Album = album;

            Init();
        }

        private async void Init()
        {
            UIEnabled = false;

            DataLoader dataLoader = DataLoader.GetInstance();

            Album = await dataLoader.GetItemFromHref<Album>(Album.Href);
            TrackList = new ObservableCollection<Track>((await dataLoader.GetAllItemsFromPagingWrapper(Album.Tracks)));

            UIEnabled = true;
        }
    }
}
