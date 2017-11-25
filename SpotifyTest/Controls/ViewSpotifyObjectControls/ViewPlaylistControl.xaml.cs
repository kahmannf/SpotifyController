using SpotifyController.LoggedInWindowViewModel;
using SpotifyControllerAPI.Model.Spotify;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SpotifyController.Controls.ViewSpotifyObjectControls
{
    /// <summary>
    /// Interaktionslogik für ViewPlaylistControl.xaml
    /// </summary>
    public partial class ViewPlaylistControl : UserControl
    {
        public ViewPlaylistControl()
        {
            InitializeComponent();

            this.DataContextChanged += ViewPlaylistControl_DataContextChanged;
        }

        private void ViewPlaylistControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is ViewModelBaseViewObject viewBase && viewBase.BaseViewSource is Playlist playlist)
            {
                LayoutGrid.DataContext = new ViewPlaylistViewModel(playlist);
            }
        }
    }
}
