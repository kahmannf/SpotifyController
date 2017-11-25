using SpotifyController.LoggedInWindowViewModel;
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

namespace SpotifyController.LoggedInWindowControls
{
    /// <summary>
    /// Interaktionslogik für UserControlPlaylist.xaml
    /// </summary>
    public partial class UserControlPlaylist : UserControl
    {
        public UserControlPlaylist()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as ViewModelPlaylists).ReturnHome();
        }

        private void ListViewButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
