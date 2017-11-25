using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyController.LoggedInWindowViewModel
{
    public class ViewModelPlaylists : TabItemBaseViewModel, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Member + NotifyPropertyChanged Method

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
        
        public ViewModelPlaylists(ViewModelLoggedIn parent) : base(parent)
        {
        }

        private List<Playlist> _playlists;

        public List<Playlist> Playlists
        {
            get { return _playlists; }
            set
            {
                _playlists = value;
                NotifyPropertyChanged("Playlists");
            }
        }

        
        private async Task LoadPlaylists()
        {
            DataLoader loader = DataLoader.GetInstance();

            var firstPage = await loader.GetUsersPlaylistPage(50, 0);

            Playlists = await loader.GetAllItemsFromPagingWrapper(firstPage);
        }

        public void AddToSession(Playlist p)
        {
            var result = System.Windows.MessageBox.Show($"Do you want to add {p.Tracks.Total} Track(s) to the backlog?", "Add to session?", System.Windows.MessageBoxButton.YesNo);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _parent.Session.AddItemToBacklog(p);
            }
        }

        public async void AddToQueue(Playlist p)
        {
            var result = System.Windows.MessageBox.Show($"Do you want to add {p.Tracks.Total} to the queue?", "Add to session?", System.Windows.MessageBoxButton.YesNo);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                IEnumerable<Track> tracks = (await DataLoader.GetInstance().GetAllItemsFromPagingWrapper(p.Tracks)).Select(x => x.Track);

                foreach (Track t in tracks)
                {
                    _parent.Session.CurrentManualQueue.Push(t);
                }
            }
        }

        public override async void Update()
        {
            _parent.BlockUI();

            await LoadPlaylists();

            _parent.UnblockUI();
        }
    }
}
