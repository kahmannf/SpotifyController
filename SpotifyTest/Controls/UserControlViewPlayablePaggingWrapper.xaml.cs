using SpotifyControllerAPI.Model;
using SpotifyControllerAPI.Model.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotifyController.Controls
{
    /// <summary>
    /// Interaktionslogik für UserControlViewPlayablePaggingWrapper.xaml
    /// </summary>
    public partial class UserControlViewPlayablePaggingWrapper : UserControl
    {
        public UserControlViewPlayablePaggingWrapper()
        {
            InitializeComponent();

            this.DataContextChanged += UserControlViewPlayablePaggingWrapper_DataContextChanged;
        }

        private void UserControlViewPlayablePaggingWrapper_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VMPagingBase vm;
            switch (this.DataContext)
            {
                case PagingWrapper<Track> tracks:
                    vm = new ViewModelViewPlayablePagingWrapper<Track>(tracks);
                    break;
                case PagingWrapper<Album> albums:
                    vm = new ViewModelViewPlayablePagingWrapper<Album>(albums);
                    break;
                case PagingWrapper<Artist> artists:
                    vm = new ViewModelViewPlayablePagingWrapper<Artist>(artists);
                    break;
                case PagingWrapper<Playlist> playlists:
                    vm = new ViewModelViewPlayablePagingWrapper<Playlist>(playlists);
                    break;
                case PlaylistAggregationSearchResult aggrSearchResult:
                    vm = new ViewModelAggregationSearchResult(aggrSearchResult);
                    break;
                default:
                    vm = null;
                    break;
            }

            LayoutRoot.DataContext = vm;
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as VMPagingBase)?.PreviousPage();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as VMPagingBase)?.NextPage();
        }
    }
}
